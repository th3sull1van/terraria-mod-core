using System;
using System.Collections.Generic;

namespace AutoOpen
{
    /// <summary>
    /// Configuration settings for the AutoOpen plugin.
    /// </summary>
    public class AutoOpenConfig
    {
        /// <summary>
        /// Enables or disables the AutoOpen plugin.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Enables continuous rapid opening when holding Right-Click on openable items.
        /// </summary>
        public bool RapidRightClickOpen { get; set; } = true;

        /// <summary>
        /// Delay in game ticks (60 ticks = 1 second) between opens while holding right click.
        /// Defaults to 3 ticks (~20 openings per second). Clamped between 0 and 30.
        /// </summary>
        public int OpenDelayTicks { get; set; } = 3;

        /// <summary>
        /// Number of containers opened per cycle.
        /// Defaults to 1. Clamped between 1 and 50.
        /// </summary>
        public int BatchSize { get; set; } = 1;

        /// <summary>
        /// Whether to play the standard container opening sound effect.
        /// </summary>
        public bool PlaySound { get; set; } = true;

        /// <summary>
        /// Fully automatic hands-free mode that scans and opens crates/bags directly from inventory.
        /// Defaults to false (player right-click hold is default).
        /// </summary>
        public bool AutoOpenInventory { get; set; } = false;

        /// <summary>
        /// Interval in game ticks between background inventory scans when AutoOpenInventory is enabled.
        /// Defaults to 10 ticks (6 scans per second). Clamped between 1 and 60.
        /// </summary>
        public int AutoOpenIntervalTicks { get; set; } = 10;

        /// <summary>
        /// Whether to also scan and open items stored inside the player's Void Bag / Void Vault.
        /// </summary>
        public bool IncludeVoidBag { get; set; } = true;

        /// <summary>
        /// List of Item IDs that should never be automatically opened.
        /// </summary>
        public List<int> ExcludedItemIds { get; set; } = new List<int>();
    }
}
