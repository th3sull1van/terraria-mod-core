using System;
using HarmonyLib;
using Terraria;

namespace AutoResearch.Patches
{
    /// <summary>
    /// Harmony prefix patch on Player.GetItem to automatically research items
    /// immediately when picked up or received into inventory.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.GetItem))]
    public static class PlayerGetItemPatch
    {
        [ThreadStatic]
        private static bool isProcessing;

        [HarmonyPrefix]
        public static bool Prefix(Player __instance, Item newItem, GetItemSettings settings, ref Item __result)
        {
            if (isProcessing) return true;

            var mod = AutoResearchMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.AutoResearchOnPickup)
            {
                return true;
            }

            if (__instance == null || __instance.whoAmI != Main.myPlayer || newItem == null || newItem.IsAir)
            {
                return true;
            }

            try
            {
                isProcessing = true;
                bool sacrificed = ResearchController.ProcessGetItem(__instance, newItem, mod.Config);

                // If the entire stack was consumed for research, skip vanilla GetItem
                if (sacrificed && (newItem.IsAir || newItem.stack <= 0))
                {
                    __result = new Item();
                    return false;
                }
            }
            finally
            {
                isProcessing = false;
            }

            return true;
        }
    }
}
