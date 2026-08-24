using System;
using HarmonyLib;
using Terraria;

namespace PiggyVault.Patches
{
    /// <summary>
    /// Harmony patch on Player.UpdateEquips to enable informational and display accessories located inside the Piggy Bank during gameplay.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateEquips))]
    public static class PiggyUpdateEquipsPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            PiggyInfoAccessoriesPatch.ProcessPiggyInfoAccessories(__instance);
        }
    }

    /// <summary>
    /// Harmony patch on Player.RefreshInfoAccs to enable informational and display accessories located inside the Piggy Bank while paused.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.RefreshInfoAccs))]
    public static class PiggyInfoAccessoriesPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            ProcessPiggyInfoAccessories(__instance);
        }

        /// <summary>
        /// Scans all item slots in the player's Piggy Bank and activates informational and mechanical accessories.
        /// </summary>
        public static void ProcessPiggyInfoAccessories(Player player)
        {
            if (player == null || player.bank?.item == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.InfoAccessoriesInPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(player, mod.Config)) return;

            for (int i = 0; i < player.bank.item.Length; i++)
            {
                Item item = player.bank.item[i];
                if (item == null || item.IsAir || item.type <= 0) continue;

                try
                {
                    player.RefreshInfoAccsFromItemType(item.type);
                    player.RefreshMechanicalAccsFromItemType(item.type);
                }
                catch { }
            }

            try
            {
                player.RefreshInfoAccsFromTeamPlayers();
            }
            catch { }
        }
    }
}
