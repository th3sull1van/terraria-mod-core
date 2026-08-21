using System;

namespace AutoFishing
{
    /// <summary>
    /// Configuration model for AutoFishing plugin.
    /// </summary>
    public class AutoFishingConfig
    {
        public bool Enabled { get; set; } = true;
        public bool AutoCast { get; set; } = true;
        public bool AutoReel { get; set; } = true;
        public int CastDelayTicks { get; set; } = 30; // 0.5s at 60 FPS
        public int ReelDelayTicks { get; set; } = 2;   // Small natural reaction delay
        public bool RequireBait { get; set; } = true;
    }
}
