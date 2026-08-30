using System;
using TerrariaModCore.API;

namespace TerrariaModCore
{
    /// <summary>
    /// Implements mod-scoped context for isolation and controlled access to host services.
    /// </summary>
    public class ModContext : IModContext
    {
        public string Id { get; }
        public ModManifest Manifest { get; }
        public string ModDirectory { get; }
        public string ConfigDirectory { get; }
        public ILogger Logger { get; }
        public IConfigManager ConfigManager { get; }
        public IPatchManager PatchManager { get; }
        public string GameVersion { get; }
        public string CoreVersion => ModEngine.Version;

        public ModContext(
            ModManifest manifest,
            string modDirectory,
            string configDirectory,
            ILogger logger,
            IConfigManager configManager,
            IPatchManager patchManager,
            string gameVersion)
        {
            Manifest = manifest;
            Id = manifest?.Id ?? string.Empty;
            ModDirectory = modDirectory;
            ConfigDirectory = configDirectory;
            Logger = logger;
            ConfigManager = configManager;
            PatchManager = patchManager;
            GameVersion = gameVersion;
        }
    }
}
