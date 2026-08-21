using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TerrariaModCore;

namespace TerrariaModCore.Launcher
{
    /// <summary>
    /// Standalone launcher entry point for Terraria with TerrariaModCore.
    /// Configures global assembly resolution, initializes TMC Host engine, loads mods, and starts vanilla Terraria.
    /// </summary>
    internal static class Program
    {
        [ThreadStatic]
        private static bool isResolving;

        static Program()
        {
            // Global Exception Handlers
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try
                {
                    string crashLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TMC", "logs", "crash.log");
                    string dir = Path.GetDirectoryName(crashLog);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.AppendAllText(crashLog, $"[{DateTime.Now}] Unhandled Exception:\n{e.ExceptionObject}\n\n");
                }
                catch { }
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                try
                {
                    string crashLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TMC", "logs", "crash.log");
                    string dir = Path.GetDirectoryName(crashLog);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.AppendAllText(crashLog, $"[{DateTime.Now}] Unobserved Task Exception:\n{e.Exception}\n\n");
                }
                catch { }
            };

            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch { }

            // Assembly Resolver configured in static constructor BEFORE Main() is JITted
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                if (isResolving) return null;

                try
                {
                    isResolving = true;
                    var asmName = new AssemblyName(resolveArgs.Name).Name;
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;

                    // Direct lookups in Game Root, TMC/, and TMC/libs
                    string[] searchPaths = {
                        Path.Combine(appDir, asmName + ".dll"),
                        Path.Combine(appDir, asmName + ".exe"),
                        Path.Combine(appDir, "TMC", asmName + ".dll"),
                        Path.Combine(appDir, "TMC", "libs", asmName + ".dll")
                    };

                    foreach (var path in searchPaths)
                    {
                        if (File.Exists(path))
                        {
                            return Assembly.LoadFrom(path);
                        }
                    }

                    // Special case for Terraria executable
                    if (string.Equals(asmName, "Terraria", StringComparison.OrdinalIgnoreCase))
                    {
                        string terrariaExe = Path.Combine(appDir, "Terraria.exe");
                        if (File.Exists(terrariaExe))
                        {
                            return Assembly.LoadFrom(terrariaExe);
                        }
                    }

                    // Search embedded resources in loaded Terraria assembly
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
                        string terrariaExe = Path.Combine(appDir, "Terraria.exe");
                        if (File.Exists(terrariaExe))
                        {
                            try { terrariaAsm = Assembly.LoadFrom(terrariaExe); } catch { }
                        }
                    }

                    if (terrariaAsm != null)
                    {
                        string resourceName = asmName + ".dll";
                        string embedded = Array.Find(terrariaAsm.GetManifestResourceNames(), element => element.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));
                        if (embedded != null)
                        {
                            using (Stream stream = terrariaAsm.GetManifestResourceStream(embedded))
                            {
                                if (stream != null)
                                {
                                    byte[] buffer = new byte[stream.Length];
                                    stream.Read(buffer, 0, buffer.Length);
                                    return Assembly.Load(buffer);
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
        private static void Main(string[] args)
        {
            RealMain(args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RealMain(string[] args)
        {
            // Pre-initialize Terraria.Program.SavePath to prevent early Main..cctor() NullReference
            try
            {
                string defaultSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria");
                Terraria.Program.SavePath = defaultSavePath;
                if (Terraria.Program.LaunchParameters == null)
                {
                    Terraria.Program.LaunchParameters = new Dictionary<string, string>();
                }
            }
            catch { }

            // Bootstrap TMC Host and Load Plugins
            try
            {
                CoreBootstrap.Initialize(AppDomain.CurrentDomain.BaseDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize TerrariaModCore:\n\n{ex}", "TMC Core Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Launch Vanilla Terraria
            try
            {
                var terrariaAsm = typeof(Terraria.Program).Assembly;
                var windowsLaunchType = terrariaAsm.GetType("Terraria.WindowsLaunch");
                if (windowsLaunchType != null)
                {
                    var mainMethod = windowsLaunchType.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (mainMethod != null)
                    {
                        mainMethod.Invoke(null, new object[] { args });
                        return;
                    }
                }

                Terraria.Program.LaunchGame(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch Terraria:\n\n{ex}", "Terraria Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
