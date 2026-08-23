using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TerrariaModCore.Tests
{
    public static class Program
    {
        [ThreadStatic]
        private static bool isResolving = false;
        private static int passed = 0;
        private static int failed = 0;

        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (isResolving) return null;

                try
                {
                    isResolving = true;
                    var asmName = new AssemblyName(e.Name).Name;
                    string gameDir = ResolveGameDirectory();
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;

                    string[] searchPaths = {
                        Path.Combine(appDir, asmName + ".dll"),
                        Path.Combine(appDir, asmName + ".exe"),
                        !string.IsNullOrEmpty(gameDir) ? Path.Combine(gameDir, asmName + ".dll") : null,
                        !string.IsNullOrEmpty(gameDir) ? Path.Combine(gameDir, asmName + ".exe") : null,
                        Path.Combine(appDir, "TMC", asmName + ".dll")
                    };

                    foreach (var p in searchPaths)
                    {
                        if (!string.IsNullOrEmpty(p) && File.Exists(p)) return Assembly.LoadFrom(p);
                    }

                    // Search embedded resources inside Terraria.exe
                    Assembly terrariaAsm = null;
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (string.Equals(asm.GetName().Name, "Terraria", StringComparison.OrdinalIgnoreCase))
                        {
                            terrariaAsm = asm;
                            break;
                        }
                    }

                    if (terrariaAsm == null && !string.IsNullOrEmpty(gameDir) && File.Exists(Path.Combine(gameDir, "Terraria.exe")))
                    {
                        try { terrariaAsm = Assembly.LoadFrom(Path.Combine(gameDir, "Terraria.exe")); } catch { }
                    }

                    if (terrariaAsm != null)
                    {
                        string resourceName = asmName + ".dll";
                        string embedded = Array.Find(terrariaAsm.GetManifestResourceNames(), el => el.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));
                        if (embedded != null)
                        {
                            using (Stream stream = terrariaAsm.GetManifestResourceStream(embedded))
                            {
                                if (stream != null)
                                {
                                    byte[] buf = new byte[stream.Length];
                                    stream.Read(buf, 0, buf.Length);
                                    return Assembly.Load(buf);
                                }
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    isResolving = false;
                }

                return null;
            };
        }

        [STAThread]
        public static void Main(string[] args)
        {
            RealMain();
        }

        private static void RealMain()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("     TerrariaModCore (TMC) Test Suite     ");
            Console.WriteLine("==========================================");

            try
            {
                Terraria.Program.SavePath = AppDomain.CurrentDomain.BaseDirectory;
                Terraria.Program.LaunchParameters = new System.Collections.Generic.Dictionary<string, string>();
            }
            catch { }

            // 1. Dependency Resolver Tests
            DependencyResolverTests.Run(Assert);
            PatchManagerTests.Run(Assert);

            // 3. Configuration & JSON Tests
            ConfigManagerTests.Run(Assert);

            // 4. Fault Isolation Tests
            FaultIsolationTests.Run(Assert);

            // 5. Plugin Logic Tests
            OreCascadePluginTests.Run(Assert);
            AutoFishingPluginTests.Run(Assert);
            FishingLinePlusPluginTests.Run(Assert);
            TurboExtractinatorPluginTests.Run(Assert);
            AutoBuffPluginTests.Run(Assert);
            AutoOpenPluginTests.Run(Assert);
            AutoResearchPluginTests.Run(Assert);
            PiggyVaultPluginTests.Run(Assert);
            TurboBucketPluginTests.Run(Assert);

            // 6. Mod Coexistence Matrix Tests
            ModCoexistenceTests.Run(Assert);

            Console.WriteLine("\n==========================================");
            Console.WriteLine($"RESULTS: {passed} PASSED, {failed} FAILED");
            Console.WriteLine("==========================================");

            if (failed > 0)
            {
                Environment.Exit(1);
            }
        }

        private static void Assert(bool condition, string testName)
        {
            if (condition)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[PASS] {testName}");
                passed++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] {testName}");
                failed++;
            }
            Console.ResetColor();
        }

        private static string ResolveGameDirectory()
        {
            string envPath = Environment.GetEnvironmentVariable("TERRARIA_PATH");
            if (!string.IsNullOrEmpty(envPath) && File.Exists(Path.Combine(envPath, "Terraria.exe")))
            {
                return envPath;
            }

            string[] candidates = {
                @"D:\Jogos\Steam\steamapps\common\Terraria",
                @"C:\Program Files (x86)\Steam\steamapps\common\Terraria",
                @"C:\Program Files\Steam\steamapps\common\Terraria",
                @"C:\GOG Games\Terraria",
                @"D:\GOG Games\Terraria",
                @"E:\Steam\steamapps\common\Terraria",
                @"E:\Jogos\Steam\steamapps\common\Terraria"
            };

            foreach (var path in candidates)
            {
                if (File.Exists(Path.Combine(path, "Terraria.exe")))
                {
                    return path;
                }
            }

            return candidates[0];
        }
    }
}
