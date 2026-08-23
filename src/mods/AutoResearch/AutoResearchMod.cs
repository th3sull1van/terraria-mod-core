using System;
using System.Reflection;
using TerrariaModCore.API;

namespace AutoResearch
{
    /// <summary>
    /// AutoResearch plugin entry point implementing TMC IMod lifecycle.
    /// Automatically researches incomplete items in Journey Mode when entering inventory without altering quantity rules.
    /// </summary>
    public class AutoResearchMod : IMod
    {
        public static AutoResearchMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public AutoResearchConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<AutoResearchConfig>();
            context.Logger.Info($"AutoResearch initialized (Enabled: {Config.Enabled}, ScanInterval: {Config.ScanIntervalTicks} ticks, VoidBag: {Config.IncludeVoidBag})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Manifest.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            ResearchController.Reset();
            Context?.Logger?.Info("AutoResearch loaded and active.");
        }

        public void Unload()
        {
            ResearchController.Reset();
            Context?.Logger?.Info("AutoResearch unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
