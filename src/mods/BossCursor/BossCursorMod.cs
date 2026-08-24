using System;
using System.Reflection;
using TerrariaModCore.API;

namespace BossCursor
{
    /// <summary>
    /// BossCursor plugin entry point implementing TMC IMod lifecycle.
    /// Draws directional arrows and boss head icons pointing toward active bosses.
    /// </summary>
    public class BossCursorMod : IMod
    {
        public static BossCursorMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public BossCursorConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<BossCursorConfig>();
            context.Logger.Info($"BossCursor initialized (Enabled: {Config.Enabled}, Distance: {Config.CursorDistance}, Size: {Config.CursorSize}, ToggleKey: {Config.ToggleKey})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Manifest.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            BossCursorController.Reset();
            Context?.Logger?.Info("BossCursor loaded and active.");
        }

        public void Unload()
        {
            BossCursorController.Reset();
            Context?.Logger?.Info("BossCursor unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
