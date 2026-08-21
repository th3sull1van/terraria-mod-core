using System;
using System.Collections.Generic;
using OreCascade;
using Terraria;
using Terraria.ID;

namespace TerrariaModCore.Tests
{
    public static class OreCascadePluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing OreCascade Plugin Logic ---");

            var config = new CascadeConfig { Enabled = true, IncludeGems = true, RequireSameOreType = true, MaxBlocksPerActivation = 100 };

            // 1. Classification
            assert(OreClassifier.IsOre(TileID.Iron), "Iron is classified as Ore");
            assert(OreClassifier.IsOre(TileID.Gold), "Gold is classified as Ore");
            assert(OreClassifier.IsOre(TileID.Chlorophyte), "Chlorophyte is classified as Ore");
            assert(OreClassifier.IsGem(TileID.Diamond), "Diamond is classified as Gem");
            assert(OreClassifier.IsEligible(TileID.Diamond, config), "Diamond is eligible when IncludeGems=true");

            var noGems = new CascadeConfig { IncludeGems = false };
            assert(!OreClassifier.IsEligible(TileID.Diamond, noGems), "Diamond is NOT eligible when IncludeGems=false");

            assert(!OreClassifier.IsOre(TileID.Dirt), "Dirt is NOT classified as Ore");
            assert(!OreClassifier.IsEligible(TileID.Dirt, config), "Dirt is NOT eligible");

            // 2. Matching
            assert(OreClassifier.IsMatching(TileID.Iron, TileID.Iron, config), "Iron matches Iron");
            assert(!OreClassifier.IsMatching(TileID.Iron, TileID.Gold, config), "Iron does not match Gold (strict)");

            // 3. BFS Vein Discovery in Grid
            Main.maxTilesX = 50;
            Main.maxTilesY = 50;
            Main.tile = new Tile[50, 50];
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    Main.tile[x, y] = new Tile();
                    Main.tile[x, y].active(false);
                }
            }

            // Cluster of 4 Iron blocks
            Main.tile[10, 10].active(true); Main.tile[10, 10].type = TileID.Iron;
            Main.tile[10, 11].active(true); Main.tile[10, 11].type = TileID.Iron;
            Main.tile[11, 10].active(true); Main.tile[11, 10].type = TileID.Iron;
            Main.tile[12, 10].active(true); Main.tile[12, 10].type = TileID.Iron;

            var vein = VeinFinder.FindVein(10, 10, TileID.Iron, config);
            assert(vein.Count == 3, $"VeinFinder discovered 3 connected neighbor Iron blocks (Total cluster 4, Count: {vein.Count})");

            // 4. Reentrancy Guard
            assert(!CascadeMiner.IsCascading, "CascadeMiner.IsCascading initially false");
            CascadeMiner.IsCascading = true;
            assert(CascadeMiner.IsCascading, "CascadeMiner.IsCascading set to true");
            CascadeMiner.IsCascading = false;
            assert(!CascadeMiner.IsCascading, "CascadeMiner.IsCascading reset to false");
        }
    }
}
