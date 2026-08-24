using System;
using System.IO;
using Terraria;
using Terraria.ID;
using TerrariaModCore.API;
using TurboBucket;

namespace TerrariaModCore.Tests
{
    public static class TurboBucketPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing TurboBucket Plugin Logic ---");

            // 1. Config Defaults & Boundaries
            var config = new TurboBucketConfig();
            assert(config.Enabled, "Default TurboBucketConfig Enabled is true");
            assert(config.SpeedMultiplier == 5, "Default SpeedMultiplier is 5x (2 ticks/pour)");
            assert(config.AffectsWater, "Default AffectsWater is true");
            assert(config.AffectsLava, "Default AffectsLava is true");
            assert(config.AffectsHoney, "Default AffectsHoney is true");
            assert(config.AffectsBottomlessBuckets, "Default AffectsBottomlessBuckets is true");
            assert(!config.AffectsEmptyBuckets, "Default AffectsEmptyBuckets is false");
            assert(!config.AffectsSponges, "Default AffectsSponges is false");

            // 2. Liquid Bucket Identification
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.WaterBucket, config), "WaterBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.LavaBucket, config), "LavaBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.HoneyBucket, config), "HoneyBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.BottomlessBucket, config), "BottomlessBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.BottomlessLavaBucket, config), "BottomlessLavaBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.BottomlessHoneyBucket, config), "BottomlessHoneyBucket is eligible for turbo pouring");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.BottomlessShimmerBucket, config), "BottomlessShimmerBucket is eligible for turbo pouring");

            // Non-target items
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.IronPickaxe, config), "IronPickaxe is NOT eligible");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.DirtBlock, config), "DirtBlock is NOT eligible");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.None, config), "ItemID.None is NOT eligible");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.EmptyBucket, config), "EmptyBucket is not eligible by default");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.SuperAbsorbantSponge, config), "Sponge is not eligible by default");

            // 3. Selective Toggles
            var customConfig = new TurboBucketConfig
            {
                AffectsWater = false,
                AffectsLava = true,
                AffectsHoney = false,
                AffectsBottomlessBuckets = false,
                AffectsEmptyBuckets = true,
                AffectsSponges = true
            };

            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.WaterBucket, customConfig), "WaterBucket disabled when AffectsWater=false");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.LavaBucket, customConfig), "LavaBucket enabled when AffectsLava=true");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.HoneyBucket, customConfig), "HoneyBucket disabled when AffectsHoney=false");
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.BottomlessBucket, customConfig), "BottomlessBucket disabled when AffectsBottomlessBuckets=false");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.EmptyBucket, customConfig), "EmptyBucket enabled when AffectsEmptyBuckets=true");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.SuperAbsorbantSponge, customConfig), "Sponge enabled when AffectsSponges=true");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.LavaAbsorbantSponge, customConfig), "LavaAbsorbantSponge enabled when AffectsSponges=true");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.HoneyAbsorbantSponge, customConfig), "HoneyAbsorbantSponge enabled when AffectsSponges=true");
            assert(TurboBucketController.IsTargetLiquidBucket(ItemID.UltraAbsorbantSponge, customConfig), "UltraAbsorbantSponge enabled when AffectsSponges=true");

            // Disabled mod
            var disabledConfig = new TurboBucketConfig { Enabled = false };
            assert(!TurboBucketController.IsTargetLiquidBucket(ItemID.WaterBucket, disabledConfig), "Target check returns false when Enabled=false");

            // 4. Speed Boost Application
            var player = new Player();
            player.itemTime = 10;
            player.itemAnimation = 10;
            player.itemAnimationMax = 10;

            var waterItem = new Item();
            waterItem.type = ItemID.WaterBucket;

            TurboBucketController.ApplySpeedBoost(player, waterItem, config);
            assert(player.itemTime == 2, "itemTime reduced from 10 to 2 at 5x speed");
            assert(player.itemAnimation == 2, "itemAnimation reduced from 10 to 2 at 5x speed");
            assert(player.itemAnimationMax == 2, "itemAnimationMax reduced from 10 to 2 at 5x speed");

            // 10x Max Speed (60 TPS)
            var maxSpeedConfig = new TurboBucketConfig { SpeedMultiplier = 10 };
            player.itemTime = 10;
            player.itemAnimation = 10;
            player.itemAnimationMax = 10;

            var lavaItem = new Item();
            lavaItem.type = ItemID.LavaBucket;

            TurboBucketController.ApplySpeedBoost(player, lavaItem, maxSpeedConfig);
            assert(player.itemTime == 1, "itemTime reduced from 10 to 1 at 10x max speed (60 TPS)");
            assert(player.itemAnimation == 1, "itemAnimation reduced from 10 to 1 at 10x max speed");

            // Non-target item preserved
            var pickItem = new Item();
            pickItem.type = ItemID.IronPickaxe;
            player.itemTime = 15;
            player.itemAnimation = 15;

            TurboBucketController.ApplySpeedBoost(player, pickItem, config);
            assert(player.itemTime == 15, "Non-bucket pickaxe itemTime is untouched");
            assert(player.itemAnimation == 15, "Non-bucket pickaxe itemAnimation is untouched");

            // 5. Plugin Lifecycle Integration
            string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TurboBucketTest");
            if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);

            var manifest = new ModManifest
            {
                Id = "turbobucket",
                Name = "TurboBucket",
                Version = "1.0.0",
                EntryAssembly = "TurboBucket.dll",
                EntryType = typeof(TurboBucketMod).FullName,
                Enabled = true
            };

            var coreLogger = new TerrariaModCore.Logging.CoreLogger(null, "Test");
            var modLogger = new TerrariaModCore.Logging.ModLogger(coreLogger, "turbobucket");
            var patchManager = new TerrariaModCore.Patching.PatchManager(coreLogger);
            var configManager = new TerrariaModCore.Configuration.ModConfigManager(testDir, modLogger);
            var context = new ModContext(manifest, testDir, testDir, modLogger, configManager, patchManager, null, null, "1.4.5.8");

            var mod = new TurboBucketMod();
            mod.Initialize(context);
            assert(mod.Config != null, "TurboBucketMod loaded configuration successfully");
            assert(patchManager.GetPatchesByMod("turbobucket").Count == 1, "TurboBucket registered 1 Harmony patch (Player.ItemCheck_UseBuckets)");

            mod.Load();
            assert(TurboBucketMod.Instance == mod, "TurboBucketMod instance active");

            mod.Unload();
            patchManager.UnpatchAll("turbobucket");
            assert(patchManager.GetPatchesByMod("turbobucket").Count == 0, "TurboBucket unpatched cleanly");
            assert(TurboBucketMod.Instance == null, "TurboBucketMod instance cleared on unload");
        }
    }
}
