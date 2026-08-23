using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Terraria;

namespace PiggyVault.Patches
{
    /// <summary>
    /// Harmony patch on Recipe.CollectItemsFromChests to include the player's Piggy Bank when finding available crafting materials.
    /// </summary>
    [HarmonyPatch(typeof(Recipe), "CollectItemsFromChests")]
    public static class PiggyCraftingPatch
    {
        private static readonly FieldInfo RecipeChestsField = AccessTools.Field(typeof(Recipe), "_recipeChests");
        private static readonly MethodInfo CollectItemsMethod = AccessTools.Method(typeof(Recipe), "CollectItems", new Type[] { typeof(Item[]), typeof(int) });

        [HarmonyPostfix]
        public static void Postfix(Player player)
        {
            if (player == null || player.bank == null || player.bank.item == null) return;

            var mod = PiggyVaultMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled || !mod.Config.CraftFromPiggyBank) return;
            if (!PiggyVaultController.IsPiggyBankUsable(player, mod.Config)) return;

            // If player.chest is currently -2, the Piggy Bank is already open and in _recipeChests
            if (player.chest == -2) return;

            try
            {
                var recipeChests = RecipeChestsField?.GetValue(null) as List<Chest>;
                if (recipeChests != null && !recipeChests.Contains(player.bank))
                {
                    recipeChests.Add(player.bank);
                    int max = player.bank.maxItems > 0 ? player.bank.maxItems : player.bank.item.Length;
                    CollectItemsMethod?.Invoke(null, new object[] { player.bank.item, max });
                }
            }
            catch
            {
                // Fallback for test harness
            }
        }
    }
}
