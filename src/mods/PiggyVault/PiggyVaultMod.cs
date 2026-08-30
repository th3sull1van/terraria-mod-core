using System;
using System.Reflection;
using TerrariaModCore.API;

namespace PiggyVault
{
    /// <summary>
    /// PiggyVault plugin entry point implementing TMC IMod lifecycle.
    /// Provides the Piggy Bank with full Void Bag capabilities (overflow pickup, crafting, quick-buff/heal/mana, ammo/bait consumption, info accessories, and unity potions).
    /// </summary>
    public class PiggyVaultMod : IMod
    {
        public static PiggyVaultMod Instance { get; private set; }

        public IModContext Context { get; private set; }
        public PiggyVaultConfig Config { get; private set; }

        public void Initialize(IModContext context)
        {
            Instance = this;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            // Load configuration via TMC ConfigManager
            Config = context.ConfigManager.Get<PiggyVaultConfig>();
            context.Logger.Info($"PiggyVault initialized (Enabled: {Config.Enabled}, RequireItem: {Config.RequirePiggyItemInInventory}, AutoPickup: {Config.AutoPickupToPiggyBank}, Craft: {Config.CraftFromPiggyBank})");

            // Register patches through central PatchManager
            context.PatchManager.RegisterAll(context.Manifest.Id, Assembly.GetExecutingAssembly());
        }

        public void Load()
        {
            Context?.Logger?.Info("PiggyVault loaded and active.");
        }

        public void Unload()
        {
            Context?.Logger?.Info("PiggyVault unloaded.");
            Instance = null;
            Config = null;
            Context = null;
        }
    }
}
