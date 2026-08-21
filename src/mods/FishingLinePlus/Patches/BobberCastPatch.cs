using System;
using HarmonyLib;
using Terraria;

namespace FishingLinePlus.Patches
{
    /// <summary>
    /// Harmony patch targeting Player.ItemCheck_Shoot to spawn multiple simultaneous fishing bobbers
    /// according to the FishingLinePlus configuration.
    /// </summary>
    [HarmonyPatch(typeof(Player), "ItemCheck_Shoot")]
    public static class BobberCastPatch
    {
        [ThreadStatic]
        private static bool _isSpawning;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i, Item sItem, int weaponDamage)
        {
            // Prevent recursive triggering when spawning extra projectiles
            if (_isSpawning)
            {
                return;
            }

            var mod = FishingLinePlusMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || sItem == null || sItem.fishingPole <= 0)
            {
                return;
            }

            if (__instance == null || i != __instance.whoAmI)
            {
                return;
            }

            try
            {
                _isSpawning = true;
                MultiBobberHelper.SpawnExtraBobbers(__instance, sItem, weaponDamage, mod.Config);
            }
            finally
            {
                _isSpawning = false;
            }
        }
    }
}
