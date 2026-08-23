using System;
using TerrariaModCore.API;

namespace TurboBucket
{
    /// <summary>
    /// TurboBucket mod entry point.
    /// Accelerates pouring and liquid manipulation of Honey, Lava, Water, and Bottomless buckets.
    /// </summary>
    public class TurboBucketMod : IMod
    {
        public static TurboBucketMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public TurboBucketConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            Config = context.ConfigManager.Get<TurboBucketConfig>();

            context.Logger.Info($"TurboBucket initialized (Enabled: {Config.Enabled}, SpeedMultiplier: {Config.SpeedMultiplier}x, Water: {Config.AffectsWater}, Lava: {Config.AffectsLava}, Honey: {Config.AffectsHoney})");

            if (Config.Enabled)
            {
                context.PatchManager.RegisterAll(context.Manifest.Id, GetType().Assembly);
            }
        }

        public void Load()
        {
            if (Config != null && Config.Enabled)
            {
                Context.Logger.Info("TurboBucket loaded and active.");
            }
        }

        public void Unload()
        {
            Context?.Logger.Info("TurboBucket unloaded.");
            Instance = null;
        }
    }
}
