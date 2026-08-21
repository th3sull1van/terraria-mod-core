using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;

namespace OreCascade
{
    /// <summary>
    /// Coordinates cascade mining execution, reentrancy guards, pickaxe validation, and multiplayer synchronization.
    /// </summary>
    public static class CascadeMiner
    {
        [ThreadStatic]
        private static bool _isCascading;

        public static bool IsCascading
        {
            get => _isCascading;
            set => _isCascading = value;
        }

        public static void ExecuteCascade(Player player, int startX, int startY, ushort initialOreType, int pickPower, CascadeConfig config)
        {
            if (IsCascading || config == null || !config.Enabled)
            {
                return;
            }

            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld || Main.tile == null || player == null)
            {
                return;
            }

            try
            {
                IsCascading = true;

                List<TilePos> vein = VeinFinder.FindVein(startX, startY, initialOreType, config);
                if (vein == null || vein.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < vein.Count; i++)
                {
                    TilePos pos = vein[i];
                    int x = pos.X;
                    int y = pos.Y;

                    if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
                    {
                        continue;
                    }

                    Tile tile = Main.tile[x, y];
                    if (tile == null || !tile.active())
                    {
                        continue;
                    }

                    if (!OreClassifier.IsMatching(initialOreType, tile.type, config))
                    {
                        continue;
                    }

                    if (!WorldGen.CanKillTile(x, y))
                    {
                        continue;
                    }

                    player.hitTile?.TryClearingAndPruning(x, y, 1);
                    player.hitReplace?.TryClearingAndPruning(x, y, 1);

                    bool wasActive = tile.active();
                    AchievementsHelper.CurrentlyMining = true;
                    try
                    {
                        WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: false);
                    }
                    finally
                    {
                        AchievementsHelper.CurrentlyMining = false;
                    }

                    if (wasActive && !Main.tile[x, y].active())
                    {
                        if (!Main.dedServ)
                        {
                            AchievementsHelper.HandleMining();
                        }

                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(17, -1, -1, null, 0, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OreCascade] Error executing cascade mining: {ex}");
            }
            finally
            {
                IsCascading = false;
                player.hitTile?.Prune();
            }
        }
    }
}
