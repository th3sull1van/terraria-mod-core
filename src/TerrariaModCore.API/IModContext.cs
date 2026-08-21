using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Provides controlled access to mod metadata, isolation paths, logging, configuration, patching, and events.
    /// </summary>
    public interface IModContext
    {
        /// <summary>
        /// Gets the unique identifier of the mod as declared in its manifest.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the loaded manifest metadata for this mod.
        /// </summary>
        ModManifest Manifest { get; }

        /// <summary>
        /// Gets the absolute directory path of the mod folder.
        /// </summary>
        string ModDirectory { get; }

        /// <summary>
        /// Gets the directory where mod configurations are persisted.
        /// </summary>
        string ConfigDirectory { get; }

        /// <summary>
        /// Gets the scoped logger for this mod.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the configuration management service.
        /// </summary>
        IConfigManager ConfigManager { get; }

        /// <summary>
        /// Gets the centralized patch manager for registering Harmony patches.
        /// </summary>
        IPatchManager PatchManager { get; }

        /// <summary>
        /// Gets the event bus for subscribing to and publishing cross-mod / game events.
        /// </summary>
        IEventBus EventBus { get; }

        /// <summary>
        /// Gets shared game services.
        /// </summary>
        IGameServices GameServices { get; }

        /// <summary>
        /// Gets the detected Terraria game version string.
        /// </summary>
        string GameVersion { get; }

        /// <summary>
        /// Gets the TMC Core version string.
        /// </summary>
        string CoreVersion { get; }
    }
}
