using System;
using HarmonyLib;
using Terraria;

namespace OreCascade.Patches
{
    /// <summary>
    /// Harmony patch targeting Player.PickTile to trigger cascade mining on ore destruction.
    /// Registered through TMC Centralized PatchManager.
    /// </summary>
    [HarmonyPatch(typeof(Player), nameof(Player.PickTile))]
    public static class MiningPatch
    {
        public struct PickContext
        {
            public bool WasActive;
            public ushort TileType;
            public bool IsEligible;
        }

        [HarmonyPrefix]
        public static void Prefix(Player __instance, int x, int y, int pickPower, out PickContext __state)
        {
            __state = default;

            var config = OreCascadeMod.Instance?.Config;
            if (CascadeMiner.IsCascading || config == null || !config.Enabled)
            {
                return;
            }

            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld || Main.tile == null || __instance == null)
            {
                return;
            }

            if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
            {
                return;
            }

            Tile tile = Main.tile[x, y];
            if (tile != null && tile.active())
            {
                ushort type = tile.type;
                if (OreClassifier.IsEligible(type, config))
                {
                    __state.WasActive = true;
                    __state.TileType = type;
                    __state.IsEligible = true;
                }
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Player __instance, int x, int y, int pickPower, PickContext __state)
        {
            var config = OreCascadeMod.Instance?.Config;
            if (CascadeMiner.IsCascading || !__state.IsEligible || config == null || !config.Enabled)
            {
                return;
            }

            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld || Main.tile == null || __instance == null)
            {
                return;
            }

            if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
            {
                return;
            }

            Tile tile = Main.tile[x, y];
            bool isDestroyed = (tile == null || !tile.active());

            if (__state.WasActive && isDestroyed)
            {
                CascadeMiner.ExecuteCascade(__instance, x, y, __state.TileType, pickPower, config);
            }
        }
    }
}
