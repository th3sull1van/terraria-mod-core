using System;
using System.Collections.Generic;

namespace AutoResearch
{
    /// <summary>
    /// Configuration settings for the AutoResearch mod.
    /// Controls sacrifice triggers, background scanning, sounds, and item exclusions.
    /// </summary>
    public class AutoResearchConfig
    {
        /// <summary>
        /// Gets or sets whether the AutoResearch mod is active. Default is true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the inventory scanning interval in game ticks. Default is 1 (every tick / instant).
        /// </summary>
        public int ScanIntervalTicks { get; set; } = 1;

        /// <summary>
        /// Gets or sets whether items inside the player's Void Bag (Void Vault) should also be auto-researched.
        /// Default is true.
        /// </summary>
        public bool IncludeVoidBag { get; set; } = true;

        /// <summary>
        /// Gets or sets whether native vanilla research sound effects should play upon sacrifice or completion.
        /// Default is true.
        /// </summary>
        public bool PlaySound { get; set; } = true;

        /// <summary>
        /// Gets or sets whether in-game chat notifications should be displayed when researching items.
        /// Default is true.
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// Gets or sets the list of Item IDs that should never be automatically researched.
        /// </summary>
        public List<int> ExcludedItemIds { get; set; } = new List<int>();
    }
}
