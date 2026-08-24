using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using PiggyVault;
using PiggyVault.Patches;
using Terraria;
using Terraria.ID;
using TerrariaModCore.API;
using TerrariaModCore.Configuration;
using TerrariaModCore.Logging;
using TerrariaModCore.Patching;

namespace TerrariaModCore.Tests
{
    public static class PiggyVaultPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing PiggyVault Plugin Logic ---");

            // 1. Configuration Defaults
            var config = new PiggyVaultConfig();
            assert(config.Enabled, "Default PiggyVaultConfig Enabled is true");
            assert(config.RequirePiggyItemInInventory, "Default RequirePiggyItemInInventory is true");
            assert(config.AutoPickupToPiggyBank, "Default AutoPickupToPiggyBank is true");
            assert(config.CraftFromPiggyBank, "Default CraftFromPiggyBank is true");
            assert(config.QuickBuffFromPiggyBank, "Default QuickBuffFromPiggyBank is true");
            assert(config.QuickHealFromPiggyBank, "Default QuickHealFromPiggyBank is true");
            assert(config.QuickManaFromPiggyBank, "Default QuickManaFromPiggyBank is true");
            assert(config.ConsumeAmmoAndBaitFromPiggyBank, "Default ConsumeAmmoAndBaitFromPiggyBank is true");
            assert(config.InfoAccessoriesInPiggyBank, "Default InfoAccessoriesInPiggyBank is true");
            assert(config.WormholePotionFromPiggyBank, "Default WormholePotionFromPiggyBank is true");
            assert(config.PlayPickupSound, "Default PlayPickupSound is true");
            assert(config.ShowPickupText, "Default ShowPickupText is true");

            // 2. Setup Mock Player & Banks
            var player = new Player();
            player.whoAmI = 0;
            player.statLifeMax = 400;
            player.statLifeMax2 = 400;
            player.statLife = 400;
            player.statManaMax = 200;
            player.statManaMax2 = 200;
            player.statMana = 200;
            player.potionDelay = 0;
            player.buffType = new int[Player.maxBuffs];
            player.buffTime = new int[Player.maxBuffs];

            player.inventory = new Item[59];
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();

            player.miscEquips = new Item[5];
            for (int i = 0; i < 5; i++) player.miscEquips[i] = new Item();

            if (player.bank.item == null) player.bank.item = new Item[40];
            for (int i = 0; i < player.bank.item.Length; i++) player.bank.item[i] = new Item();

            if (player.bank4.item == null) player.bank4.item = new Item[40];
            for (int i = 0; i < player.bank4.item.Length; i++) player.bank4.item[i] = new Item();

            Main.myPlayer = 0;
            Main.player = new Player[256];
            Main.player[0] = player;

            // 3. Piggy Bank Usability Detection
            assert(!PiggyVaultController.IsPiggyBankUsable(player, config), "Empty inventory returns not usable when item is required");

            // Put Piggy Bank item in inventory
            player.inventory[0].type = PiggyVaultController.ItemIdPiggyBank;
            player.inventory[0].stack = 1;
            assert(PiggyVaultController.IsPiggyBankUsable(player, config), "Usable when carrying Piggy Bank (Item 87)");
            player.inventory[0].TurnToAir();

            // Put Money Trough item in inventory
            player.inventory[1].type = PiggyVaultController.ItemIdMoneyTrough;
            player.inventory[1].stack = 1;
            assert(PiggyVaultController.IsPiggyBankUsable(player, config), "Usable when carrying Money Trough (Item 3213)");
            player.inventory[1].TurnToAir();

            // Put Eyebone (Chester) in inventory
            player.inventory[2].type = PiggyVaultController.ItemIdEyebone;
            player.inventory[2].stack = 1;
            assert(PiggyVaultController.IsPiggyBankUsable(player, config), "Usable when carrying Eyebone / Chester (Item 5098)");
            player.inventory[2].TurnToAir();

            // Put Eyebone in miscEquips (pet slot)
            player.miscEquips[0].type = PiggyVaultController.ItemIdEyebone;
            player.miscEquips[0].stack = 1;
            assert(PiggyVaultController.IsPiggyBankUsable(player, config), "Usable when Chester is equipped in miscEquips");
            player.miscEquips[0].TurnToAir();

            // Config override: RequirePiggyItemInInventory = false
            config.RequirePiggyItemInInventory = false;
            assert(PiggyVaultController.IsPiggyBankUsable(player, config), "Usable with empty inventory when RequirePiggyItemInInventory is false");
            config.RequirePiggyItemInInventory = true;

            // Give player Money Trough for remaining tests
            player.inventory[0].type = PiggyVaultController.ItemIdMoneyTrough;
            player.inventory[0].stack = 1;

            // 4. Auto-Pickup & Item Placement in Piggy Bank
            var groundItem = new Item();
            groundItem.type = ItemID.GoldBar;
            groundItem.stack = 25;
            groundItem.maxStack = 9999;

            assert(PiggyVaultController.HasSpaceInPiggyBank(player, groundItem), "HasSpaceInPiggyBank returns true for empty bank");

            var remaining = PiggyVaultController.PutItemInPiggyBank(player, groundItem, GetItemSettings.PickupItemFromWorld);
            assert(remaining.IsAir || remaining.stack == 0, "PutItemInPiggyBank consumed entire 25 GoldBar stack");
            assert(player.bank.item[0].type == ItemID.GoldBar && player.bank.item[0].stack == 25, "Piggy Bank slot 0 contains 25 GoldBar");

            // Stacking test: Add 30 more GoldBar to merge into slot 0
            var secondGroundItem = new Item();
            secondGroundItem.type = ItemID.GoldBar;
            secondGroundItem.stack = 30;
            secondGroundItem.maxStack = 9999;

            var secondRemaining = PiggyVaultController.PutItemInPiggyBank(player, secondGroundItem, GetItemSettings.PickupItemFromWorld);
            assert(secondRemaining.IsAir || secondRemaining.stack == 0, "PutItemInPiggyBank consumed second GoldBar stack");
            assert(player.bank.item[0].type == ItemID.GoldBar && player.bank.item[0].stack == 55, "Piggy Bank slot 0 stacked to 55 GoldBar");

            // 5. Crafting Material Collection from Piggy Bank
            player.bank.item[1].type = ItemID.Wood;
            player.bank.item[1].stack = 100;

            // Instantiate mod for patch testing
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_temp_pv_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            var coreLogger = new CoreLogger(null, "Test");
            var modLogger = new ModLogger(coreLogger, "piggy_vault");
            var patchManager = new PatchManager(coreLogger);
            var configManager = new ModConfigManager(tempDir, modLogger);
            var manifest = new ModManifest
            {
                Id = "piggy_vault",
                Name = "PiggyVault",
                Version = "1.0.0",
                EntryAssembly = "PiggyVault.dll",
                EntryType = typeof(PiggyVaultMod).FullName,
                Enabled = true
            };
            var context = new ModContext(manifest, tempDir, tempDir, modLogger, configManager, patchManager, null, null, "1.4.5.8");

            var modInstance = new PiggyVaultMod();
            modInstance.Initialize(context);
            modInstance.Load();

            PiggyCraftingPatch.Postfix(player);
            var recipeChests = AccessTools.Field(typeof(Recipe), "_recipeChests")?.GetValue(null) as List<Chest>;
            assert(recipeChests != null && recipeChests.Contains(player.bank), "Recipe._recipeChests contains player.bank after PiggyCraftingPatch");

            // 6. Quick Heal Fallback from Piggy Bank
            player.statLife = 100; // missing 300 HP
            player.bank.item[2].type = ItemID.GreaterHealingPotion;
            player.bank.item[2].stack = 10;
            player.bank.item[2].healLife = 150;
            player.bank.item[2].potion = true;

            Item healItem = null;
            PiggyQuickHealPatch.Postfix(player, ref healItem);
            assert(healItem != null && healItem.type == ItemID.GreaterHealingPotion, "Quick Heal selects Greater Healing Potion from Piggy Bank");

            // 7. Quick Mana Fallback from Piggy Bank
            player.statMana = 10;
            player.bank.item[3].type = ItemID.GreaterManaPotion;
            player.bank.item[3].stack = 15;
            player.bank.item[3].healMana = 200;
            player.bank.item[3].potion = true;

            Item manaItem = null;
            PiggyQuickManaPatch.Postfix(player, ref manaItem);
            assert(manaItem != null && manaItem.type == ItemID.GreaterManaPotion, "Quick Mana selects Greater Mana Potion from Piggy Bank");

            // 8. Quick Buff & Food from Piggy Bank
            player.bank.item[4].type = ItemID.CookedFish;
            player.bank.item[4].stack = 5;
            player.bank.item[4].buffType = BuffID.WellFed;
            player.bank.item[4].buffTime = 18000;
            player.bank.item[4].consumable = true;

            player.bank.item[5].type = ItemID.IronskinPotion;
            player.bank.item[5].stack = 5;
            player.bank.item[5].buffType = BuffID.Ironskin;
            player.bank.item[5].buffTime = 28800;
            player.bank.item[5].consumable = true;

            Item bestFood = null;
            PiggyQuickFoodPatch.Postfix(player, ref bestFood);
            assert(bestFood != null && bestFood.type == ItemID.CookedFish, "Quick Buff selects Cooked Fish from Piggy Bank");

            PiggyQuickBuffPatch.Postfix(player);
            assert(player.FindBuffIndex(BuffID.WellFed) != -1, "Well Fed buff applied from Piggy Bank");
            assert(player.FindBuffIndex(BuffID.Ironskin) != -1, "Ironskin buff applied from Piggy Bank");
            assert(player.bank.item[4].stack == 4, "Cooked Fish stack decremented in Piggy Bank (5 -> 4)");
            assert(player.bank.item[5].stack == 4, "Ironskin Potion stack decremented in Piggy Bank (5 -> 4)");

            // 9. Ammo, Wire & Bait Consumption from Piggy Bank
            player.bank.item[6].type = ItemID.MusketBall;
            player.bank.item[6].stack = 50;

            bool musketConsumed = false;
            PiggyConsumeItemPatch.Postfix(player, ItemID.MusketBall, false, false, ref musketConsumed);
            assert(musketConsumed, "Musket Ball consumed from Piggy Bank via Player.ConsumeItem");
            assert(player.bank.item[6].stack == 49, "Musket Ball stack decremented in Piggy Bank (50 -> 49)");

            // 10. Wormhole (Unity) Potion Support
            player.bank.item[7].type = PiggyVaultController.ItemIdWormholePotion;
            player.bank.item[7].stack = 3;

            bool hasUnity = false;
            PiggyHasUnityPotionPatch.Postfix(player, ref hasUnity);
            assert(hasUnity, "HasUnityPotion returns true when Wormhole Potion is in Piggy Bank");

            PiggyTakeUnityPotionPatch.Prefix(player);
            PiggyTakeUnityPotionPatch.Postfix(player);
            assert(player.bank.item[7].stack == 2, "TakeUnityPotion consumed 1 Wormhole Potion from Piggy Bank (3 -> 2)");

            // 11. Informational Accessories UI Activation
            player.bank.item[8].type = ItemID.Compass;
            player.bank.item[8].stack = 1;
            player.bank.item[9].type = ItemID.DepthMeter;
            player.bank.item[9].stack = 1;

            assert(player.accCompass == 0 && player.accDepthMeter == 0, "Info accessories initially 0");
            PiggyInfoAccessoriesPatch.Postfix(player);
            assert(player.accCompass > 0, "accCompass active from Compass stored in Piggy Bank");
            assert(player.accDepthMeter > 0, "accDepthMeter active from Depth Meter stored in Piggy Bank");

            // Cleanup & Unload
            modInstance.Unload();

            try
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
            catch { }
        }
    }
}
