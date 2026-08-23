using System;
using HarmonyLib;
using Terraria;

namespace PiggyVault.Patches
{
    /// <summary>
    /// Harmony patch on Player.QuickHeal_GetItemToUse to fall back to healing potions in the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "QuickHeal_GetItemToUse")]
    public static class PiggyQuickHealPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref Item __result)
        {
            if (__result != null && !__result.IsAir && __result.healLife > 0) return;
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.QuickHealFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            Item bankItem = PiggyVaultController.GetQuickHealItemFromPiggyBank(__instance);
            if (bankItem != null && !bankItem.IsAir)
            {
                __result = bankItem;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.QuickMana_GetItemToUse to fall back to mana potions in the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "QuickMana_GetItemToUse")]
    public static class PiggyQuickManaPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref Item __result)
        {
            if (__result != null && !__result.IsAir && __result.healMana > 0) return;
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.QuickManaFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            Item bankItem = PiggyVaultController.GetQuickManaItemFromPiggyBank(__instance);
            if (bankItem != null && !bankItem.IsAir)
            {
                __result = bankItem;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.QuickBuff_PickBestFoodItem to consider food inside the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "QuickBuff_PickBestFoodItem")]
    public static class PiggyQuickFoodPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref Item __result)
        {
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.QuickBuffFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            Item bankFood = PiggyVaultController.PickBestFoodItemFromPiggyBank(__instance);
            if (bankFood != null && !bankFood.IsAir)
            {
                if (__result == null || __result.IsAir)
                {
                    __result = bankFood;
                }
                else
                {
                    int currentPrio = PiggyVaultController.GetFoodPriority(__result.buffType);
                    int bankPrio = PiggyVaultController.GetFoodPriority(bankFood.buffType);

                    if (bankPrio > currentPrio || (bankPrio == currentPrio && bankFood.buffTime > __result.buffTime))
                    {
                        __result = bankFood;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.QuickBuff to apply active buffs from potions in the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "QuickBuff")]
    public static class PiggyQuickBuffPatch
    {
        [ThreadStatic]
        private static bool _isBuffing;

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (_isBuffing) return;
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.QuickBuffFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            try
            {
                _isBuffing = true;
                PiggyVaultController.QuickBuffFromPiggyBank(__instance);
            }
            finally
            {
                _isBuffing = false;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.ConsumeItem to allow consuming ammo, wire, and bait from the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "ConsumeItem")]
    public static class PiggyConsumeItemPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, int type, bool reverseOrder, bool includeVoidBag, ref bool __result)
        {
            if (__result) return;
            if (__instance == null || type <= 0) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.ConsumeAmmoAndBaitFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            if (PiggyVaultController.ConsumeItemFromPiggyBank(__instance, type))
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.HasUnityPotion to check for Wormhole Potions in the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), "HasUnityPotion")]
    public static class PiggyHasUnityPotionPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref bool __result)
        {
            if (__result) return;
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.WormholePotionFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            if (PiggyVaultController.HasUnityPotionInPiggyBank(__instance))
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Harmony patch on Player.TakeUnityPotion to consume a Wormhole Potion from the Piggy Bank if needed.
    /// </summary>
    [HarmonyPatch(typeof(Player), "TakeUnityPotion")]
    public static class PiggyTakeUnityPotionPatch
    {
        [ThreadStatic]
        private static bool _hadInInventoryOrVoid;

        [HarmonyPrefix]
        public static void Prefix(Player __instance)
        {
            _hadInInventoryOrVoid = false;
            if (__instance == null) return;

            try
            {
                _hadInInventoryOrVoid = __instance.HasItem(PiggyVaultController.ItemIdWormholePotion);
                if (!_hadInInventoryOrVoid && __instance.useVoidBag() && __instance.bank4?.item != null)
                {
                    _hadInInventoryOrVoid = __instance.HasItem(PiggyVaultController.ItemIdWormholePotion, __instance.bank4.item);
                }
            }
            catch { }
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (_hadInInventoryOrVoid) return;
            if (__instance == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.WormholePotionFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            PiggyVaultController.TakeUnityPotionFromPiggyBank(__instance);
        }
    }
}
