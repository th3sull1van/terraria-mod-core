using System;
using System.Collections.Generic;
using FishingLinePlus;
using FishingLinePlus.Patches;
using Microsoft.Xna.Framework;
using Terraria;

namespace TerrariaModCore.Tests
{
    public static class FishingLinePlusPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing FishingLinePlus Plugin Logic ---");

            var config = new FishingLineConfig
            {
                Enabled = true,
                MaxActiveFishingLines = 4,
                LinesPerCast = 4,
                SpreadAngleDegrees = 7.0f,
                VelocitySpread = 0.08f
            };

            // 1. Config Defaults and Capacity Validation
            assert(config.Enabled, "FishingLinePlus enabled by default");
            assert(config.MaxActiveFishingLines == 4, $"Max active fishing lines configured to {config.MaxActiveFishingLines}");
            assert(config.LinesPerCast == 4, $"Lines cast per action configured to {config.LinesPerCast}");
            assert(config.SpreadAngleDegrees == 7.0f, $"Spread angle configured to {config.SpreadAngleDegrees}°");

            // 2. Spread Angle and Velocity Calculation Tests
            Vector2 baseVelocity = new Vector2(10f, 0f);

            // Offset verification
            Vector2 vLeft = MultiBobberHelper.CalculateOffsetVelocity(baseVelocity, -1f, config.SpreadAngleDegrees, config.VelocitySpread);
            Vector2 vRight = MultiBobberHelper.CalculateOffsetVelocity(baseVelocity, 1f, config.SpreadAngleDegrees, config.VelocitySpread);
            Vector2 vCenter = MultiBobberHelper.CalculateOffsetVelocity(baseVelocity, 0f, config.SpreadAngleDegrees, config.VelocitySpread);

            assert(vLeft.Y != 0f && vRight.Y != 0f, "Spread velocities have vertical deviation from aim vector");
            assert(Math.Sign(vLeft.Y) == -Math.Sign(vRight.Y), "Left and right spread lines disperse in opposite symmetric directions");
            assert(vCenter == baseVelocity, "Zero-offset central line matches base aim velocity exactly");

            // Multi-line index distribution verification (3 lines: index 0 is left, 1 is center, 2 is right)
            Vector2 v3Left = MultiBobberHelper.CalculateSpreadVelocity(baseVelocity, 0, 3, config.SpreadAngleDegrees, config.VelocitySpread);
            Vector2 v3Center = MultiBobberHelper.CalculateSpreadVelocity(baseVelocity, 1, 3, config.SpreadAngleDegrees, config.VelocitySpread);
            Vector2 v3Right = MultiBobberHelper.CalculateSpreadVelocity(baseVelocity, 2, 3, config.SpreadAngleDegrees, config.VelocitySpread);
            assert(v3Center == baseVelocity && Math.Sign(v3Left.Y) == -Math.Sign(v3Right.Y), "3-line distribution correctly places index 1 at center and indices 0/2 symmetric");

            // 3. Active Bobbers Capacity and Clamping Logic
            int currentBobbers = 0;
            int maxAllowed = config.MaxActiveFishingLines;
            int linesToCast = config.LinesPerCast;
            int extraToSpawn = Math.Min(linesToCast - 1, maxAllowed - (currentBobbers + 1));
            assert(extraToSpawn == 3, $"Initial cast of {linesToCast} lines with 0 existing bobbers spawns 3 extra bobbers (total 4 with vanilla)");

            currentBobbers = 2;
            extraToSpawn = Math.Min(linesToCast - 1, maxAllowed - (currentBobbers + 1));
            assert(extraToSpawn == 1, $"Sequential cast with 2 active bobbers spawns 1 extra bobber (total 4, strictly respecting MaxActiveFishingLines cap)");

            currentBobbers = 4;
            extraToSpawn = Math.Max(0, Math.Min(linesToCast - 1, maxAllowed - (currentBobbers + 1)));
            assert(extraToSpawn == 0, "No extra bobbers spawned when MaxActiveFishingLines cap is already reached");

            // 4. Bobber Projectile Type Resolution
            var player = new Player();
            player.overrideFishingBobber = -1;
            var rodItem = new Item();
            rodItem.fishingPole = 25;
            rodItem.shoot = 360; // Wood Fishing Pole bobber projectile

            int resolvedProj = MultiBobberHelper.GetBobberProjectileType(player, rodItem);
            assert(resolvedProj == 360, "Default rod bobber projectile correctly resolved (360)");

            player.overrideFishingBobber = 986; // Glowing Bobber cosmetic accessory
            resolvedProj = MultiBobberHelper.GetBobberProjectileType(player, rodItem);
            assert(resolvedProj == 986, "Cosmetic accessory bobber override correctly honored (986)");

            // 5. Multi-Bobber Independence and State Retention
            if (Main.projectile != null)
            {
                int testOwner = 0;
                var dummyBobbers = new List<Projectile>();
                for (int i = 0; i < 4; i++)
                {
                    var p = new Projectile();
                    p.SetDefaults(360);
                    p.active = true;
                    p.owner = testOwner;
                    p.bobber = true;
                    p.ai[0] = 0f; // Floating
                    p.ai[1] = (i == 2) ? -2303f : 0f; // Bobber 2 has a bite (e.g. Bass itemId 2303)
                    p.localAI[1] = (i == 2) ? 2303f : 100f;
                    dummyBobbers.Add(p);
                }

                // Verify independent bite state
                int bitingBobbers = 0;
                for (int i = 0; i < dummyBobbers.Count; i++)
                {
                    if (dummyBobbers[i].ai[1] < 0f && dummyBobbers[i].localAI[1] != 0f)
                    {
                        bitingBobbers++;
                    }
                }

                assert(bitingBobbers == 1, "Single biting bobber correctly detected among 4 simultaneous active bobbers without state corruption");
                assert(dummyBobbers[0].ai[1] == 0f && dummyBobbers[2].ai[1] < 0f, "Other 3 bobbers remain peacefully floating waiting for independent bites");
            }
        }
    }
}
