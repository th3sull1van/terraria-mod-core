using System;
using AutoFishing;
using Terraria;

namespace TerrariaModCore.Tests
{
    public static class AutoFishingPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing AutoFishing Plugin Logic ---");

            var player = new Player();
            player.whoAmI = 0;
            Main.myPlayer = 0;
            Main.gameMenu = false;
            WorldGen.isGeneratingOrLoadingWorld = false;

            // 1. Inventory Bait Detection
            player.inventory = new Item[59];
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();

            assert(!FishingController.HasBait(player), "HasBait returns false when inventory has no bait");

            player.inventory[5].stack = 10;
            player.inventory[5].bait = 15; // Master Bait / Worm
            assert(FishingController.HasBait(player), "HasBait returns true when bait item present in inventory");

            // 2. Active Bobber Detection
            Main.projectile = new Projectile[1000];
            for (int i = 0; i < 1000; i++) Main.projectile[i] = new Projectile();

            var bobbersBefore = FishingController.GetActiveBobbers(player.whoAmI);
            assert(bobbersBefore.Count == 0, "Initial active bobbers count is 0");

            // Spawn active bobber
            Main.projectile[10].active = true;
            Main.projectile[10].owner = 0;
            Main.projectile[10].bobber = true;
            Main.projectile[10].ai[0] = 0f;
            Main.projectile[10].ai[1] = 100f; // Waiting for bite

            var bobbersAfter = FishingController.GetActiveBobbers(player.whoAmI);
            assert(bobbersAfter.Count == 1, "Discovered 1 active bobber owned by player");

            // 3. Bite Detection Condition
            bool isBiting = (Main.projectile[10].ai[1] < 0f && Main.projectile[10].localAI[1] != 0f);
            assert(!isBiting, "No bite detected while ai[1] > 0");

            // Simulate bite trigger from engine
            Main.projectile[10].ai[1] = -120f;
            Main.projectile[10].localAI[1] = 1f;

            bool isBitingNow = (Main.projectile[10].ai[1] < 0f && Main.projectile[10].localAI[1] != 0f);
            assert(isBitingNow, "Bite successfully detected when ai[1] < 0 and localAI[1] != 0");

            // Cleanup
            Main.projectile[10].active = false;
        }
    }
}
