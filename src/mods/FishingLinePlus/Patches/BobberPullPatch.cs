using System;
using System.Reflection;
using HarmonyLib;
using Terraria;

namespace FishingLinePlus.Patches
{
    /// <summary>
    /// Harmony patch targeting Player.ItemCheck_PullFishingBobbers.
    /// When any bobber gets a bite and the player reels in, this patch ensures all other active floating
    /// bobbers in the water also roll a FishingCheck, allowing all simultaneous lines to catch fish!
    /// </summary>
    [HarmonyPatch(typeof(Player), "ItemCheck_PullFishingBobbers")]
    public static class BobberPullPatch
    {
        private static readonly MethodInfo FishingCheckMethod = typeof(Projectile).GetMethod(
            "FishingCheck",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        );

        [HarmonyPrefix]
        public static void Prefix(Player __instance)
        {
            var mod = FishingLinePlusMod.Instance;
            if (mod == null || mod.Config == null || !mod.Config.Enabled) return;
            if (__instance == null || __instance.whoAmI < 0 || __instance.whoAmI >= Main.maxPlayers) return;

            // Check if AT LEAST ONE bobber is in bite state (ai[0] == 0f floating, ai[1] < 0f && localAI[1] != 0f)
            bool anyBiting = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p != null && p.active && p.owner == __instance.whoAmI && p.bobber && p.ai[0] == 0f && p.wet && p.ai[1] < 0f && p.localAI[1] != 0f)
                {
                    anyBiting = true;
                    break;
                }
            }

            // If at least one line has hooked a fish and the lines are being reeled in:
            // Trigger FishingCheck on all other floating bobbers in water so every line gets its catch!
            if (anyBiting && FishingCheckMethod != null)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p != null && p.active && p.owner == __instance.whoAmI && p.bobber && p.ai[0] == 0f && p.wet && p.ai[1] >= 0f)
                    {
                        int attempts = 0;
                        while (p.ai[1] >= 0f && attempts < 15)
                        {
                            try
                            {
                                FishingCheckMethod.Invoke(p, null);
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
    }
}
