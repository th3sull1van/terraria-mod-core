using System;

namespace PiggyVault
{
    /// <summary>
    /// Configuration settings for the PiggyVault plugin.
    /// </summary>
    public class PiggyVaultConfig
    {
        /// <summary>
        /// Master switch enabling or disabling all PiggyVault functionality.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// When true, PiggyVault requires carrying a Piggy Bank item (Piggy Bank, Money Trough, or Eye Bone/Chester) in inventory.
        /// When false, PiggyVault features are always active for the local player.
        /// </summary>
        public bool RequirePiggyItemInInventory { get; set; } = true;

        /// <summary>
        /// Automatically routes picked up items and coins to the Piggy Bank when the player inventory is full.
        /// </summary>
        public bool AutoPickupToPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables crafting recipes to draw materials directly from the Piggy Bank.
        /// </summary>
        public bool CraftFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables Quick Buff (hotkey B) and food consumption from the Piggy Bank.
        /// </summary>
        public bool QuickBuffFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables Quick Heal (hotkey H) to consume healing potions from the Piggy Bank.
        /// </summary>
        public bool QuickHealFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables Quick Mana (hotkey M) to consume mana potions from the Piggy Bank.
        /// </summary>
        public bool QuickManaFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables ammo, wire, actuators, and fishing bait consumption directly from the Piggy Bank.
        /// </summary>
        public bool ConsumeAmmoAndBaitFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables informational accessories (PDA, Cell Phone, Compass, Watch, etc.) inside the Piggy Bank to function.
        /// </summary>
        public bool InfoAccessoriesInPiggyBank { get; set; } = true;

        /// <summary>
        /// Enables Wormhole Potions inside the Piggy Bank to allow teleporting to teammates on the map.
        /// </summary>
        public bool WormholePotionFromPiggyBank { get; set; } = true;

        /// <summary>
        /// Plays a sound effect when items are vacuumed into the Piggy Bank.
        /// </summary>
        public bool PlayPickupSound { get; set; } = true;

        /// <summary>
        /// Displays popup text when items are vacuumed into the Piggy Bank.
        /// </summary>
        public bool ShowPickupText { get; set; } = true;
    }
}
