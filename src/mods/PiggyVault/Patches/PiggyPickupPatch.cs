using System;
using HarmonyLib;
using Terraria;

namespace PiggyVault.Patches
{
    /// <summary>
    /// Harmony patch on Player.GetItem to automatically route unhandled items/coins into the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.GetItem))]
    public static class PiggyGetItemPatch
    {
        [ThreadStatic]
        private static bool _inPickup;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, Item newItem, GetItemSettings settings, ref Item __result)
        {
            if (_inPickup) return;
            if (__instance == null || __result == null || __result.IsAir || __result.stack <= 0) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.AutoPickupToPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            // Ensure settings permit automatic container pickup
            if (!settings.CanGoIntoVoidVault)
            {
                return;
            }

            try
            {
                _inPickup = true;
                __result = PiggyVaultController.PutItemInPiggyBank(__instance, __result, settings);
            }
            finally
            {
                _inPickup = false;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.ItemSpaceForCofveve to allow world item pickups when space is available in Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.ItemSpaceForCofveve))]
    public static class PiggyItemSpacePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, Item newItem, ref bool __result)
        {
            if (__result) return;
            if (__instance == null || newItem == null || newItem.IsAir) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.AutoPickupToPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            if (PiggyVaultController.HasSpaceInPiggyBank(__instance, newItem))
            {
                __result = true;
            }
        }
    }
}
