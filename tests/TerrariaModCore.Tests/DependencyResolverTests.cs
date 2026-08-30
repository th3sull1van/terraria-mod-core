using System;
using System.Collections.Generic;
using System.Linq;
using TerrariaModCore.API;
using TerrariaModCore.Dependencies;

namespace TerrariaModCore.Tests
{
    public static class DependencyResolverTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing DependencyResolver ---");
            var resolver = new DependencyResolver();

            // Test 1: Linear Dependency A -> B -> C
            var mA = new ModManifest { Id = "mod_a", Name = "Mod A", Enabled = true, Dependencies = new List<string> { "mod_b" } };
            var mB = new ModManifest { Id = "mod_b", Name = "Mod B", Enabled = true, Dependencies = new List<string> { "mod_c" } };
            var mC = new ModManifest { Id = "mod_c", Name = "Mod C", Enabled = true };

            var res1 = resolver.Resolve(new[] { mA, mB, mC });
            assert(res1.Success, "Linear dependencies resolved successfully");
            assert(res1.OrderedMods.Count == 3, "All 3 mods ordered");
            assert(res1.OrderedMods[0].Id == "mod_c" && res1.OrderedMods[1].Id == "mod_b" && res1.OrderedMods[2].Id == "mod_a",
                "Load order strictly respects dependencies: [mod_c, mod_b, mod_a]");

            // Test 2: Missing Mandatory Dependency
            var mIsolated = new ModManifest { Id = "mod_isolated", Name = "Mod Isolated", Enabled = true, Dependencies = new List<string> { "missing_lib" } };
            var res2 = resolver.Resolve(new[] { mIsolated });
            assert(!res2.Success, "Missing dependency fails resolution");
            assert(res2.Errors.Any(e => e.Contains("missing_lib")), "Error message mentions missing dependency 'missing_lib'");

            // Test 3: Circular Dependency (A -> B -> C -> A)
            var cA = new ModManifest { Id = "cycle_a", Name = "Cycle A", Enabled = true, Dependencies = new List<string> { "cycle_b" } };
            var cB = new ModManifest { Id = "cycle_b", Name = "Cycle B", Enabled = true, Dependencies = new List<string> { "cycle_c" } };
            var cC = new ModManifest { Id = "cycle_c", Name = "Cycle C", Enabled = true, Dependencies = new List<string> { "cycle_a" } };

            var res3 = resolver.Resolve(new[] { cA, cB, cC });
            assert(!res3.Success, "Circular dependency fails resolution");
            assert(res3.Errors.Any(e => e.Contains("Circular dependency")), "Error message explicitly identifies circular dependency");

            // Test 4: Incompatible With
            var incompA = new ModManifest { Id = "incomp_a", Name = "Incomp A", Enabled = true, IncompatibleWith = new List<string> { "incomp_b" } };
            var incompB = new ModManifest { Id = "incomp_b", Name = "Incomp B", Enabled = true };

            var res4 = resolver.Resolve(new[] { incompA, incompB });
            assert(!res4.Success, "Incompatible mods fail resolution");
            assert(res4.Errors.Any(e => e.Contains("incompatible")), "Error message identifies incompatible mod pair");

            // Test 5: LoadBefore / LoadAfter Ordering
            var oA = new ModManifest { Id = "alpha", Name = "Alpha", Enabled = true, LoadBefore = new List<string> { "beta" } };
            var oB = new ModManifest { Id = "beta", Name = "Beta", Enabled = true };

            var res5 = resolver.Resolve(new[] { oB, oA });
            assert(res5.Success, "LoadBefore ordering resolved successfully");
            assert(res5.OrderedMods[0].Id == "alpha" && res5.OrderedMods[1].Id == "beta", "Alpha ordered before Beta");

            // Test 6: Disabled Mod Ignored
            var dis = new ModManifest { Id = "disabled_mod", Name = "Disabled Mod", Enabled = false };
            var res6 = resolver.Resolve(new[] { dis });
            assert(res6.Success && res6.OrderedMods.Count == 0, "Disabled mod is excluded from load order");
        }
    }
}
