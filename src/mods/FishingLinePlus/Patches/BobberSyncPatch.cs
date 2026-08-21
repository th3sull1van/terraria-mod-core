using System;
using System.Reflection;
using HarmonyLib;
using Terraria;

namespace FishingLinePlus.Patches
{
    /// <summary>
    /// Harmony patch targeting Projectile.AI_061_FishingBobber.
    /// When any bobber gets a bite (ai[1] < 0), synchronizes all sibling bobbers in water to also roll catches
    /// and show visual bite animations simultaneously.
    /// </summary>
    [HarmonyPatch(typeof(Projectile), "AI_061_FishingBobber")]
    public static class BobberSyncPatch
    {
        private static readonly MethodInfo FishingCheckMethod = typeof(Projectile).GetMethod(
            "FishingCheck",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        );

        [ThreadStatic]
        private static bool _isSyncing;

        [HarmonyPostfix]
        public static void Postfix(Projectile __instance)
        {
            if (_isSyncing) return;

            var mod = FishingLinePlusMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;
            if (__instance == null || !__instance.active || !__instance.bobber) return;

            // Check if THIS bobber just got a bite in water (ai[0] == 0f, ai[1] < 0f, localAI[1] != 0f)
            if (__instance.ai[0] == 0f && __instance.wet && __instance.ai[1] < 0f && __instance.localAI[1] != 0f)
            {
                int owner = __instance.owner;
                if (owner < 0 || owner >= Main.maxPlayers) return;

                try
                {
                    _isSyncing = true;
                    if (FishingCheckMethod != null)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile sibling = Main.projectile[i];
                            if (sibling != null && sibling.active && sibling.owner == owner && sibling.bobber && sibling.whoAmI != __instance.whoAmI && sibling.ai[0] == 0f && sibling.wet && sibling.ai[1] >= 0f)
                            {
                                int attempts = 0;
                                while (sibling.ai[1] >= 0f && attempts < 15)
                                {
                                    try
                                    {
                                        FishingCheckMethod.Invoke(sibling, null);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    attempts++;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _isSyncing = false;
                }
            }
        }
    }
}
