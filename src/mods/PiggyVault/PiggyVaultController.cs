using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace PiggyVault
{
    /// <summary>
    /// Core controller managing Piggy Bank extended storage, auto-pickup, crafting, quick-actions, and accessories.
    /// </summary>
    public static class PiggyVaultController
    {
        public const int ItemIdPiggyBank = 87;
        public const int ItemIdMoneyTrough = 3213;
        public const int ItemIdEyebone = 5098;
        public const int ItemIdWormholePotion = 2997;

        /// <summary>
        /// Determines if the player has access to the Piggy Bank features based on config and carried items.
        /// </summary>
        public static bool IsPiggyBankUsable(Player player, PiggyVaultConfig config)
        {
            if (player == null || config == null || !config.Enabled)
            {
                return false;
            }

            if (!config.RequirePiggyItemInInventory)
            {
                return true;
            }

            if (player.inventory == null)
            {
                return false;
            }

            // Check main inventory (slots 0..57)
            for (int i = 0; i < 58; i++)
            {
                if (i >= player.inventory.Length) break;
                Item item = player.inventory[i];
                if (item == null || item.IsAir || item.stack <= 0) continue;

                if (item.type == ItemIdPiggyBank || item.type == ItemIdMoneyTrough || item.type == ItemIdEyebone)
                {
                    return true;
                }
            }

            // Check pet / accessory equips for Chester (Eyebone)
            if (player.miscEquips != null)
            {
                for (int i = 0; i < player.miscEquips.Length; i++)
                {
                    Item item = player.miscEquips[i];
                    if (item == null || item.IsAir || item.stack <= 0) continue;

                    if (item.type == ItemIdEyebone)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the Piggy Bank has space to receive the specified item.
        /// </summary>
        public static bool HasSpaceInPiggyBank(Player player, Item item)
        {
            if (player?.bank?.item == null || item == null || item.IsAir || item.stack <= 0)
            {
                return false;
            }

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item slot = player.bank.item[i];
                if (slot == null || slot.IsAir || slot.type == 0 || slot.stack <= 0)
                {
                    return true;
                }

                if (slot.type == item.type && slot.stack < slot.maxStack && slot.prefix == item.prefix)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Places an item into the Piggy Bank, merging into existing stacks and using empty slots.
        /// Returns an empty Item if completely absorbed, or the remaining item with leftover stack.
        /// </summary>
        public static Item PutItemInPiggyBank(Player player, Item item, GetItemSettings settings)
        {
            if (player?.bank?.item == null || item == null || item.IsAir || item.stack <= 0)
            {
                return item;
            }

            var config = PiggyVaultMod.Instance?.Config;
            int initialStack = item.stack;

            // Pass 1: Stack with identical items
            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item slot = player.bank.item[i];
                if (slot == null || slot.IsAir || slot.type != item.type || slot.prefix != item.prefix || slot.stack >= slot.maxStack)
                {
                    continue;
                }

                int space = slot.maxStack - slot.stack;
                int transfer = Math.Min(item.stack, space);

                slot.stack += transfer;
                item.stack -= transfer;

                if (config != null)
                {
                    PlayPickupFeedback(player, slot, transfer, config);
                }

                if (item.stack <= 0)
                {
                    item.TurnToAir();
                    return new Item();
                }
            }

            // Pass 2: Place in empty slot
            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item slot = player.bank.item[i];
                if (slot == null || slot.IsAir || slot.type == 0 || slot.stack <= 0)
                {
                    int transfer = item.stack;
                    player.bank.item[i] = item.Clone();
                    item.TurnToAir();

                    if (config != null)
                    {
                        PlayPickupFeedback(player, player.bank.item[i], transfer, config);
                    }

                    return new Item();
                }
            }

            return item;
        }

        /// <summary>
        /// Plays pickup sound and visual popup text for items absorbed by the Piggy Bank.
        /// </summary>
        public static void PlayPickupFeedback(Player player, Item item, int amount, PiggyVaultConfig config)
        {
            if (player == null || item == null || amount <= 0 || config == null)
            {
                return;
            }

            try
            {
                if (config.PlayPickupSound)
                {
                    SoundEngine.PlaySound(SoundID.Item9, player.position);
                }
            }
            catch
            {
                // Headless/test fallback
            }

            try
            {
                if (config.ShowPickupText)
                {
                    PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, player.Center, amount, false, false);
                }
            }
            catch
            {
                // Headless/test fallback
            }
        }

        /// <summary>
        /// Finds the best healing potion in the Piggy Bank for Quick Heal.
        /// </summary>
        public static Item GetQuickHealItemFromPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return null;

            int missingLife = player.statLifeMax2 - player.statLife;
            Item bestItem = null;
            int bestExcess = -player.statLifeMax2;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.stack <= 0 || item.type <= 0 || item.healLife <= 0)
                {
                    continue;
                }

                if (player.potionDelay > 0 && item.potion)
                {
                    continue;
                }

                int excess = item.healLife - missingLife;
                if (bestItem == null)
                {
                    bestItem = item;
                    bestExcess = excess;
                }
                else if (bestExcess < 0)
                {
                    if (excess > bestExcess)
                    {
                        bestItem = item;
                        bestExcess = excess;
                    }
                }
                else if (excess < bestExcess && excess >= 0)
                {
                    bestItem = item;
                    bestExcess = excess;
                }
            }

            return bestItem;
        }

        /// <summary>
        /// Finds the first eligible mana potion in the Piggy Bank for Quick Mana.
        /// </summary>
        public static Item GetQuickManaItemFromPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return null;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.stack <= 0 || item.type <= 0 || item.healMana <= 0)
                {
                    continue;
                }

                if (player.potionDelay > 0 && item.potion)
                {
                    continue;
                }

                return item;
            }

            return null;
        }

        /// <summary>
        /// Finds the highest priority food item in the Piggy Bank for Quick Buff.
        /// </summary>
        public static Item PickBestFoodItemFromPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return null;

            Item best = null;
            int bestPriority = 0;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0) continue;

                int priority = GetFoodPriority(item.buffType);
                if (priority <= 0) continue;

                if (priority > bestPriority || (priority == bestPriority && best != null && item.buffTime > best.buffTime))
                {
                    best = item;
                    bestPriority = priority;
                }
            }

            return best;
        }

        public static int GetFoodPriority(int buffType)
        {
            if (buffType == BuffID.WellFed3) return 3;
            if (buffType == BuffID.WellFed2) return 2;
            if (buffType == BuffID.WellFed) return 1;
            return 0;
        }

        /// <summary>
        /// Consumes eligible buff potions and food from the Piggy Bank when player uses Quick Buff.
        /// </summary>
        public static void QuickBuffFromPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return;

            // 1. Food evaluation
            Item bestFood = PickBestFoodItemFromPiggyBank(player);
            if (bestFood != null && !bestFood.IsAir && bestFood.buffType > 0)
            {
                int currentFoodPrio = 0;
                for (int i = 0; i < Player.maxBuffs; i++)
                {
                    if (player.buffType != null && i < player.buffType.Length && player.buffTime != null && i < player.buffTime.Length)
                    {
                        if (player.buffType[i] > 0 && player.buffTime[i] > 0)
                        {
                            int prio = GetFoodPriority(player.buffType[i]);
                            if (prio > currentFoodPrio) currentFoodPrio = prio;
                        }
                    }
                }

                int newPrio = GetFoodPriority(bestFood.buffType);
                if (newPrio > currentFoodPrio)
                {
                    ConsumeBuffItem(player, bestFood);
                }
            }

            // 2. Potions evaluation
            for (int i = 0; i < player.bank.item.Length; i++)
            {
                if (player.CountBuffs() >= Player.maxBuffs) return;

                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0) continue;
                if (GetFoodPriority(item.buffType) > 0) continue; // Food is handled above

                if (player.FindBuffIndex(item.buffType) == -1)
                {
                    ConsumeBuffItem(player, item);
                }
            }
        }

        private static void ConsumeBuffItem(Player player, Item item)
        {
            if (player == null || item == null || item.IsAir || item.buffType <= 0) return;

            int duration = item.buffTime > 0 ? item.buffTime : 3600;
            player.AddBuff(item.buffType, duration, false);

            try
            {
                if (item.UseSound != null)
                {
                    SoundEngine.PlaySound(item.UseSound, player.position);
                }
            }
            catch { }

            if (item.consumable)
            {
                item.stack--;
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
            }
        }

        /// <summary>
        /// Consumes 1 item of the specified type from the Piggy Bank (for ammo, wire, actuators, bait).
        /// Returns true if an item was found and consumed.
        /// </summary>
        public static bool ConsumeItemFromPiggyBank(Player player, int type)
        {
            if (player?.bank?.item == null || type <= 0) return false;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.type != type || item.stack <= 0) continue;

                item.stack--;
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the player has a Wormhole Potion in their Piggy Bank.
        /// </summary>
        public static bool HasUnityPotionInPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return false;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item != null && !item.IsAir && item.type == ItemIdWormholePotion && item.stack > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Consumes 1 Wormhole Potion from the Piggy Bank.
        /// </summary>
        public static bool TakeUnityPotionFromPiggyBank(Player player)
        {
            return ConsumeItemFromPiggyBank(player, ItemIdWormholePotion);
        }
    }
}
