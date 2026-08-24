using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaModCore.API;
using TerrariaModCore.Configuration;
using TerrariaModCore.Logging;
using TerrariaModCore.Patching;
using TurboExtractinator;
using TurboExtractinator.Patches;

namespace TerrariaModCore.Tests
{
    /// <summary>
    /// Automated test suite for the TurboExtractinator plugin.
    /// </summary>
    public static class TurboExtractinatorPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- TurboExtractinator Plugin Tests ---");

            // 1. Default Configuration
            var config = new TurboExtractConfig();
            assert(config.Enabled == true, "TurboExtractConfig: Enabled defaults to true");
            assert(config.SpeedMultiplier == 5, "TurboExtractConfig: SpeedMultiplier defaults to 5");
            assert(config.AffectsChlorophyteExtractinator == true, "TurboExtractConfig: AffectsChlorophyteExtractinator defaults to true");
            assert(config.BatchExtractionSize == 1, "TurboExtractConfig: BatchExtractionSize defaults to 1");

            // 2. Speed Scaling Logic Simulation
            int baseItemTime = 15;
            int speedMultiplier = 5;
            int acceleratedItemTime = Math.Max(1, baseItemTime / speedMultiplier);
            assert(acceleratedItemTime == 3, $"Speed calculation: 15 ticks / 5x = {acceleratedItemTime} ticks (5x faster)");

            int speed15 = 15;
            int accelerated15 = Math.Max(1, baseItemTime / speed15);
            assert(accelerated15 == 1, $"Speed calculation: 15 ticks / 15x = {accelerated15} tick (1 frame per item)");

            // 3. Item Validation Guard Simulation (Throwing Knives vs Desert Fossil)
            int throwingKnifeType = ItemID.ThrowingKnife; // 279
            int extractModeThrowingKnife = throwingKnifeType < ItemID.Sets.ExtractinatorMode.Length ? ItemID.Sets.ExtractinatorMode[throwingKnifeType] : -1;
            assert(extractModeThrowingKnife < 0, "Extractinator Guard: ThrowingKnife is NOT an extractable item (mode < 0)");

            int desertFossilType = ItemID.DesertFossil; // 3380
            int extractModeFossil = desertFossilType < ItemID.Sets.ExtractinatorMode.Length ? ItemID.Sets.ExtractinatorMode[desertFossilType] : -1;
            assert(extractModeFossil >= 0, "Extractinator Guard: DesertFossil is a valid extractable item (mode >= 0)");

            int siltType = ItemID.SiltBlock; // 173
            int extractModeSilt = siltType < ItemID.Sets.ExtractinatorMode.Length ? ItemID.Sets.ExtractinatorMode[siltType] : -1;
            assert(extractModeSilt >= 0, "Extractinator Guard: SiltBlock is a valid extractable item (mode >= 0)");

            // 3. Mod Initialization & Lifecycle
            var logger = new CoreLogger(null, "Test");
            var modLogger = new ModLogger(logger, "turbo_extractinator");
            var patchManager = new PatchManager(logger);
            string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_temp_turbo_" + Guid.NewGuid().ToString("N"));

            try
            {
                if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);

                var configMgr = new ModConfigManager(testDir, modLogger);
                var manifest = new ModManifest
                {
                    Id = "turbo_extractinator",
                    Name = "TurboExtractinator",
                    Version = "1.1.0",
                    EntryAssembly = "TurboExtractinator.dll",
                    EntryType = "TurboExtractinator.TurboExtractinatorMod",
                    Enabled = true
                };

                var context = new ModContext(
                    manifest,
                    testDir,
                    testDir,
                    modLogger,
                    configMgr,
                    patchManager,
                    null,
                    null,
                    "1.4.5.8"
                );

                var mod = new TurboExtractinatorMod();
                mod.Initialize(context);
                assert(mod.Config != null, "TurboExtractinatorMod: Config loaded during Initialize");
                assert(mod.Config.SpeedMultiplier == 5, "TurboExtractinatorMod: Config SpeedMultiplier is 5");

                mod.Load();
                assert(patchManager.GetPatchesByMod("turbo_extractinator").Count >= 1, "TurboExtractinatorMod: PlaceThing_ItemInExtractinator patch registered");

                mod.Unload();
                patchManager.UnpatchAll("turbo_extractinator");
                assert(patchManager.GetPatchesByMod("turbo_extractinator").Count == 0, "TurboExtractinatorMod: Clean unpatch on unload");
            }
            finally
            {
                if (Directory.Exists(testDir))
                {
                    try { Directory.Delete(testDir, true); } catch { }
                }
            }
        }
    }
}
