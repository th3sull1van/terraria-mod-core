using System;
using System.Reflection;
using TerrariaModCore.API;

namespace AutoOpen
{
    /// <summary>
    /// AutoOpen plugin entry point implementing TMC IMod lifecycle.
    /// Accelerates and automates grab bag, crate, and container opening with zero file modification.
    /// </summary>
    public class AutoOpenMod : IMod
    {
        public static AutoOpenMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public AutoOpenConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<AutoOpenConfig>();
            context.Logger.Info($"AutoOpen initialized (Enabled: {Config.Enabled}, RapidRightClick: {Config.RapidRightClickOpen}, Delay: {Config.OpenDelayTicks} ticks, BatchSize: {Config.BatchSize})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Manifest.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            OpenController.Reset();
            Context?.Logger?.Info("AutoOpen loaded and active.");
        }

        public void Unload()
        {
            OpenController.Reset();
            Context?.Logger?.Info("AutoOpen unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
