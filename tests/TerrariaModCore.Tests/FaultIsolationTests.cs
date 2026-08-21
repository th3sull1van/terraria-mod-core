using System;
using System.IO;
using System.Reflection;
using TerrariaModCore.API;
using TerrariaModCore.Configuration;
using TerrariaModCore.Logging;
using TerrariaModCore.Patching;

namespace TerrariaModCore.Tests
{
    public class BrokenTestMod : IMod
    {
        public void Initialize(IModContext context)
        {
            context.Logger.Info("BrokenTestMod initializing...");
        }

        public void Load()
        {
            throw new InvalidOperationException("Simulated catastrophic crash inside BrokenTestMod.Load()");
        }

        public void Unload() { }
    }

    public class HealthyTestMod : IMod
    {
        public bool IsLoaded = false;

        public void Initialize(IModContext context) { }

        public void Load()
        {
            IsLoaded = true;
        }

        public void Unload()
        {
            IsLoaded = false;
        }
    }

    public static class FaultIsolationTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing Fault Isolation Boundary ---");

            var logger = new CoreLogger(null, "Test");
            var registry = new ModRegistry();
            var loader = new ModLoader(logger, registry);
            var patchManager = new PatchManager(logger);

            string asmFile = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var brokenManifest = new ModManifest
            {
                Id = "broken_test_mod",
                Name = "Broken Test Mod",
                Version = "1.0.0",
                EntryAssembly = asmFile,
                EntryType = typeof(BrokenTestMod).FullName,
                Enabled = true
            };

            var healthyManifest = new ModManifest
            {
                Id = "healthy_test_mod",
                Name = "Healthy Test Mod",
                Version = "1.0.0",
                EntryAssembly = asmFile,
                EntryType = typeof(HealthyTestMod).FullName,
                Enabled = true
            };

            registry.Register(new ModInfo { Manifest = brokenManifest, State = ModState.Discovered });
            registry.Register(new ModInfo { Manifest = healthyManifest, State = ModState.Discovered });

            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var brokenCtx = new ModContext(brokenManifest, dir, dir, new ModLogger(logger, brokenManifest.Id), null, patchManager, null, null, "1.4.5.7");
            var healthyCtx = new ModContext(healthyManifest, dir, dir, new ModLogger(logger, healthyManifest.Id), null, patchManager, null, null, "1.4.5.7");

            // 1. Load Broken Mod
            bool brokenResult = loader.LoadMod(brokenManifest, dir, brokenCtx, out IMod brokenInst);
            assert(!brokenResult, "BrokenTestMod load returns false (fault intercepted)");
            assert(registry.GetMod("broken_test_mod").State == ModState.Failed, "BrokenTestMod state marked as FAILED in registry");
            assert(registry.GetMod("broken_test_mod").ErrorDetails.Contains("Simulated catastrophic crash"), "Error details captured in registry");

            // 2. Load Healthy Mod
            bool healthyResult = loader.LoadMod(healthyManifest, dir, healthyCtx, out IMod healthyInst);
            assert(healthyResult, "HealthyTestMod loads successfully despite prior mod crash");
            assert(registry.GetMod("healthy_test_mod").State == ModState.Loaded, "HealthyTestMod state marked as LOADED in registry");
            assert(((HealthyTestMod)healthyInst).IsLoaded, "HealthyTestMod is active and functional");
        }
    }
}
