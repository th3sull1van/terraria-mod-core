using System;

namespace TerrariaModCore
{
    /// <summary>
    /// Static entry point called during launcher bootstrap before vanilla game startup.
    /// </summary>
    public static class CoreBootstrap
    {
        private static ModEngine _engine;

        /// <summary>
        /// Gets the active TMC Host Engine instance.
        /// </summary>
        public static ModEngine Engine => _engine;

        /// <summary>
        /// Initializes the TerrariaModCore Host and loads all discovered plugins.
        /// </summary>
        /// <param name="baseDirectory">Root installation directory containing Terraria.exe and /mods.</param>
        public static void Initialize(string baseDirectory = null)
        {
            try
            {
                _engine = new ModEngine(baseDirectory);
                _engine.InitializeAndLoadAll();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[TMC CRITICAL] Fatal error during CoreBootstrap initialization: {ex}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Safely shuts down the mod host and unloads all plugins.
        /// </summary>
        public static void Shutdown()
        {
            _engine?.Shutdown();
            _engine = null;
        }
    }
}
