using System;
using System.Reflection;
using HarmonyLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace TurboExtractinator.Patches
{
    /// <summary>
    /// Harmony patch targeting Player.PlaceThing_ItemInExtractinator to accelerate
    /// Extractinator processing speed by a configurable multiplier (default 5x).
    /// Strictly guards against non-extractinator interactions (weapons, throwing knives, tools).
    /// </summary>
    [HarmonyPatch(typeof(Player), "PlaceThing_ItemInExtractinator")]
    public static class ExtractinatorSpeedPatch
    {
        [ThreadStatic]
        private static bool _isProcessingBatch;

        private static readonly MethodInfo ExtractinatorUseMethod = typeof(Player).GetMethod(
            "ExtractinatorUse",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        );

        private static readonly MethodInfo TryGettingItemTraderMethod = typeof(Player).GetMethod(
            "TryGettingItemTraderFromBlock",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
        );

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (_isProcessingBatch) return;

            var mod = TurboExtractinatorMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;

            // Strict guard: verify player is actively targeting an Extractinator with a valid extractable item
            if (!IsValidExtractinatorInteraction(__instance, out Item heldItem, out Tile tile, out int tileType, out int extractMode))
            {
                return;
            }

            if (tileType == TileID.ChlorophyteExtractinator && !mod.Config.AffectsChlorophyteExtractinator)
            {
                return;
            }

            int speed = Math.Max(1, Math.Min(60, mod.Config.SpeedMultiplier));

            // Reduce item cooldown delay according to speed multiplier while preserving itemTime == itemTimeMax equality
            if (__instance.itemTime > 0)
            {
                int newTime = Math.Max(1, __instance.itemTime / speed);
                __instance.SetItemTime(newTime);
            }
            if (__instance.itemAnimation > 0)
            {
                int newAnim = Math.Max(1, __instance.itemAnimation / speed);
                __instance.itemAnimation = newAnim;
                __instance.itemAnimationMax = Math.Max(1, __instance.itemAnimationMax / speed);
            }

            // Optional extra batch processing per tick if configured
            int batchSize = Math.Max(1, Math.Min(50, mod.Config.BatchExtractionSize));
            if (batchSize > 1 && ExtractinatorUseMethod != null && extractMode >= 0)
            {
                try
                {
                    _isProcessingBatch = true;
                    int extraExtracts = batchSize - 1;

                    for (int b = 0; b < extraExtracts; b++)
                    {
                        if (heldItem.stack <= 0 || heldItem.type <= ItemID.None) break;

                        int currentExtractMode = ItemID.Sets.ExtractinatorMode[heldItem.type];
                        if (currentExtractMode >= 0)
                        {
                            heldItem.stack--;
                            if (heldItem.stack <= 0)
                            {
                                heldItem.TurnToAir();
                            }
                            try
                            {
                                ExtractinatorUseMethod.Invoke(__instance, new object[] { currentExtractMode, tileType });
                            }
                            catch { }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    _isProcessingBatch = false;
                }
            }
        }

        /// <summary>
        /// Validates that the player is actively interacting with an Extractinator block
        /// using an extractable material or valid item trader trade.
        /// </summary>
        public static bool IsValidExtractinatorInteraction(Player player, out Item heldItem, out Tile tile, out int tileType, out int extractMode)
        {
            heldItem = null;
            tile = null;
            tileType = 0;
            extractMode = -1;

            if (player == null || player.whoAmI != Main.myPlayer || player.dead)
                return false;

            if (player.inventory == null || player.selectedItem < 0 || player.selectedItem >= player.inventory.Length)
                return false;

            heldItem = player.inventory[player.selectedItem];
            if (heldItem == null || heldItem.stack <= 0 || heldItem.type <= ItemID.None)
                return false;

            if (Main.tile == null)
                return false;

            int targetX = Player.tileTargetX;
            int targetY = Player.tileTargetY;

            if (targetX < 0 || targetX >= Main.maxTilesX || targetY < 0 || targetY >= Main.maxTilesY)
                return false;

            tile = Main.tile[targetX, targetY];
            if (tile == null || !tile.active())
                return false;

            tileType = tile.type;
            if (tileType != TileID.Extractinator && tileType != TileID.ChlorophyteExtractinator)
                return false;

            if (!player.IsInTileInteractionRange(targetX, targetY, TileReachCheckSettings.Simple, heldItem.tileBoost + player.blockRange))
                return false;

            if (heldItem.type < ItemID.Sets.ExtractinatorMode.Length)
            {
                extractMode = ItemID.Sets.ExtractinatorMode[heldItem.type];
            }

            if (extractMode >= 0)
                return true;

            // Support Chlorophyte Extractinator ItemTrader recipes
            if (tileType == TileID.ChlorophyteExtractinator && TryGettingItemTraderMethod != null)
            {
                try
                {
                    var trader = TryGettingItemTraderMethod.Invoke(null, new object[] { tile }) as Terraria.GameContent.ItemTrader;
                    if (trader != null && trader.TryGetTradeOption(heldItem, out _))
                    {
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
