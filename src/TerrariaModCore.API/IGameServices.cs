using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Provides shared game state queries, version checks, and safe environment utilities.
    /// </summary>
    public interface IGameServices
    {
        /// <summary>
        /// Gets the detected Terraria version string.
        /// </summary>
        string GameVersion { get; }

        /// <summary>
        /// Gets whether the game is currently in menus or actively playing in a world.
        /// </summary>
        bool IsInWorld { get; }

        /// <summary>
        /// Gets whether the game is in multiplayer mode (client or server).
        /// </summary>
        bool IsMultiplayer { get; }

        /// <summary>
        /// Gets whether the game is running as a dedicated server.
        /// </summary>
        bool IsDedicatedServer { get; }
    }
}
