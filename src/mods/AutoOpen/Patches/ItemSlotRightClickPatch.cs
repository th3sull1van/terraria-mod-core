using System;
using HarmonyLib;
using Terraria;
using Terraria.UI;

namespace AutoOpen.Patches
{
    /// <summary>
    /// Harmony prefix patch on ItemSlot.RightClick to handle continuous rapid opening
    /// when holding Right-Click on grab bags, crates, and containers.
    /// </summary>
    [HarmonyPatch(typeof(ItemSlot), nameof(ItemSlot.RightClick), new Type[] { typeof(Item[]), typeof(int), typeof(int) })]
    public static class ItemSlotRightClickPatch
    {
        [ThreadStatic]
        private static bool isProcessing;

        [HarmonyPrefix]
        public static bool Prefix(Item[] inv, int context, int slot)
        {
            if (isProcessing) return true;

            var mod = AutoOpenMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.RapidRightClickOpen)
            {
                return true;
            }

            if (inv == null || slot < 0 || slot >= inv.Length)
            {
                return true;
            }

            Item item = inv[slot];
            if (item == null || item.IsAir || item.stack <= 0)
            {
                return true;
            }

            if (!OpenController.IsOpenable(item.type, mod.Config))
            {
                return true;
            }

            if (!Main.mouseRight)
            {
                return true;
            }

            try
            {
                if (Main.LocalPlayerHasPendingInventoryActions())
                {
                    return false;
                }
            }
            catch { }

            Player player = (Main.player != null && Main.myPlayer >= 0 && Main.myPlayer < Main.player.Length)
                ? Main.player[Main.myPlayer]
                : null;

            if (player == null || player.itemAnimation > 0)
            {
                return false;
            }

            try
            {
                isProcessing = true;
                OpenController.ProcessRightClick(inv, context, slot, player, mod.Config);
                return false; // Skip vanilla RightClick so it doesn't split the item into the cursor!
            }
            finally
            {
                isProcessing = false;
            }
        }
    }
}
