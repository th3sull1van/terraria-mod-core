using System;
using System.Reflection;
using TerrariaModCore.API;

namespace AutoFishing
{
    /// <summary>
    /// AutoFishing plugin entry point implementing TMC IMod lifecycle.
    /// </summary>
    public class AutoFishingMod : IMod
    {
        public static AutoFishingMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public AutoFishingConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context;

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<AutoFishingConfig>();
            context.Logger.Info($"AutoFishing initialized (Enabled: {Config.Enabled}, AutoCast: {Config.AutoCast}, AutoReel: {Config.AutoReel})");

            // Register patches through TMC PatchManager
            context.PatchManager.RegisterAll(context.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            FishingController.Reset();
            Context?.Logger?.Info("AutoFishing loaded and active.");
        }

        public void Unload()
        {
            FishingController.Reset();
            Context?.Logger?.Info("AutoFishing unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
