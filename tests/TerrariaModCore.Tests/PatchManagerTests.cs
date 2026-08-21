using System;
using System.Reflection;
using HarmonyLib;
using TerrariaModCore.API;
using TerrariaModCore.Patching;

using System.Runtime.CompilerServices;

namespace TerrariaModCore.Tests
{
    public class DummyTarget
    {
        public int Value = 10;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public int Calculate(int x)
        {
            return Value + x;
        }
    }

    public static class DummyPrefixPatch
    {
        public static void Prefix(ref int x)
        {
            x *= 2;
        }
    }

    public static class DummyPostfixPatch
    {
        public static void Postfix(ref int __result)
        {
            __result += 100;
        }
    }

    public static class PatchManagerTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing PatchManager ---");
            var patchManager = new PatchManager(null);

            var targetMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.Calculate));
            var prefixMethod = typeof(DummyPrefixPatch).GetMethod(nameof(DummyPrefixPatch.Prefix));
            var postfixMethod = typeof(DummyPostfixPatch).GetMethod(nameof(DummyPostfixPatch.Postfix));

            var target = new DummyTarget();
            assert(target.Calculate(5) == 15, "Initial Target.Calculate(5) returns 15");

            // Test 1: Register Prefix for mod_1
            patchManager.RegisterPrefix("mod_1", targetMethod, prefixMethod, PatchPriority.Normal);
            assert(target.Calculate(5) == 20, "Prefix applied: Target.Calculate(5) -> (10 + (5*2)) = 20");

            var mod1Patches = patchManager.GetPatchesByMod("mod_1");
            assert(mod1Patches.Count == 1, "PatchManager tracks 1 patch for mod_1");

            // Test 2: Register Postfix for mod_2 (shared hook on same method)
            patchManager.RegisterPostfix("mod_2", targetMethod, postfixMethod, PatchPriority.Normal);
            assert(target.Calculate(5) == 120, "Both Prefix (mod_1) and Postfix (mod_2) active on shared target: returns 120");

            var allPatches = patchManager.GetAllPatches();
            assert(allPatches.Count == 2, "PatchManager tracks 2 total active patches");

            // Test 3: Unpatch mod_1 only
            patchManager.UnpatchAll("mod_1");
            assert(target.Calculate(5) == 115, "After unpatching mod_1, mod_2 Postfix remains active: (10+5)+100 = 115");
            assert(patchManager.GetPatchesByMod("mod_1").Count == 0, "mod_1 has 0 tracked patches after unpatch");

            // Test 4: Unpatch mod_2
            patchManager.UnpatchAll("mod_2");
            assert(target.Calculate(5) == 15, "After unpatching mod_2, original behavior restored: returns 15");
            assert(patchManager.GetAllPatches().Count == 0, "0 active patches remain in PatchManager");
        }
    }
}
