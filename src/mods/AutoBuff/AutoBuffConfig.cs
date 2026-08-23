using System;
using System.Collections.Generic;

namespace AutoBuff
{
    /// <summary>
    /// Configuration settings for the AutoBuff plugin.
    /// </summary>
    public class AutoBuffConfig
    {
        /// <summary>
        /// Enables or disables the AutoBuff plugin.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Frequency in game ticks (60 ticks = 1 second) to evaluate inventory and active buffs.
        /// Defaults to 15 ticks (4 times per second). Clamped between 1 and 300.
        /// </summary>
        public int CheckIntervalTicks { get; set; } = 15;

        /// <summary>
        /// Whether to automatically consume food items when Well Fed, Plenty Satisfied, or Exquisitely Stuffed expires.
        /// </summary>
        public bool IncludeFood { get; set; } = true;

        /// <summary>
        /// Whether to automatically consume weapon flasks/imbues when melee buff expires.
        /// </summary>
        public bool IncludeFlasks { get; set; } = true;

        /// <summary>
        /// Whether to scan the player's Void Bag / Void Vault (bank4) if unlocked and open.
        /// </summary>
        public bool IncludeVoidBag { get; set; } = true;

        /// <summary>
        /// Whether to scan the player's Piggy Bank (bank) if accessible or carried.
        /// </summary>
        public bool IncludePiggyBank { get; set; } = true;

        /// <summary>
        /// Minimum remaining buff duration (in ticks) below which the potion should be re-applied.
        /// 0 means re-apply only when the buff is completely expired. Clamped between 0 and 3600.
        /// </summary>
        public int MinBuffTimeThresholdTicks { get; set; } = 0;

        /// <summary>
        /// List of Buff IDs that should never be automatically consumed or re-applied.
        /// </summary>
        public List<int> ExcludedBuffIds { get; set; } = new List<int>
        {
            18,  // Gravitation (prevent unexpected disorientation)
            119, // Lovestruck (Love Potion)
            120  // Stinky (Stink Potion)
        };

        /// <summary>
        /// List of Item IDs that should never be automatically consumed.
        /// </summary>
        public List<int> ExcludedItemIds { get; set; } = new List<int>
        {
            1344, // Red Potion (hazardous debuffs in non-FTW worlds)
            2756  // Gender Change Potion
        };
    }
}
