using System;

namespace FishingLinePlus
{
    /// <summary>
    /// Configuration model for FishingLinePlus plugin.
    /// Controls simultaneous active fishing lines, lines cast per action, and spread physics.
    /// </summary>
    public class FishingLineConfig
    {
        /// <summary>
        /// Whether the FishingLinePlus mod is active.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Maximum total number of active bobbers/lines allowed simultaneously for the player.
        /// Range: 1 to 30. Default: 4.
        /// </summary>
        public int MaxActiveFishingLines { get; set; } = 4;

        /// <summary>
        /// Number of fishing lines/bobbers spawned per fishing pole cast.
        /// Range: 1 to 30. Default: 4.
        /// </summary>
        public int LinesPerCast { get; set; } = 4;

        /// <summary>
        /// Angular spread in degrees between multiple bobber projectiles when cast.
        /// Default: 7.0 degrees.
        /// </summary>
        public float SpreadAngleDegrees { get; set; } = 7.0f;

        /// <summary>
        /// Velocity variance factor (0.0 to 0.3) applied across lines so bobbers disperse naturally.
        /// Default: 0.08 (8%).
        /// </summary>
        public float VelocitySpread { get; set; } = 0.08f;
    }
}
