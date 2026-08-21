using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoFishing;
using FishingLinePlus;
using OreCascade;
using TerrariaModCore.API;
using TerrariaModCore.Configuration;
using TerrariaModCore.Logging;
using TerrariaModCore.Patching;

namespace TerrariaModCore.Tests
{
    public static class ModCoexistenceTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("     Mod Coexistence Matrix Testing       ");
            Console.WriteLine("==========================================");

            // Scenario 1: OreCascade alone
            TestCombination(assert, "Scenario 1: OreCascade alone", new[] { typeof(OreCascadeMod) });

            // Scenario 2: AutoFishing alone
            TestCombination(assert, "Scenario 2: AutoFishing alone", new[] { typeof(AutoFishingMod) });

            // Scenario 3: FishingLinePlus alone
            TestCombination(assert, "Scenario 3: FishingLinePlus alone", new[] { typeof(FishingLinePlusMod) });

            // Scenario 4: OreCascade + AutoFishing
            TestCombination(assert, "Scenario 4: OreCascade + AutoFishing", new[] { typeof(OreCascadeMod), typeof(AutoFishingMod) });

            // Scenario 5: OreCascade + FishingLinePlus
            TestCombination(assert, "Scenario 5: OreCascade + FishingLinePlus", new[] { typeof(OreCascadeMod), typeof(FishingLinePlusMod) });

            // Scenario 6: AutoFishing + FishingLinePlus (Shared fishing hooks)
            TestCombination(assert, "Scenario 6: AutoFishing + FishingLinePlus", new[] { typeof(AutoFishingMod), typeof(FishingLinePlusMod) });

            // Scenario 7: All three mods simultaneously
            TestCombination(assert, "Scenario 7: All Three (OreCascade + AutoFishing + FishingLinePlus)",
                new[] { typeof(OreCascadeMod), typeof(AutoFishingMod), typeof(FishingLinePlusMod) });
        }

        private static void TestCombination(Action<bool, string> assert, string scenarioName, Type[] modTypes)
        {
            Console.WriteLine($"\n--- {scenarioName} ---");

            var logger = new CoreLogger(null, "Test");
            var patchManager = new PatchManager(logger);
            var activeMods = new List<IMod>();
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_temp_" + Guid.NewGuid().ToString("N"));

            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                foreach (var modType in modTypes)
                {
                    string modId = modType.Namespace.ToLowerInvariant();
                    var manifest = new ModManifest
                    {
                        Id = modId,
                        Name = modType.Namespace,
                        Version = "1.0.0",
                        EntryAssembly = modType.Assembly.GetName().Name + ".dll",
                        EntryType = modType.FullName,
                        Enabled = true
                    };

                    string modDir = Path.Combine(dir, modId);
                    Directory.CreateDirectory(modDir);

                    var modLogger = new ModLogger(logger, modId);
                    var modConfigMgr = new ModConfigManager(modDir, modLogger);
                    var context = new ModContext(manifest, modDir, modDir, modLogger, modConfigMgr, patchManager, null, null, "1.4.5.7");

                    var modInstance = (IMod)Activator.CreateInstance(modType);
                    modInstance.Initialize(context);
                    modInstance.Load();
                    activeMods.Add(modInstance);

                    logger.Info($"Mod {modId} initialized and loaded.");
                }

                int expectedMinPatches = modTypes.Length;
                int registeredPatches = patchManager.GetAllPatches().Count;
                assert(registeredPatches >= expectedMinPatches,
                    $"{scenarioName}: Successfully registered and applied {registeredPatches} patches across {modTypes.Length} active mod(s)");

                // Clean unload
                foreach (var mod in activeMods)
                {
                    string modId = mod.GetType().Namespace.ToLowerInvariant();
                    mod.Unload();
                    patchManager.UnpatchAll(modId);
                }

                assert(patchManager.GetAllPatches().Count == 0,
                    $"{scenarioName}: Clean unload verified (0 remaining patches)");
            }
            catch (Exception ex)
            {
                assert(false, $"{scenarioName} FAILED with Exception: {ex.Message}");
            }
            finally
            {
                try { if (Directory.Exists(dir)) Directory.Delete(dir, true); } catch { }
            }
        }
    }
}
