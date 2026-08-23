using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AutoBuff
{
    /// <summary>
    /// Controller responsible for scanning player inventory and auto-consuming buff potions and food.
    /// </summary>
    public static class BuffController
    {
        private static int _tickCounter;

        /// <summary>
        /// Resets the internal tick counter.
        /// </summary>
        public static void Reset()
        {
            _tickCounter = 0;
        }

        /// <summary>
        /// Executes the main evaluation tick for the local player.
        /// </summary>
        /// <param name="player">The player instance being updated.</param>
        /// <param name="config">The active AutoBuff configuration.</param>
        public static void Update(Player player, AutoBuffConfig config)
        {
            if (player == null || config == null || !config.Enabled)
            {
                return;
            }

            // Verify valid in-game state
            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld)
            {
                return;
            }

            if (player.dead || player.ghost || player.CCed || player.cursed || player.spectating >= 0)
            {
                return;
            }

            // Check if player has pending inventory actions
            try
            {
                if (Main.LocalPlayerHasPendingInventoryActions())
                {
                    return;
                }
            }
            catch
            {
                // Fallback for test or headless environments
            }

            // Throttle execution by CheckIntervalTicks
            int interval = Math.Max(1, config.CheckIntervalTicks);
            _tickCounter++;
            if (_tickCounter < interval)
            {
                return;
            }
            _tickCounter = 0;

            // Cannot apply buffs if at maximum buff capacity
            if (player.CountBuffs() >= Player.maxBuffs)
            {
                return;
            }

            // 1. Process Food / Well-Fed Buffs
            if (config.IncludeFood && !HasActiveFoodBuff(player, config.MinBuffTimeThresholdTicks))
            {
                TryConsumeBestFood(player, config);
            }

            // 2. Process Regular Buff Potions & Flasks
            ProcessBuffPotions(player, config);
        }

        /// <summary>
        /// Checks if the player currently has an active food buff above the threshold.
        /// </summary>
        public static bool HasActiveFoodBuff(Player player, int thresholdTicks = 0)
        {
            if (player == null || player.buffType == null)
            {
                return false;
            }

            for (int i = 0; i < Player.maxBuffs; i++)
            {
                if (i >= player.buffType.Length) break;

                int buff = player.buffType[i];
                if (buff <= 0) continue;

                if (IsFoodBuff(buff))
                {
                    int timeLeft = (player.buffTime != null && i < player.buffTime.Length) ? player.buffTime[i] : 0;
                    if (timeLeft > thresholdTicks)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a buff ID represents a Well Fed / Food state.
        /// </summary>
        public static bool IsFoodBuff(int buffType)
        {
            if (buffType <= 0) return false;

            if (buffType == BuffID.WellFed || buffType == BuffID.WellFed2 || buffType == BuffID.WellFed3)
            {
                return true;
            }

            try
            {
                if (BuffID.Sets.IsWellFed != null && buffType < BuffID.Sets.IsWellFed.Length && BuffID.Sets.IsWellFed[buffType])
                {
                    return true;
                }
                if (BuffID.Sets.IsFedState != null && buffType < BuffID.Sets.IsFedState.Length && BuffID.Sets.IsFedState[buffType])
                {
                    return true;
                }
            }
            catch
            {
                // Fallback for test harnesses
            }

            return false;
        }

        /// <summary>
        /// Finds and consumes the best food item available in inventory or Void Bag.
        /// </summary>
        public static bool TryConsumeBestFood(Player player, AutoBuffConfig config)
        {
            if (player == null) return false;

            Item foodItem = PickBestFoodItem(player, config);
            if (foodItem == null || foodItem.IsAir || foodItem.stack <= 0 || foodItem.buffType <= 0)
            {
                return false;
            }

            if (config != null)
            {
                if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(foodItem.type)) return false;
                if (config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(foodItem.buffType)) return false;
            }

            return ConsumeItemForBuff(player, foodItem, foodItem.buffType);
        }

        /// <summary>
        /// Selects the highest priority food item from player inventory, Void Bag, and Piggy Bank.
        /// </summary>
        public static Item PickBestFoodItem(Player player, AutoBuffConfig config)
        {
            if (player?.inventory == null) return null;

            Item best = null;
            int bestPriority = 0;

            // 1. Scan main inventory (slots 0..57)
            for (int i = 0; i < 58; i++)
            {
                if (i >= player.inventory.Length) break;
                Item item = player.inventory[i];
                if (item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0) continue;

                if (config != null)
                {
                    if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(item.type)) continue;
                    if (config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(item.buffType)) continue;
                }

                if (!IsFoodBuff(item.buffType)) continue;

                int priority = item.buffType == BuffID.WellFed3 ? 3 : (item.buffType == BuffID.WellFed2 ? 2 : 1);
                if (priority > bestPriority)
                {
                    best = item;
                    bestPriority = priority;
                }
            }

            // 2. Scan Void Bag if enabled and accessible
            if (config != null && config.IncludeVoidBag)
            {
                try
                {
                    if (player.useVoidBag() && player.bank4?.item != null)
                    {
                        for (int i = 0; i < player.bank4.item.Length; i++)
                        {
                            Item item = player.bank4.item[i];
                            if (item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0) continue;

                            if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(item.type)) continue;
                            if (config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(item.buffType)) continue;

                            if (!IsFoodBuff(item.buffType)) continue;

                            int priority = item.buffType == BuffID.WellFed3 ? 3 : (item.buffType == BuffID.WellFed2 ? 2 : 1);
                            if (priority > bestPriority)
                            {
                                best = item;
                                bestPriority = priority;
                            }
                        }
                    }
                }
                catch
                {
                    // Fallback for tests
                }
            }

            // 3. Scan Piggy Bank if enabled and accessible
            if (config != null && config.IncludePiggyBank)
            {
                try
                {
                    if (CanAccessPiggyBank(player) && player.bank?.item != null)
                    {
                        for (int i = 0; i < player.bank.item.Length; i++)
                        {
                            Item item = player.bank.item[i];
                            if (item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0) continue;

                            if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(item.type)) continue;
                            if (config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(item.buffType)) continue;

                            if (!IsFoodBuff(item.buffType)) continue;

                            int priority = item.buffType == BuffID.WellFed3 ? 3 : (item.buffType == BuffID.WellFed2 ? 2 : 1);
                            if (priority > bestPriority)
                            {
                                best = item;
                                bestPriority = priority;
                            }
                        }
                    }
                }
                catch
                {
                    // Fallback for tests
                }
            }

            return best;
        }

        /// <summary>
        /// Iterates through player inventory, Void Bag, and Piggy Bank to evaluate and consume eligible buff potions.
        /// </summary>
        public static void ProcessBuffPotions(Player player, AutoBuffConfig config)
        {
            if (player?.inventory == null) return;

            // 1. Scan main inventory (slots 0..57)
            for (int i = 0; i < 58; i++)
            {
                if (i >= player.inventory.Length) break;
                if (player.CountBuffs() >= Player.maxBuffs) return;

                Item item = player.inventory[i];
                TryProcessItem(player, item, config);
            }

            // 2. Scan Void Bag if enabled and accessible
            if (config != null && config.IncludeVoidBag)
            {
                try
                {
                    if (player.useVoidBag() && player.bank4?.item != null)
                    {
                        for (int i = 0; i < player.bank4.item.Length; i++)
                        {
                            if (player.CountBuffs() >= Player.maxBuffs) return;

                            Item item = player.bank4.item[i];
                            TryProcessItem(player, item, config);
                        }
                    }
                }
                catch
                {
                    // Test harness fallback
                }
            }

            // 3. Scan Piggy Bank if enabled and accessible
            if (config != null && config.IncludePiggyBank)
            {
                try
                {
                    if (CanAccessPiggyBank(player) && player.bank?.item != null)
                    {
                        for (int i = 0; i < player.bank.item.Length; i++)
                        {
                            if (player.CountBuffs() >= Player.maxBuffs) return;

                            Item item = player.bank.item[i];
                            TryProcessItem(player, item, config);
                        }
                    }
                }
                catch
                {
                    // Test harness fallback
                }
            }
        }

        /// <summary>
        /// Determines if the player has access to their Piggy Bank (open container or carrying Piggy Bank, Money Trough, or Chester).
        /// </summary>
        public static bool CanAccessPiggyBank(Player player)
        {
            if (player?.bank?.item == null) return false;

            // 1. Currently opened container is Piggy Bank
            if (player.chest == -2) return true;

            // 2. Carried in inventory (slots 0..58 including mouse item)
            if (player.inventory != null)
            {
                for (int i = 0; i < 59; i++)
                {
                    if (i >= player.inventory.Length) break;
                    Item item = player.inventory[i];
                    if (item == null || item.IsAir || item.stack <= 0) continue;

                    if (item.type == ItemID.PiggyBank || item.type == ItemID.MoneyTrough || item.type == ItemID.ChesterPetItem)
                    {
                        return true;
                    }
                }
            }

            // 3. Equipped in pet / vanity accessory slots (Eyebone / Chester)
            if (player.miscEquips != null)
            {
                for (int i = 0; i < player.miscEquips.Length; i++)
                {
                    Item item = player.miscEquips[i];
                    if (item != null && !item.IsAir && item.stack > 0 && item.type == ItemID.ChesterPetItem)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates a single item to determine if its buff should be applied.
        /// </summary>
        public static bool TryProcessItem(Player player, Item item, AutoBuffConfig config)
        {
            if (player == null || item == null || item.IsAir || item.stack <= 0 || item.buffType <= 0)
            {
                return false;
            }

            // Summon items are handled manually by the player
            if (item.summon)
            {
                return false;
            }

            // Food is processed separately by TryConsumeBestFood
            if (IsFoodBuff(item.buffType))
            {
                return false;
            }

            // Check configuration blacklists
            if (config != null)
            {
                if (config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(item.type))
                {
                    return false;
                }

                if (config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(item.buffType))
                {
                    return false;
                }
            }

            // Check flask settings
            bool isFlask = IsFlaskBuff(item.buffType);
            if (isFlask && (config == null || !config.IncludeFlasks))
            {
                return false;
            }

            // Check if player already has active melee buff / flask
            int threshold = config != null ? config.MinBuffTimeThresholdTicks : 0;
            if (isFlask && HasActiveFlaskBuff(player, threshold))
            {
                return false;
            }

            // Check if the specific buff is already active with remaining time above threshold
            int buffIdx = player.FindBuffIndex(item.buffType);
            if (buffIdx >= 0)
            {
                int timeLeft = (player.buffTime != null && buffIdx < player.buffTime.Length) ? player.buffTime[buffIdx] : 0;
                if (timeLeft > threshold)
                {
                    return false;
                }
            }

            // Check light / vanity pets
            if (IsPetBuff(item.buffType) && HasActivePet(player, item.buffType))
            {
                return false;
            }

            // Check mana cost if applicable
            if (item.mana > 0)
            {
                int manaRequired = (int)(item.mana * player.manaCost);
                if (player.statMana < manaRequired)
                {
                    return false;
                }
                player.ApplyManaRegenerationDelay();
                player.statMana -= manaRequired;
            }

            return ConsumeItemForBuff(player, item, item.buffType);
        }

        /// <summary>
        /// Consumes a single stack of the item and applies the buff to the player.
        /// </summary>
        public static bool ConsumeItemForBuff(Player player, Item item, int buffType)
        {
            if (player == null || item == null || buffType <= 0)
            {
                return false;
            }

            int duration = item.buffTime > 0 ? item.buffTime : 3600;
            player.AddBuff(buffType, duration, false);

            // Play sound effect
            try
            {
                if (item.UseSound != null)
                {
                    Terraria.Audio.SoundEngine.PlaySound(item.UseSound, player.position);
                }
            }
            catch
            {
                // Headless/test fallback
            }

            // Decrement item stack
            if (item.consumable)
            {
                item.stack--;
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a buff type is a weapon flask / melee imbue.
        /// </summary>
        public static bool IsFlaskBuff(int buffType)
        {
            if (buffType <= 0) return false;

            try
            {
                if (Main.meleeBuff != null && buffType < Main.meleeBuff.Length && Main.meleeBuff[buffType])
                {
                    return true;
                }
                if (BuffID.Sets.IsAFlaskBuff != null && buffType < BuffID.Sets.IsAFlaskBuff.Length && BuffID.Sets.IsAFlaskBuff[buffType])
                {
                    return true;
                }
            }
            catch
            {
                // Fallback for test harnesses
            }

            return false;
        }

        private static bool HasActiveFlaskBuff(Player player, int thresholdTicks)
        {
            if (player?.buffType == null) return false;

            for (int i = 0; i < Player.maxBuffs; i++)
            {
                if (i >= player.buffType.Length) break;
                int buff = player.buffType[i];
                if (buff <= 0) continue;

                if (IsFlaskBuff(buff))
                {
                    int timeLeft = (player.buffTime != null && i < player.buffTime.Length) ? player.buffTime[i] : 0;
                    if (timeLeft > thresholdTicks)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsPetBuff(int buffType)
        {
            if (buffType <= 0) return false;

            try
            {
                if (Main.lightPet != null && buffType < Main.lightPet.Length && Main.lightPet[buffType]) return true;
                if (Main.vanityPet != null && buffType < Main.vanityPet.Length && Main.vanityPet[buffType]) return true;
            }
            catch { }

            return false;
        }

        private static bool HasActivePet(Player player, int buffType)
        {
            if (player?.buffType == null) return false;

            bool isLight = false;
            bool isVanity = false;
            try
            {
                isLight = Main.lightPet != null && buffType < Main.lightPet.Length && Main.lightPet[buffType];
                isVanity = Main.vanityPet != null && buffType < Main.vanityPet.Length && Main.vanityPet[buffType];
            }
            catch { }

            for (int i = 0; i < Player.maxBuffs; i++)
            {
                if (i >= player.buffType.Length) break;
                int buff = player.buffType[i];
                if (buff <= 0) continue;

                try
                {
                    if (isLight && Main.lightPet != null && buff < Main.lightPet.Length && Main.lightPet[buff]) return true;
                    if (isVanity && Main.vanityPet != null && buff < Main.vanityPet.Length && Main.vanityPet[buff]) return true;
                }
                catch { }
            }

            return false;
        }
    }
}
