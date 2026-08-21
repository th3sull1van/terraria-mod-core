using System;
using System.Reflection;
using TerrariaModCore.API;

namespace TerrariaModCore.Compatibility
{
    /// <summary>
    /// Validates runtime Terraria assembly identity and version compatibility.
    /// </summary>
    public static class GameVersionChecker
    {
        public const string TargetTerrariaVersion = "1.4.5.7";

        public static bool ValidateTerrariaVersion(out string detectedVersion)
        {
            try
            {
                Assembly terrariaAsm = null;
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Equals(asm.GetName().Name, "Terraria", StringComparison.OrdinalIgnoreCase))
                    {
                        terrariaAsm = asm;
                        break;
                    }
                }

                if (terrariaAsm == null)
                {
                    try
                    {
                        // Safe reflection without triggering early static init of Main
                        Type progType = Type.GetType("Terraria.Program, Terraria");
                        if (progType != null) terrariaAsm = progType.Assembly;
                    }
                    catch { }
                }

                if (terrariaAsm == null)
                {
                    detectedVersion = "Terraria assembly not loaded";
                    return false;
                }

                Version asmVer = terrariaAsm.GetName().Version;
                detectedVersion = asmVer != null ? $"{asmVer.Major}.{asmVer.Minor}.{asmVer.Build}.{asmVer.Revision}" : "Unknown";

                if (detectedVersion.StartsWith(TargetTerrariaVersion, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var fileVerAttr = terrariaAsm.GetCustomAttribute<AssemblyFileVersionAttribute>();
                if (fileVerAttr != null && fileVerAttr.Version.StartsWith(TargetTerrariaVersion, StringComparison.OrdinalIgnoreCase))
                {
                    detectedVersion = fileVerAttr.Version;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                detectedVersion = "Error: " + ex.Message;
                return false;
            }
        }
    }
}
