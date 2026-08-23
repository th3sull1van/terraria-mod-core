using System;
using HarmonyLib;
using Terraria;

namespace AutoResearch.Patches
{
    /// <summary>
    /// Harmony postfix patch on Player.Update to execute continuous background auto-researching
    /// of inventory, cursor, and void vault items when AutoResearchInventory is enabled.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class PlayerResearchPatch
    {
        [ThreadStatic]
        private static bool isUpdating;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            if (isUpdating) return;

            var mod = AutoResearchMod.Instance;
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
                ResearchController.UpdateInventoryScan(__instance, mod.Config);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
}
