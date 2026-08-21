using System;

namespace TurboExtractinator
{
    /// <summary>
    /// Configuration settings for the TurboExtractinator plugin.
    /// </summary>
    public class TurboExtractConfig
    {
        /// <summary>
        /// Enables or disables the TurboExtractinator speed boost.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Speed multiplier for extraction cycles (default is 5, meaning 5x faster).
        /// Value is clamped between 1 and 60.
        /// </summary>
        public int SpeedMultiplier { get; set; } = 5;

        /// <summary>
        /// Whether the speed multiplier also applies to the Chlorophyte Extractinator.
        /// </summary>
        public bool AffectsChlorophyteExtractinator { get; set; } = true;

        /// <summary>
        /// Extra items to process per single extraction tick when SpeedMultiplier exceeds item cooldown.
        /// Value is clamped between 1 and 50.
        /// </summary>
        public int BatchExtractionSize { get; set; } = 1;
    }
}
