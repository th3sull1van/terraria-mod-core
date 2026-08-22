using System;
using HarmonyLib;
using Terraria;

namespace AutoBuff.Patches
{
    /// <summary>
    /// Harmony patch on Player.Update to evaluate buff expirations and auto-consume potions each tick interval.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class PlayerBuffPatch
    {
        [ThreadStatic]
        private static bool isUpdating;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            if (isUpdating) return;

            var mod = AutoBuffMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled)
            {
                return;
            }

            if (__instance == null || i != Main.myPlayer)
            {
                return;
            }

            try
            {
                isUpdating = true;
                BuffController.Update(__instance, mod.Config);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
}
