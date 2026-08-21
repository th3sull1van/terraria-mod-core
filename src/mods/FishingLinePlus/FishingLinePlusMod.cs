using System;
using System.Reflection;
using TerrariaModCore.API;

namespace FishingLinePlus
{
    /// <summary>
    /// FishingLinePlus plugin entry point implementing TMC IMod lifecycle.
    /// </summary>
    public class FishingLinePlusMod : IMod
    {
        public static FishingLinePlusMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public FishingLineConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context;

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<FishingLineConfig>();
            context.Logger.Info($"FishingLinePlus initialized (Enabled: {Config.Enabled}, MaxActiveLines: {Config.MaxActiveFishingLines}, LinesPerCast: {Config.LinesPerCast}, Spread: {Config.SpreadAngleDegrees}°)");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            Context?.Logger?.Info("FishingLinePlus loaded and active.");
        }

        public void Unload()
        {
            Context?.Logger?.Info("FishingLinePlus unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
