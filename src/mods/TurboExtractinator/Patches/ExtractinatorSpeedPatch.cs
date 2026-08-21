using System;
using System.Reflection;
using HarmonyLib;
using Terraria;
using Terraria.ID;

namespace TurboExtractinator.Patches
{
    /// <summary>
    /// Harmony patch targeting Player.PlaceThing_ItemInExtractinator to accelerate
    /// Extractinator processing speed by a configurable multiplier (default 5x).
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

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (_isProcessingBatch) return;

            var mod = TurboExtractinatorMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;
            if (__instance == null || __instance.whoAmI != Main.myPlayer || __instance.dead) return;

            int speed = Math.Max(1, Math.Min(60, mod.Config.SpeedMultiplier));

            // Reduce item cooldown delay according to speed multiplier (e.g. 15 ticks / 5 = 3 ticks)
            if (__instance.itemTime > 0)
            {
                __instance.itemTime = Math.Max(1, __instance.itemTime / speed);
            }
            if (__instance.itemAnimation > 0)
            {
                __instance.itemAnimation = Math.Max(1, __instance.itemAnimation / speed);
            }

            // Optional extra batch processing per tick if configured
            int batchSize = Math.Max(1, Math.Min(50, mod.Config.BatchExtractionSize));
            if (batchSize > 1 && ExtractinatorUseMethod != null)
            {
                Item heldItem = __instance.inventory[__instance.selectedItem];
                if (heldItem == null || heldItem.stack <= 0 || heldItem.type <= ItemID.None) return;

                Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
                if (tile == null || !tile.active()) return;

                int tileType = tile.type;
                if (tileType != TileID.Extractinator && tileType != TileID.ChlorophyteExtractinator) return;

                if (tileType == TileID.ChlorophyteExtractinator && !mod.Config.AffectsChlorophyteExtractinator) return;

                try
                {
                    _isProcessingBatch = true;
                    int extraExtracts = batchSize - 1;

                    for (int b = 0; b < extraExtracts; b++)
                    {
                        if (heldItem.stack <= 0 || heldItem.type <= ItemID.None) break;

                        int extractMode = ItemID.Sets.ExtractinatorMode[heldItem.type];
                        if (extractMode >= 0)
                        {
                            heldItem.stack--;
                            if (heldItem.stack <= 0)
                            {
                                heldItem.TurnToAir();
                            }
                            try
                            {
                                ExtractinatorUseMethod.Invoke(__instance, new object[] { extractMode, tileType });
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
    }
}
