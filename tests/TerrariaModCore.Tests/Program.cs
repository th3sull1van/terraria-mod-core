using System;
using System.IO;
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
                    string gameDir = @"D:\Jogos\Steam\steamapps\common\Terraria";
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;

                    string[] searchPaths = {
                        Path.Combine(appDir, asmName + ".dll"),
                        Path.Combine(appDir, asmName + ".exe"),
                        Path.Combine(gameDir, asmName + ".dll"),
                        Path.Combine(gameDir, asmName + ".exe"),
                        Path.Combine(appDir, "TMC", asmName + ".dll")
                    };

                    foreach (var p in searchPaths)
                    {
                        if (File.Exists(p)) return Assembly.LoadFrom(p);
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

                    if (terrariaAsm == null && File.Exists(Path.Combine(gameDir, "Terraria.exe")))
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

            // 2. Patch Manager Tests
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
    }
}
