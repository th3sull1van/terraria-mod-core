using System;
using HarmonyLib;
using Terraria;

namespace AutoFishing.Patches
{
    /// <summary>
    /// Harmony patch on Player.ItemCheck_PullFishingBobbers to detect manual reel-in by the player and stop automation.
    /// </summary>
    [HarmonyPatch(typeof(Player), "ItemCheck_PullFishingBobbers")]
    public static class FishingPullPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Player __instance)
        {
            var mod = AutoFishingMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled)
            {
                return;
            }

            if (__instance == null || __instance.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Only trigger if this pull was initiated by the player's manual click (not internal auto-reel)
            if (!FishingController.IsInternalAction)
            {
                FishingController.OnManualPull(__instance, mod.Config);
            }
        }
    }
}
