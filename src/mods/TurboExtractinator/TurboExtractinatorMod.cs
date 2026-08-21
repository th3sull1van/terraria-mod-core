using System;
using TerrariaModCore.API;

namespace TurboExtractinator
{
    /// <summary>
    /// TurboExtractinator plugin entry point.
    /// Speeds up the processing rate of Extractinator and Chlorophyte Extractinator tiles.
    /// </summary>
    public class TurboExtractinatorMod : IMod
    {
        public static TurboExtractinatorMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public TurboExtractConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            Config = context.ConfigManager.Get<TurboExtractConfig>();

            context.Logger.Info($"TurboExtractinator initialized (Enabled: {Config.Enabled}, SpeedMultiplier: {Config.SpeedMultiplier}x, BatchSize: {Config.BatchExtractionSize})");

            if (Config.Enabled)
            {
                context.PatchManager.RegisterAll(context.Manifest.Id, GetType().Assembly);
            }
        }

        public void Load()
        {
            if (Config != null && Config.Enabled)
            {
                Context.Logger.Info("TurboExtractinator loaded and active.");
            }
        }

        public void Unload()
        {
            Context?.Logger.Info("TurboExtractinator unloaded.");
            Instance = null;
        }
    }
}
