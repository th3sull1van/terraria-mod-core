using System;
using HarmonyLib;
using Terraria;

namespace AutoFishing.Patches
{
    /// <summary>
    /// Harmony patch on Player.Update to execute the fishing automation loop each tick.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class PlayerFishingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            var mod = AutoFishingMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled)
            {
                return;
            }

            if (__instance == null || i != Main.myPlayer)
            {
                return;
            }

            FishingController.Update(__instance, mod.Config);
        }
    }
}
