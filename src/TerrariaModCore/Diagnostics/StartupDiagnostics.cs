using System;
using System.Collections.Generic;
using System.Text;
using TerrariaModCore.API;

namespace TerrariaModCore.Diagnostics
{
    /// <summary>
    /// Formats and displays startup diagnostics with mod statuses, versions, and error reasons.
    /// </summary>
    public static class StartupDiagnostics
    {
        public static void PrintSummary(string coreVersion, string gameVersion, IReadOnlyList<ModInfo> mods)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine($" TerrariaModCore {coreVersion}");
            sb.AppendLine($" Terraria {gameVersion}");
            sb.AppendLine("========================================");
            sb.AppendLine();
            sb.AppendLine($"Discovered mods: {mods.Count}");
            sb.AppendLine();

            int loadedCount = 0;
            int failedCount = 0;
            int disabledCount = 0;

            foreach (var mod in mods)
            {
                string name = mod.Manifest?.Name ?? "Unknown";
                string version = mod.Manifest?.Version ?? "0.0.0";
                string paddedName = name.PadRight(22);

                if (mod.State == ModState.Loaded)
                {
                    sb.AppendLine($"  [PASS] {paddedName} {version}");
                    loadedCount++;
                }
                else if (mod.State == ModState.Failed)
                {
                    sb.AppendLine($"  [FAIL] {paddedName} {version}");
                    if (!string.IsNullOrEmpty(mod.ErrorDetails))
                    {
                        sb.AppendLine($"         Reason: {mod.ErrorDetails}");
                    }
                    failedCount++;
                }
                else if (mod.State == ModState.Disabled)
                {
                    sb.AppendLine($"  [DISABLED] {paddedName} {version}");
                    disabledCount++;
                }
                else
                {
                    sb.AppendLine($"  [{mod.State.ToString().ToUpper()}] {paddedName} {version}");
                }
            }

            sb.AppendLine();
            if (failedCount == 0)
            {
                sb.AppendLine("All enabled mods loaded successfully.");
            }
            else
            {
                sb.AppendLine($"Finished with {loadedCount} loaded, {failedCount} failed, {disabledCount} disabled.");
            }
            sb.AppendLine("========================================");

            Console.WriteLine(sb.ToString());
        }
    }
}
