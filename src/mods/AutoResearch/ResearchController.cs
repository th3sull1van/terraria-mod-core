using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;

namespace AutoResearch
{
    /// <summary>
    /// Core engine controller for automated Journey Mode item research.
    /// Handles deficit calculation, stack consumption, unlocks, network syncing, and notifications.
    /// </summary>
    public static class ResearchController
    {
        private static int scanTimer = 0;

        /// <summary>
        /// Resets controller state.
        /// </summary>
        public static void Reset()
        {
            scanTimer = 0;
        }

        /// <summary>
        /// Determines whether the specified player is in Journey / Creative Mode.
        /// </summary>
        public static bool IsJourneyMode(Player player)
        {
            return player != null && player.difficulty == 3; // PlayerDifficultyID.Creative == 3
        }

        /// <summary>
        /// Attempts to sacrifice/research the provided item up to the vanilla requirement cap.
        /// </summary>
        /// <param name="item">The item to sacrifice.</param>
        /// <param name="player">The owner player.</param>
        /// <param name="config">Mod configuration.</param>
        /// <param name="sacrificed">The amount consumed for research.</param>
        /// <param name="isComplete">True if the item reached 100% research completion.</param>
        /// <returns>True if at least one item was sacrificed; otherwise false.</returns>
        public static bool TrySacrificeItem(Item item, Player player, AutoResearchConfig config, out int sacrificed, out bool isComplete)
        {
            sacrificed = 0;
            isComplete = false;

            if (config == null || !config.Enabled)
            {
                return false;
            }

            if (!IsJourneyMode(player))
            {
                return false;
            }

            if (item == null || item.IsAir || item.type <= 0 || item.stack <= 0)
            {
                return false;
            }

            if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(item.type))
            {
                return false;
            }

            var tracker = Main.LocalPlayerCreativeTracker?.ItemSacrifices;
            if (tracker == null)
            {
                return false;
            }

            int amountWeHave = 0;
            int amountNeededTotal = 0;
            if (!tracker.TryGetSacrificeNumbers(item.type, out amountWeHave, out amountNeededTotal) || amountNeededTotal <= 0)
            {
                return false;
            }

            int remainingNeeded = Utils.Clamp(amountNeededTotal - amountWeHave, 0, amountNeededTotal);
            if (remainingNeeded <= 0)
            {
                return false; // Already fully researched
            }

            int toSacrifice = Math.Min(item.stack, remainingNeeded);
            if (toSacrifice <= 0)
            {
                return false;
            }

            sacrificed = toSacrifice;
            int newTotal = amountWeHave + sacrificed;
            isComplete = (newTotal >= amountNeededTotal);

            // 1. Sync multiplayer packet if in a network session
            try
            {
                if (Main.netMode == 1 && NetManager.Instance != null)
                {
                    NetPacket packet = NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(Main.myPlayer, item.type, sacrificed);
                    NetManager.Instance.SendToServer(packet);
                }
            }
            catch { }

            // 2. Direct local progress registration when not in ServerSideCharacter mode
            if (!Main.ServerSideCharacter)
            {
                tracker.RegisterItemSacrifice(item.type, sacrificed, null);
            }

            int originalType = item.type;

            // 3. Decrement item stack and convert to Air if exhausted
            item.stack -= sacrificed;
            if (item.stack <= 0)
            {
                item.TurnToAir();
            }

            // 4. Play audio cue
            if (config.PlaySound && !Main.gameMenu)
            {
                int soundId = isComplete ? SoundID.ResearchComplete : SoundID.Research;
                try
                {
                    SoundEngine.PlaySound(soundId, (int)player.position.X, (int)player.position.Y);
                }
                catch { }
            }

            // 5. Render notification
            if (config.ShowNotifications && !Main.gameMenu)
            {
                NotifyResearch(originalType, sacrificed, newTotal, amountNeededTotal, isComplete);
            }

            return true;
        }

        /// <summary>
        /// Sweeps the player's real inventory and void bag in the background.
        /// </summary>
        public static void UpdateInventoryScan(Player player, AutoResearchConfig config)
        {
            if (config == null || !config.Enabled)
            {
                return;
            }

            if (!IsJourneyMode(player))
            {
                return;
            }

            scanTimer++;
            if (config.ScanIntervalTicks > 1 && scanTimer % config.ScanIntervalTicks != 0)
            {
                return;
            }

            // 1. Scan Main Inventory (0..57: hotbar/backpack 0..49, coins 50..53, ammo 54..57; slot 58 is InventoryMouseItem and is excluded)
            if (player.inventory != null)
            {
                int maxInventorySlot = Math.Min(player.inventory.Length, 58);
                for (int i = 0; i < maxInventorySlot; i++)
                {
                    var item = player.inventory[i];
                    if (item != null && !item.IsAir && item.type > 0 && item.stack > 0)
                    {
                        TrySacrificeItem(item, player, config, out _, out _);
                    }
                }
            }

            // 2. Scan Void Bag / Void Vault if enabled
            if (config.IncludeVoidBag && player.bank4 != null && player.bank4.item != null)
            {
                for (int i = 0; i < player.bank4.item.Length; i++)
                {
                    var item = player.bank4.item[i];
                    if (item != null && !item.IsAir && item.type > 0 && item.stack > 0)
                    {
                        TrySacrificeItem(item, player, config, out _, out _);
                    }
                }
            }
        }

        private static void NotifyResearch(int itemId, int sacrificedAmount, int newTotal, int neededTotal, bool isComplete)
        {
            try
            {
                string itemName = Lang.GetItemNameValue(itemId);
                if (string.IsNullOrEmpty(itemName))
                {
                    itemName = $"Item #{itemId}";
                }

                if (isComplete)
                {
                    Main.NewText($"[AutoResearch] Unlocked {itemName} for Duplication! ({neededTotal}/{neededTotal})", 255, 215, 0); // Gold
                }
                else
                {
                    Main.NewText($"[AutoResearch] Researched {sacrificedAmount}x {itemName} ({newTotal}/{neededTotal})", 100, 220, 255); // Cyan
                }
            }
            catch { }
        }
    }
}
