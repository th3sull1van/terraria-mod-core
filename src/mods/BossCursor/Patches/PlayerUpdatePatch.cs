using System;
using HarmonyLib;
using Terraria;

namespace BossCursor.Patches
{
    /// <summary>
    /// Harmony patch on Player.Update to monitor hotkey toggling for the local player.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class PlayerUpdatePatch
    {
        [ThreadStatic]
        private static bool isUpdating;

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int i)
        {
            if (isUpdating) return;

            var mod = BossCursorMod.Instance;
            if (mod == null || mod.Config == null)
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
                BossCursorController.UpdateInput();
            }
            catch
            {
            }
            finally
            {
                isUpdating = false;
            }
        }
    }
}
