using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AutoOpen
{
    /// <summary>
    /// Core controller for accelerating and automating container and grab bag openings.
    /// </summary>
    public static class OpenController
    {
        private static int _holdCooldownTicks;
        private static int _autoOpenTickCounter;
        private static int _lastRightClickedSlot = -1;

        /// <summary>
        /// Resets internal tick counters and state.
        /// </summary>
        public static void Reset()
        {
            _holdCooldownTicks = 0;
            _autoOpenTickCounter = 0;
            _lastRightClickedSlot = -1;
        }

        /// <summary>
        /// Checks whether a given item type is an openable container / grab bag.
        /// </summary>
        public static bool IsOpenable(int itemType, AutoOpenConfig config)
        {
            if (itemType <= 0) return false;

            if (config?.ExcludedItemIds != null && config.ExcludedItemIds.Contains(itemType))
            {
                return false;
            }

            try
            {
                if (ItemID.Sets.OpenableBag != null && itemType < ItemID.Sets.OpenableBag.Length && ItemID.Sets.OpenableBag[itemType])
                {
                    return true;
                }
                if (ItemID.Sets.BossBag != null && itemType < ItemID.Sets.BossBag.Length && ItemID.Sets.BossBag[itemType])
                {
                    return true;
                }
                if (ItemID.Sets.IsFishingCrate != null && itemType < ItemID.Sets.IsFishingCrate.Length && ItemID.Sets.IsFishingCrate[itemType])
                {
                    return true;
                }
            }
            catch
            {
                // Fallback for test harnesses
            }

            switch (itemType)
            {
                case ItemID.HerbBag:          // 3093
                case ItemID.CanOfWorms:        // 4345
                case ItemID.Oyster:            // 4410
                case ItemID.GoodieBag:         // 1774
                case 6142:                     // Chillet Egg
                case ItemID.LockBox:           // 3085
                case ItemID.ObsidianLockbox:   // 4879
                case ItemID.Present:           // 1869
                case 599:
                case 600:
                case 601:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Executes item opening when the player right-clicks or holds right-click on an openable container slot.
        /// </summary>
        public static void ProcessRightClick(Item[] inv, int context, int slot, Player player, AutoOpenConfig config)
        {
            if (inv == null || slot < 0 || slot >= inv.Length || player == null || config == null)
            {
                return;
            }

            Item item = inv[slot];
            if (item == null || item.IsAir || item.stack <= 0)
            {
                return;
            }

            // If switching slots, reset cooldown immediately
            if (_lastRightClickedSlot != slot)
            {
                _lastRightClickedSlot = slot;
                _holdCooldownTicks = 0;
            }

            if (_holdCooldownTicks > 0)
            {
                _holdCooldownTicks--;
                return;
            }

            int batchSize = Math.Max(1, config.BatchSize);
            int opened = 0;

            for (int i = 0; i < batchSize; i++)
            {
                if (item.stack <= 0 || item.IsAir) break;

                bool success = OpenSingle(item, player, config.PlaySound && opened == 0);
                if (!success)
                {
                    // Failed (e.g. out of keys for Lock Box)
                    break;
                }
                opened++;
            }

            if (opened > 0)
            {
                _holdCooldownTicks = Math.Max(0, config.OpenDelayTicks);
                Main.stackSplit = config.OpenDelayTicks;
                Main.mouseRightRelease = false;
            }
        }

        /// <summary>
        /// Background auto-open logic when AutoOpenInventory is enabled.
        /// </summary>
        public static void UpdateInventoryAutoOpen(Player player, AutoOpenConfig config)
        {
            if (player == null || config == null || !config.Enabled || !config.AutoOpenInventory)
            {
                return;
            }

            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld)
            {
                return;
            }

            if (player.dead || player.ghost || player.CCed || player.cursed || player.spectating >= 0)
            {
                return;
            }

            int interval = Math.Max(1, config.AutoOpenIntervalTicks);
            _autoOpenTickCounter++;
            if (_autoOpenTickCounter < interval)
            {
                return;
            }
            _autoOpenTickCounter = 0;

            if (player.inventory == null) return;

            // Scan main inventory (slots 0..57)
            for (int i = 0; i < 58; i++)
            {
                if (i >= player.inventory.Length) break;
                Item item = player.inventory[i];
                if (item == null || item.IsAir || item.stack <= 0) continue;

                if (IsOpenable(item.type, config))
                {
                    int batchSize = Math.Max(1, config.BatchSize);
                    for (int b = 0; b < batchSize; b++)
                    {
                        if (item.stack <= 0 || item.IsAir) break;
                        if (!OpenSingle(item, player, config.PlaySound && b == 0)) break;
                    }
                    return;
                }
            }

            // Scan Void Bag if enabled
            if (config.IncludeVoidBag)
            {
                try
                {
                    if (player.useVoidBag() && player.bank4?.item != null)
                    {
                        for (int i = 0; i < player.bank4.item.Length; i++)
                        {
                            Item item = player.bank4.item[i];
                            if (item == null || item.IsAir || item.stack <= 0) continue;

                            if (IsOpenable(item.type, config))
                            {
                                int batchSize = Math.Max(1, config.BatchSize);
                                for (int b = 0; b < batchSize; b++)
                                {
                                    if (item.stack <= 0 || item.IsAir) break;
                                    if (!OpenSingle(item, player, config.PlaySound && b == 0)) break;
                                }
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // Test fallback
                }
            }
        }

        /// <summary>
        /// Opens a single instance of a grab bag / container item.
        /// </summary>
        public static bool OpenSingle(Item item, Player player, bool playSound)
        {
            if (item == null || item.IsAir || item.stack <= 0 || player == null)
            {
                return false;
            }

            try
            {
                try
                {
                    if (Player.GetItemLogger != null)
                    {
                        Player.GetItemLogger.Start();
                    }
                }
                catch { }

                bool granted = GrantContainerItems(item, player);

                try
                {
                    if (Player.GetItemLogger != null)
                    {
                        Player.GetItemLogger.Stop();
                    }
                }
                catch { }

                if (!granted)
                {
                    return false;
                }

                item.stack--;
                if (item.stack <= 0)
                {
                    item.SetDefaults(0);
                }

                if (playSound)
                {
                    try
                    {
                        Terraria.Audio.SoundEngine.PlaySound(7, player.position);
                    }
                    catch
                    {
                        // Fallback for test/headless environments
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Dispatches item granting to the corresponding vanilla Player.Open* method.
        /// </summary>
        public static bool GrantContainerItems(Item item, Player player)
        {
            if (item == null || player == null) return false;

            try
            {
                if (ItemID.Sets.BossBag != null && item.type < ItemID.Sets.BossBag.Length && ItemID.Sets.BossBag[item.type])
                {
                    player.OpenBossBag(item.type);
                    return true;
                }
            }
            catch { }

            try
            {
                if (ItemID.Sets.IsFishingCrate != null && item.type < ItemID.Sets.IsFishingCrate.Length && ItemID.Sets.IsFishingCrate[item.type])
                {
                    player.OpenFishingCrate(item.type);
                    return true;
                }
            }
            catch { }

            switch (item.type)
            {
                case ItemID.HerbBag:          // 3093
                    player.OpenHerbBag(item.type);
                    return true;
                case ItemID.CanOfWorms:        // 4345
                    player.OpenCanofWorms(item.type);
                    return true;
                case ItemID.Oyster:            // 4410
                    player.OpenOyster(item.type);
                    return true;
                case ItemID.GoodieBag:         // 1774
                    player.OpenGoodieBag(item.type);
                    return true;
                case 6142:                     // Chillet Egg
                    player.OpenChilletEgg(item.type);
                    return true;
                case ItemID.LockBox:           // 3085
                    if (player.ConsumeItem(ItemID.GoldenKey, false, true))
                    {
                        player.OpenLockBox(item.type);
                        return true;
                    }
                    return false;
                case ItemID.ObsidianLockbox:   // 4879
                    if (player.HasItemInInventoryOrOpenVoidBag(ItemID.ShadowKey))
                    {
                        player.OpenShadowLockbox(item.type);
                        return true;
                    }
                    return false;
                case ItemID.Present:           // 1869
                    player.OpenPresent(item.type);
                    return true;
                case 599:
                case 600:
                case 601:
                    player.OpenLegacyPresent(item.type);
                    return true;
                default:
                    // Generic openable bag fallback
                    try
                    {
                        if (ItemID.Sets.OpenableBag != null && item.type < ItemID.Sets.OpenableBag.Length && ItemID.Sets.OpenableBag[item.type])
                        {
                            player.OpenFishingCrate(item.type);
                            return true;
                        }
                    }
                    catch { }
                    return false;
            }
        }
    }
}
