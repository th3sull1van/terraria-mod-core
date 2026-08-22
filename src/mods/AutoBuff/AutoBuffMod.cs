using System;
using System.Reflection;
using TerrariaModCore.API;

namespace AutoBuff
{
    /// <summary>
    /// AutoBuff plugin entry point implementing TMC IMod lifecycle.
    /// Automatically detects expired buffs and drinks corresponding potions and food from inventory.
    /// </summary>
    public class AutoBuffMod : IMod
    {
        public static AutoBuffMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public AutoBuffConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<AutoBuffConfig>();
            context.Logger.Info($"AutoBuff initialized (Enabled: {Config.Enabled}, CheckInterval: {Config.CheckIntervalTicks} ticks, IncludeFood: {Config.IncludeFood}, IncludeFlasks: {Config.IncludeFlasks})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Manifest.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            BuffController.Reset();
            Context?.Logger?.Info("AutoBuff loaded and active.");
        }

        public void Unload()
        {
            BuffController.Reset();
            Context?.Logger?.Info("AutoBuff unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
