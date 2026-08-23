using System;
using HarmonyLib;
using Terraria;

namespace PiggyVault.Patches
{
    /// <summary>
    /// Harmony patch on Player.RefreshInfoAccs to enable informational and display accessories located inside the Piggy Bank.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.RefreshInfoAccs))]
    public static class PiggyInfoAccessoriesPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (__instance == null || __instance.bank?.item == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.InfoAccessoriesInPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(__instance, mod.Config)) return;

            for (int i = 0; i < __instance.bank.item.Length; i++)
            {
                Item item = __instance.bank.item[i];
                if (item == null || item.IsAir || item.type <= 0) continue;

                try
                {
                    __instance.RefreshInfoAccsFromItemType(item.type);
                }
                catch { }
            }

            try
            {
                __instance.RefreshInfoAccsFromTeamPlayers();
            }
            catch { }
        }
    }
}
