using System;
using System.Collections.Generic;

namespace BossCursor
{
    /// <summary>
    /// Configuration settings for the BossCursor plugin.
    /// </summary>
    public class BossCursorConfig
    {
        /// <summary>
        /// Enables or disables the BossCursor plugin.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Hide the cursor for bosses that are currently visible within the screen camera viewport.
        /// Defaults to false.
        /// </summary>
        public bool HideOnScreen { get; set; } = false;

        /// <summary>
        /// The radial distance (in pixels) from the player center where the cursor is rendered.
        /// Defaults to 150 (range: 0 to 500).
        /// </summary>
        public int CursorDistance { get; set; } = 150;

        /// <summary>
        /// The scale multiplier applied to the cursor arrow and boss head icon.
        /// Defaults to 1.0 (range: 0.1 to 2.0).
        /// </summary>
        public float CursorSize { get; set; } = 1.0f;

        /// <summary>
        /// The radial offset distance (in pixels) between the cursor arrow and the boss head icon.
        /// Defaults to 45 (range: 10 to 200).
        /// </summary>
        public float HeadOffset { get; set; } = 45f;

        /// <summary>
        /// Whether to blacklist Celestial / Lunar Towers (Solar, Nebula, Vortex, Stardust).
        /// Defaults to true.
        /// </summary>
        public bool BlacklistPillars { get; set; } = true;

        /// <summary>
        /// The keyboard key used to toggle Boss Cursor on/off in-game.
        /// Defaults to "B". Set to "None" or empty to disable keybinding.
        /// </summary>
        public string ToggleKey { get; set; } = "B";

        /// <summary>
        /// Custom list of NPC IDs that should never have a cursor drawn (Blacklist).
        /// </summary>
        public List<int> ExcludedNpcIds { get; set; } = new List<int>();

        /// <summary>
        /// Custom list of NPC IDs that should always have a cursor drawn even if not a standard boss (Whitelist).
        /// </summary>
        public List<int> IncludedNpcIds { get; set; } = new List<int>();
    }
}
