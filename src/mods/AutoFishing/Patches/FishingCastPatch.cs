using System;
using HarmonyLib;
using Terraria;

namespace AutoFishing.Patches
{
    /// <summary>
    /// Harmony patch on Player.ItemCheck_Shoot to detect manual casting by the player and start automation.
    /// </summary>
    [HarmonyPatch(typeof(Player), "ItemCheck_Shoot")]
    public static class FishingCastPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i, Item sItem, int weaponDamage)
        {
            var mod = AutoFishingMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled)
            {
                return;
            }

            if (__instance == null || i != Main.myPlayer || sItem == null || sItem.fishingPole <= 0)
            {
                return;
            }

            // Only trigger if this cast was initiated by the player's manual click (not internal automation)
            if (!FishingController.IsInternalAction)
            {
                FishingController.OnManualCast(__instance, mod.Config);
            }
        }
    }
}
