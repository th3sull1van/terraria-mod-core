using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TerrariaModCore.API;
using TerrariaModCore.Configuration;

namespace TerrariaModCore
{
    /// <summary>
    /// Handles discovery, validation, isolated assembly loading, and lifecycle execution of plugins.
    /// Implements strict error boundaries to ensure that mod failures never crash the loader or other mods.
    /// </summary>
    public class ModLoader
    {
        private readonly ILogger _logger;
        private readonly ModRegistry _registry;

        public ModLoader(ILogger logger, ModRegistry registry)
        {
            _logger = logger;
            _registry = registry;
        }

        public List<ModManifest> DiscoverMods(string modsDirectory)
        {
            var manifests = new List<ModManifest>();
            if (string.IsNullOrEmpty(modsDirectory) || !Directory.Exists(modsDirectory))
            {
                _logger?.Warning($"Mods directory not found: '{modsDirectory}'");
                return manifests;
            }

            string[] subDirs = Directory.GetDirectories(modsDirectory);
            foreach (string dir in subDirs)
            {
                string manifestPath = Path.Combine(dir, "manifest.json");
                if (!File.Exists(manifestPath))
                {
                    continue;
                }

                try
                {
                    string json = File.ReadAllText(manifestPath, Encoding.UTF8);
                    var manifest = SimpleJson.Deserialize<ModManifest>(json);

                    if (manifest == null || string.IsNullOrWhiteSpace(manifest.Id))
                    {
                        _logger?.Warning($"Invalid manifest at {manifestPath}: Missing 'id'.");
                        continue;
                    }

                    var modInfo = new ModInfo
                    {
                        Manifest = manifest,
                        State = manifest.Enabled ? ModState.Discovered : ModState.Disabled
                    };

                    _registry.Register(modInfo);
                    manifests.Add(manifest);
                    _logger?.Debug($"Discovered mod: {manifest.Name} ({manifest.Id}) v{manifest.Version} [Enabled: {manifest.Enabled}]");
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Failed to parse manifest at {manifestPath}", ex);
                }
            }

            return manifests;
        }

        public bool LoadMod(ModManifest manifest, string modDirectory, IModContext context, out IMod modInstance)
        {
            modInstance = null;
            string modId = manifest?.Id ?? "unknown";

            try
            {
                if (manifest == null)
                {
                    throw new ArgumentNullException(nameof(manifest));
                }

                string asmFileName = manifest.EntryAssembly;
                string asmPath = Path.Combine(modDirectory, asmFileName);
                if (!File.Exists(asmPath))
                {
                    if (File.Exists(asmPath + ".dll")) asmPath += ".dll";
                    else if (File.Exists(asmPath + ".exe")) asmPath += ".exe";
                }

                Assembly modAsm = null;
                if (File.Exists(asmPath))
                {
                    modAsm = Assembly.LoadFrom(asmPath);
                }
                else
                {
                    // Check already loaded assemblies in current domain
                    string pureName = Path.GetFileNameWithoutExtension(asmFileName);
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (string.Equals(asm.GetName().Name, pureName, StringComparison.OrdinalIgnoreCase))
                        {
                            modAsm = asm;
                            break;
                        }
                    }
                }

                if (modAsm == null)
                {
                    throw new FileNotFoundException($"Mod entry assembly '{asmFileName}' not found at: {asmPath}");
                }
                Type entryType = modAsm.GetType(manifest.EntryType);

                if (entryType == null)
                {
                    throw new TypeLoadException($"Mod entry type '{manifest.EntryType}' not found in assembly '{asmFileName}'.");
                }

                if (!typeof(IMod).IsAssignableFrom(entryType))
                {
                    throw new InvalidCastException($"Mod entry type '{entryType.FullName}' does not implement interface '{typeof(IMod).FullName}'.");
                }

                modInstance = (IMod)Activator.CreateInstance(entryType);
                if (modInstance == null)
                {
                    throw new InvalidOperationException($"Failed to create instance of mod entry type '{entryType.FullName}'.");
                }

                // 1. Initialize
                _logger?.Debug($"Initializing mod '{modId}'...");
                modInstance.Initialize(context);

                // 2. Load
                _logger?.Debug($"Loading mod '{modId}'...");
                modInstance.Load();

                var modInfo = _registry.GetMod(modId);
                if (modInfo != null)
                {
                    modInfo.Instance = modInstance;
                    modInfo.Context = context;
                    modInfo.State = ModState.Loaded;
                }

                _logger?.Info($"[TMC:{modId}] Successfully loaded v{manifest.Version}");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to load mod '{modId}': {ex.Message}", ex);
                _registry.UpdateState(modId, ModState.Failed, ex.Message);

                // Clean up any patches registered before failure
                try
                {
                    context.PatchManager?.UnpatchAll(modId);
                }
                catch { }

                return false;
            }
        }

        public bool UnloadMod(string modId)
        {
            var modInfo = _registry.GetMod(modId);
            if (modInfo == null || modInfo.State != ModState.Loaded || modInfo.Instance == null)
            {
                return false;
            }

            try
            {
                _logger?.Info($"Unloading mod '{modId}'...");
                modInfo.Instance.Unload();
                modInfo.Context?.PatchManager?.UnpatchAll(modId);
                modInfo.State = ModState.Unloaded;
                _logger?.Info($"Mod '{modId}' unloaded successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error occurred while unloading mod '{modId}'", ex);
                modInfo.State = ModState.Failed;
                return false;
            }
        }
    }
}
