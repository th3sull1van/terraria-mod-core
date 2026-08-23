using System;
using HarmonyLib;
using Terraria;

namespace TurboBucket.Patches
{
    /// <summary>
    /// Harmony postfix patch on Player.ItemCheck_UseBuckets to accelerate bucket pouring
    /// and liquid manipulation according to the configured SpeedMultiplier.
    /// </summary>
    [HarmonyPatch(typeof(Player), "ItemCheck_UseBuckets")]
    public static class BucketSpeedPatch
    {
        [ThreadStatic]
        private static bool isProcessing;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, Item sItem)
        {
            if (isProcessing) return;

            var mod = TurboBucketMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;

            if (__instance == null || __instance.whoAmI != Main.myPlayer || sItem == null || sItem.IsAir)
            {
                return;
            }

            try
            {
                isProcessing = true;
                TurboBucketController.ApplySpeedBoost(__instance, sItem, mod.Config);
            }
            finally
            {
                isProcessing = false;
            }
        }
    }
}
