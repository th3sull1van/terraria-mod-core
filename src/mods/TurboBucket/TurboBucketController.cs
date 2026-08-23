using System;
using Terraria;
using Terraria.ID;

namespace TurboBucket
{
    /// <summary>
    /// Core controller handling logic, item validation, and execution speed adjustment for buckets.
    /// </summary>
    public static class TurboBucketController
    {
        /// <summary>
        /// Checks whether the specified item is an eligible liquid bucket or liquid tool based on config.
        /// </summary>
        public static bool IsTargetLiquidBucket(int itemType, TurboBucketConfig config)
        {
            if (config == null || !config.Enabled || itemType <= ItemID.None)
            {
                return false;
            }

            // Standard Liquid Buckets
            if (itemType == ItemID.WaterBucket && config.AffectsWater) return true;
            if (itemType == ItemID.LavaBucket && config.AffectsLava) return true;
            if (itemType == ItemID.HoneyBucket && config.AffectsHoney) return true;

            // Bottomless Buckets
            if (config.AffectsBottomlessBuckets)
            {
                if (itemType == ItemID.BottomlessBucket && config.AffectsWater) return true;
                if (itemType == ItemID.BottomlessLavaBucket && config.AffectsLava) return true;
                if (itemType == ItemID.BottomlessHoneyBucket && config.AffectsHoney) return true;
                if (itemType == ItemID.BottomlessShimmerBucket) return true;
            }

            // Empty Bucket (Draining)
            if (config.AffectsEmptyBuckets && itemType == ItemID.EmptyBucket)
            {
                return true;
            }

            // Sponges
            if (config.AffectsSponges)
            {
                if (itemType == ItemID.SuperAbsorbantSponge) return true;
                if (itemType == ItemID.LavaAbsorbantSponge) return true;
                if (itemType == ItemID.HoneyAbsorbantSponge) return true;
                if (itemType == ItemID.UltraAbsorbantSponge) return true;
            }

            return false;
        }

        /// <summary>
        /// Applies the speed boost by scaling down itemTime and itemAnimation cooldowns.
        /// </summary>
        public static void ApplySpeedBoost(Player player, Item item, TurboBucketConfig config)
        {
            if (player == null || item == null || config == null || !config.Enabled)
            {
                return;
            }

            if (!IsTargetLiquidBucket(item.type, config))
            {
                return;
            }

            int multiplier = Math.Max(1, Math.Min(10, config.SpeedMultiplier));
            if (multiplier <= 1)
            {
                return;
            }

            if (player.itemTime > 0)
            {
                int newTime = Math.Max(1, player.itemTime / multiplier);
                player.SetItemTime(newTime);
            }

            if (player.itemAnimation > 0)
            {
                int newAnim = Math.Max(1, player.itemAnimation / multiplier);
                player.itemAnimation = newAnim;
                player.itemAnimationMax = Math.Max(1, player.itemAnimationMax / multiplier);
            }
        }
    }
}
