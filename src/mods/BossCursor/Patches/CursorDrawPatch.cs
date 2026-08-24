using System;
using HarmonyLib;
using Terraria;

namespace BossCursor.Patches
{
    /// <summary>
    /// Harmony patch on Main.DrawInterface_36_Cursor to render directional arrows and boss head icons.
    /// </summary>
    [HarmonyPatch(typeof(Main), "DrawInterface_36_Cursor")]
    public static class CursorDrawPatch
    {
        [ThreadStatic]
        private static bool isDrawing;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (isDrawing) return;

            var mod = BossCursorMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled)
            {
                return;
            }

            try
            {
                isDrawing = true;
                BossCursorController.Draw(Main.spriteBatch);
            }
            catch
            {
                // Never crash the game draw loop on render issues
            }
            finally
            {
                isDrawing = false;
            }
        }
    }
}
