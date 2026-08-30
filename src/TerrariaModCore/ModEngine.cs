using System;
using System.Collections.Generic;
using System.IO;
using TerrariaModCore.API;
using TerrariaModCore.Compatibility;
using TerrariaModCore.Configuration;
using TerrariaModCore.Dependencies;
using TerrariaModCore.Diagnostics;
using TerrariaModCore.Logging;
using TerrariaModCore.Patching;

namespace TerrariaModCore
{
    /// <summary>
    /// Central TMC Host Engine managing initialization, lifecycle, discovery, patching, and mod resolution.
    /// </summary>
    public class ModEngine
    {
        public const string Version = "1.2.0";

        public CoreLogger Logger { get; }
        public CoreConfig Config { get; }
        public ModRegistry Registry { get; }
        public ModLoader Loader { get; }
        public PatchManager PatchManager { get; }
        public DependencyResolver DependencyResolver { get; }

        public string BaseDirectory { get; }
        public string ModsDirectory { get; }
        public string TmcDirectory { get; }

        public ModEngine(string baseDirectory)
        {
            BaseDirectory = string.IsNullOrEmpty(baseDirectory) ? AppDomain.CurrentDomain.BaseDirectory : baseDirectory;
            TmcDirectory = Path.Combine(BaseDirectory, "TMC");
            ModsDirectory = Path.Combine(BaseDirectory, "mods");

            string logPath = Path.Combine(TmcDirectory, "logs", "tmc.log");
            Logger = new CoreLogger(logPath, "TMC");

            Registry = new ModRegistry();
            Loader = new ModLoader(Logger, Registry);
            PatchManager = new PatchManager(Logger);
            DependencyResolver = new DependencyResolver();

            // Load Core Config
            string coreConfigPath = Path.Combine(TmcDirectory, "config", "core.json");
            Config = LoadCoreConfig(coreConfigPath);
            Logger.MinimumLevel = Config.LogLevel;
        }

        public void InitializeAndLoadAll()
        {
            Logger.Info($"Initializing TerrariaModCore Host v{Version}...");

            // 1. Validate Game Version
            if (!GameVersionChecker.ValidateTerrariaVersion(out string detectedVersion))
            {
                Logger.Warning($"Terraria version mismatch! Detected: {detectedVersion}, Expected: {GameVersionChecker.TargetTerrariaVersion}");
            }
            else
            {
                Logger.Info($"Terraria version verified: {detectedVersion}");
            }

            // Apply engine compatibility and race-condition guards
            CoreFixPatches.Apply(PatchManager.HarmonyInstance);

            // 2. Discover Mods
            Logger.Info($"Scanning mods directory: '{ModsDirectory}'");
            var discoveredManifests = Loader.DiscoverMods(ModsDirectory);
            Logger.Info($"Discovered {discoveredManifests.Count} mod(s).");

            // 3. Resolve Dependencies and Order
            var resolution = DependencyResolver.Resolve(discoveredManifests);
            if (!resolution.Success)
            {
                foreach (var err in resolution.Errors)
                {
                    Logger.Error($"Dependency Resolution Error: {err}");
                }
            }

            // 4. Load Resolved Mods
            foreach (var manifest in resolution.OrderedMods)
            {
                string modDir = Registry.GetMod(manifest.Id).Directory;

                var modLogger = new ModLogger(Logger, manifest.Id);
                var modConfigManager = new ModConfigManager(modDir, modLogger);
                var modContext = new ModContext(
                    manifest,
                    modDir,
                    modDir,
                    modLogger,
                    modConfigManager,
                    PatchManager,
                    detectedVersion);

                Loader.LoadMod(manifest, modDir, modContext, out _);
            }

            // 5. Diagnostics Banner
            if (Config.DiagnosticBannerOnStartup)
            {
                StartupDiagnostics.PrintSummary(Version, detectedVersion, Registry.GetAllMods());
            }
        }

        public void Shutdown()
        {
            Logger.Info("Shutting down TerrariaModCore...");
            var mods = Registry.GetAllMods();
            for (int i = mods.Count - 1; i >= 0; i--)
            {
                if (mods[i].State == ModState.Loaded)
                {
                    Loader.UnloadMod(mods[i].Manifest.Id);
                }
            }
            Logger.Info("TerrariaModCore shutdown complete.");
        }

        private CoreConfig LoadCoreConfig(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var cfg = SimpleJson.Deserialize<CoreConfig>(json);
                    if (cfg != null) return cfg;
                }
                else
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    var def = new CoreConfig();
                    File.WriteAllText(path, SimpleJson.Serialize(def, true));
                    return def;
                }
            }
            catch (Exception ex)
            {
                Logger?.Warning($"Failed to load core configuration ({ex.Message}), using defaults.");
            }
            return new CoreConfig();
        }
    }
}
