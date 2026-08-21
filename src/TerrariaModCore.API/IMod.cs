using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Lifecycle interface for all TerrariaModCore plugins.
    /// Every mod must implement this interface in its declared entry type.
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// Called once when the mod is loaded and before patches are applied.
        /// Use this to initialize configuration, services, and register Harmony patches.
        /// </summary>
        /// <param name="context">The unique context provided by TMC Host.</param>
        void Initialize(IModContext context);

        /// <summary>
        /// Called after all mods have completed initialization and patches have been applied.
        /// Use this to begin mod operations and subscribe to game events.
        /// </summary>
        void Load();

        /// <summary>
        /// Called when the mod is unloaded or disabled at runtime.
        /// Clean up event subscriptions, timers, unmanaged resources, and reset state.
        /// </summary>
        void Unload();
    }
}
