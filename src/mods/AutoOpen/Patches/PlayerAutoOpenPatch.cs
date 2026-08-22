using System;
using HarmonyLib;
using Terraria;

namespace AutoOpen.Patches
{
    /// <summary>
    /// Harmony postfix patch on Player.Update to execute background auto-unpacking
    /// when AutoOpenInventory is enabled.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class PlayerAutoOpenPatch
    {
        [ThreadStatic]
        private static bool isUpdating;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            if (isUpdating) return;

            var mod = AutoOpenMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.AutoOpenInventory)
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
                OpenController.UpdateInventoryAutoOpen(__instance, mod.Config);
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
}
