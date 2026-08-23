using System;
using System.Reflection;
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

            // 4. Automation Lifecycle & Manual Start/Stop
            var config = new AutoFishingConfig
            {
                Enabled = true,
                AutoCast = true,
                AutoReel = true,
                CastDelayTicks = 30,
                ReelDelayTicks = 2,
                RequireBait = true
            };

            FishingController.Reset();
            assert(!FishingController.IsAutomating, "FishingController starts in inactive state (IsAutomating == false)");

            // Select fishing rod
            SetSelectedItem(player, 0);
            player.inventory[0].fishingPole = 30; // Wood Fishing Pole
            Main.projectile[10].active = false;   // No active bobbers

            // Ticking while inactive should NOT trigger auto-cast
            player.controlUseItem = false;
            FishingController.Update(player, config);
            assert(!FishingController.IsAutomating, "Holding fishing rod does not auto-activate automation without manual cast");
            assert(!player.controlUseItem, "Holding fishing rod does not simulate use item before first user click");

            // Simulate user manual cast
            FishingController.OnManualCast(player, config);
            assert(FishingController.IsAutomating, "Manual cast activates automation (IsAutomating == true)");
            assert(FishingController.CastTimer == config.CastDelayTicks, "Cast timer initialized to CastDelayTicks on manual cast");

            // Simulate user manual reel-in / cancel
            FishingController.OnManualPull(player, config);
            assert(!FishingController.IsAutomating, "Manual reel-in cancels automation (IsAutomating == false)");

            // Verify no auto-cast after manual reel-in
            player.controlUseItem = false;
            FishingController.Update(player, config);
            assert(!player.controlUseItem, "No auto-cast occurs after manual reel-in cancellation");

            // Simulate manual cast followed by slot change
            FishingController.OnManualCast(player, config);
            assert(FishingController.IsAutomating, "Automation reactivated on subsequent manual cast");

            SetSelectedItem(player, 1); // Switch to sword / different slot
            FishingController.Update(player, config);
            assert(!FishingController.IsAutomating, "Switching selected inventory slot resets automation");

            // Cleanup
            FishingController.Reset();
            Main.projectile[10].active = false;
        }

        private static void SetSelectedItem(Player player, int slot)
        {
            var stateField = typeof(Player).GetField("selectedItemState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (stateField != null)
            {
                var stateObj = stateField.GetValue(player);
                if (stateObj != null)
                {
                    var selectedField = stateObj.GetType().GetField("selected", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (selectedField != null)
                    {
                        selectedField.SetValue(stateObj, slot);
                    }
                    var hotbarField = stateObj.GetType().GetField("hotbar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (hotbarField != null)
                    {
                        hotbarField.SetValue(stateObj, slot);
                    }
                    stateField.SetValue(player, stateObj);
                }
            }
        }
    }
}
