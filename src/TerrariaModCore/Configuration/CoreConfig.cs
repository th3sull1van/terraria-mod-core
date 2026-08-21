using System;
using TerrariaModCore.API;

namespace TerrariaModCore.Configuration
{
    /// <summary>
    /// Global TMC Host Engine configuration settings.
    /// </summary>
    public class CoreConfig
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        public bool DiagnosticBannerOnStartup { get; set; } = true;
        public bool StrictCompatibilityCheck { get; set; } = true;
        public bool SafeModeOnModFailure { get; set; } = true;
        public string ModsDirectoryName { get; set; } = "mods";
    }
}
