using System;

namespace TurboBucket
{
    /// <summary>
    /// Configuration settings for the TurboBucket mod.
    /// </summary>
    public class TurboBucketConfig
    {
        /// <summary>
        /// Enables or disables bucket pouring acceleration.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Speed multiplier for bucket pouring and liquid actions (1 to 10).
        /// Default is 5 (delay reduced from 10 ticks to 2 ticks = 30 pours/second).
        /// Value 10 achieves native 60 TPS (1 pour per frame).
        /// </summary>
        public int SpeedMultiplier { get; set; } = 5;

        /// <summary>
        /// Accelerate Water Bucket pouring.
        /// </summary>
        public bool AffectsWater { get; set; } = true;

        /// <summary>
        /// Accelerate Lava Bucket pouring.
        /// </summary>
        public bool AffectsLava { get; set; } = true;

        /// <summary>
        /// Accelerate Honey Bucket pouring.
        /// </summary>
        public bool AffectsHoney { get; set; } = true;

        /// <summary>
        /// Accelerate Bottomless liquid buckets (Water, Lava, Honey, Shimmer).
        /// </summary>
        public bool AffectsBottomlessBuckets { get; set; } = true;

        /// <summary>
        /// Accelerate empty bucket liquid gathering.
        /// </summary>
        public bool AffectsEmptyBuckets { get; set; } = false;

        /// <summary>
        /// Accelerate sponge liquid absorption.
        /// </summary>
        public bool AffectsSponges { get; set; } = false;
    }
}
