using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace FishingLinePlus.Patches
{
    /// <summary>
    /// Utility methods for multi-bobber calculations, physics spread, and lifecycle tracking.
    /// </summary>
    public static class MultiBobberHelper
    {
        /// <summary>
        /// Counts all currently active fishing bobber projectiles owned by the given player.
        /// </summary>
        public static int CountActiveBobbers(int playerIndex)
        {
            if (Main.projectile == null || playerIndex < 0 || playerIndex >= Main.maxPlayers)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p != null && p.active && p.owner == playerIndex && p.bobber)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets all active bobber projectiles owned by the given player.
        /// </summary>
        public static List<Projectile> GetActiveBobbers(int playerIndex)
        {
            var list = new List<Projectile>();
            if (Main.projectile == null || playerIndex < 0 || playerIndex >= Main.maxPlayers)
            {
                return list;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p != null && p.active && p.owner == playerIndex && p.bobber)
                {
                    list.Add(p);
                }
            }
            return list;
        }

        /// <summary>
        /// Resolves the bobber projectile type to shoot for the player and rod item.
        /// </summary>
        public static int GetBobberProjectileType(Player player, Item rod)
        {
            if (player != null && player.overrideFishingBobber > -1)
            {
                return player.overrideFishingBobber;
            }
            return rod != null ? rod.shoot : 0;
        }

        /// <summary>
        /// Calculates a spread velocity vector for a multi-bobber cast using 0-based line index and total lines.
        /// Distributes lines evenly across [-totalSpread/2, +totalSpread/2] with subtle speed variation.
        /// </summary>
        public static Vector2 CalculateSpreadVelocity(Vector2 baseVelocity, int lineIndex, int totalLines, float spreadDegrees, float velocitySpread)
        {
            if (totalLines <= 1 || baseVelocity == Vector2.Zero)
            {
                return baseVelocity;
            }

            float step = lineIndex - (totalLines - 1) * 0.5f;
            return CalculateOffsetVelocity(baseVelocity, step, spreadDegrees, velocitySpread);
        }

        /// <summary>
        /// Calculates a spread velocity vector offset by a specific number of step units.
        /// offsetSteps = 0 gives baseVelocity; negative/positive steps disperse symmetrically left/right.
        /// </summary>
        public static Vector2 CalculateOffsetVelocity(Vector2 baseVelocity, float offsetSteps, float spreadDegrees, float velocitySpread)
        {
            if (offsetSteps == 0f || baseVelocity == Vector2.Zero)
            {
                return baseVelocity;
            }

            float angleRadians = (float)(offsetSteps * spreadDegrees * (Math.PI / 180.0));
            float speedFactor = 1.0f + (offsetSteps * velocitySpread * 0.5f);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);
            Vector2 rotated = new Vector2(
                baseVelocity.X * cos - baseVelocity.Y * sin,
                baseVelocity.X * sin + baseVelocity.Y * cos
            ) * speedFactor;

            return rotated;
        }

        /// <summary>
        /// Spawns additional bobber projectiles for a multi-line cast up to configured limits.
        /// </summary>
        public static int SpawnExtraBobbers(Player player, Item sItem, int weaponDamage, FishingLineConfig config)
        {
            if (player == null || sItem == null || config == null || !config.Enabled)
            {
                return 0;
            }

            int currentCount = CountActiveBobbers(player.whoAmI);
            int maxAllowed = Math.Max(1, Math.Min(30, config.MaxActiveFishingLines));
            int linesToCast = Math.Max(1, Math.Min(config.LinesPerCast, maxAllowed));

            // Vanilla has already spawned 1 bobber during ItemCheck_Shoot
            // We spawn (linesToCast - 1) extra bobbers, respecting the maxActiveFishingLines ceiling
            int availableSlots = maxAllowed - currentCount;
            int extraToSpawn = Math.Min(linesToCast - 1, availableSlots);

            if (extraToSpawn <= 0)
            {
                return 0;
            }

            int projType = GetBobberProjectileType(player, sItem);
            if (projType <= 0)
            {
                return 0;
            }

            Vector2 mountedCenter = player.MountedCenter;
            Vector2 spawnPos = player.RotatedRelativePoint(mountedCenter);
            Vector2 aimVector = Main.MouseWorld - spawnPos;
            if (aimVector != Vector2.Zero)
            {
                aimVector.Normalize();
            }
            Vector2 baseVelocity = aimVector * sItem.shootSpeed;

            IEntitySource source = new EntitySource_ItemUse(player, sItem);
            int spawnedCount = 0;

            for (int i = 0; i < extraToSpawn; i++)
            {
                // Assign alternate symmetric offsets: slot +1, -1, +2, -2, etc.
                float slot = (i % 2 == 0) ? ((i / 2) + 1f) : -((i / 2) + 1f);
                Vector2 velocity = CalculateOffsetVelocity(baseVelocity, slot, config.SpreadAngleDegrees, config.VelocitySpread);

                int projIndex = Projectile.NewProjectile(
                    source,
                    spawnPos.X,
                    spawnPos.Y,
                    velocity.X,
                    velocity.Y,
                    projType,
                    0,
                    sItem.knockBack,
                    player.whoAmI
                );

                if (projIndex >= 0 && projIndex < Main.maxProjectiles)
                {
                    spawnedCount++;
                }
            }

            return spawnedCount;
        }
    }
}
