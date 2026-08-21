using System;
using System.Reflection;
using TerrariaModCore.API;

namespace OreCascade
{
    /// <summary>
    /// OreCascade plugin entry point implementing TMC IMod lifecycle.
    /// </summary>
    public class OreCascadeMod : IMod
    {
        public static OreCascadeMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public CascadeConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context;

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<CascadeConfig>();
            context.Logger.Info($"OreCascade initialized (Enabled: {Config.Enabled}, MaxBlocks: {Config.MaxBlocksPerActivation})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            Context?.Logger?.Info("OreCascade loaded and active.");
        }

        public void Unload()
        {
            Context?.Logger?.Info("OreCascade unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
