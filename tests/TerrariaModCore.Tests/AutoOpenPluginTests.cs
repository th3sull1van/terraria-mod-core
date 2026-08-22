using System;
using System.Collections.Generic;
using AutoOpen;
using Terraria;
using Terraria.ID;

namespace TerrariaModCore.Tests
{
    public static class AutoOpenPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing AutoOpen Plugin Logic ---");

            // 1. Config Defaults & Boundaries
            var config = new AutoOpenConfig();
            assert(config.Enabled, "Default AutoOpenConfig Enabled is true");
            assert(config.RapidRightClickOpen, "Default RapidRightClickOpen is true");
            assert(config.OpenDelayTicks == 3, "Default OpenDelayTicks is 3 ticks (~20/s)");
            assert(config.BatchSize == 1, "Default BatchSize is 1");
            assert(config.PlaySound, "Default PlaySound is true");
            assert(!config.AutoOpenInventory, "Default AutoOpenInventory is false");
            assert(config.IncludeVoidBag, "Default IncludeVoidBag is true");
            assert(config.ExcludedItemIds != null && config.ExcludedItemIds.Count == 0, "Default ExcludedItemIds is empty");

            // 2. Openable Container Identification
            assert(OpenController.IsOpenable(ItemID.WoodenCrate, config), "Wooden Crate is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.IronCrate, config), "Iron Crate is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.GoldenCrate, config), "Golden Crate is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.HerbBag, config), "Herb Bag is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.CanOfWorms, config), "Can of Worms is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.Oyster, config), "Oyster is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.GoodieBag, config), "Goodie Bag is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.Present, config), "Present is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.LockBox, config), "Lock Box is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.ObsidianLockbox, config), "Obsidian Lockbox is recognized as openable");
            assert(OpenController.IsOpenable(ItemID.KingSlimeBossBag, config), "King Slime Boss Bag is recognized as openable");

            // 3. Non-Openable Filter
            assert(!OpenController.IsOpenable(ItemID.IronPickaxe, config), "Iron Pickaxe is NOT openable");
            assert(!OpenController.IsOpenable(ItemID.DirtBlock, config), "Dirt Block is NOT openable");
            assert(!OpenController.IsOpenable(ItemID.HealingPotion, config), "Healing Potion is NOT openable");
            assert(!OpenController.IsOpenable(0, config), "Item 0 is NOT openable");

            // 4. Blacklist Exclusion Enforcement
            config.ExcludedItemIds.Add(ItemID.GoldenCrate);
            assert(!OpenController.IsOpenable(ItemID.GoldenCrate, config), "Blacklisted Golden Crate is rejected by IsOpenable");
            config.ExcludedItemIds.Clear();

            // Setup Player Mock
            var player = new Player();
            player.whoAmI = 0;
            player.position = new Microsoft.Xna.Framework.Vector2(100f, 100f);
            Main.myPlayer = 0;
            Main.player = new Player[256];
            Main.player[0] = player;
            Main.gameMenu = false;
            WorldGen.isGeneratingOrLoadingWorld = false;

            Main.rand = new Terraria.Utilities.UnifiedRandom(12345);
            Main.item = new WorldItem[401];
            for (int i = 0; i < 401; i++) Main.item[i] = new WorldItem();
            Player.GetItemLogger = new Terraria.DataStructures.PlayerGetItemLogger();
            Main.showItemText = false;
            try { PopupText.ClearAll(); } catch { }

            player.inventory = new Item[59];
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();

            // 5. OpenSingle Stack Decrement & Air Transition
            var crateItem = new Item();
            crateItem.type = ItemID.WoodenCrate;
            crateItem.stack = 5;

            bool openedCrate = OpenController.OpenSingle(crateItem, player, false);
            assert(openedCrate, "OpenSingle successfully opens Wooden Crate");
            assert(crateItem.stack == 4, "Crate stack decremented from 5 to 4");

            var singleBag = new Item();
            singleBag.type = ItemID.HerbBag;
            singleBag.stack = 1;

            bool openedSingle = OpenController.OpenSingle(singleBag, player, false);
            assert(openedSingle, "OpenSingle successfully opens single stack Herb Bag");
            assert(singleBag.IsAir || singleBag.stack == 0, "Single stack item became air/empty after opening");

            // 6. Key Requirement for Lock Box
            var lockBox = new Item();
            lockBox.type = ItemID.LockBox;
            lockBox.stack = 3;

            // Player has NO golden keys
            bool lockBoxNoKey = OpenController.OpenSingle(lockBox, player, false);
            assert(!lockBoxNoKey, "Lock Box fails to open when player lacks Golden Key");
            assert(lockBox.stack == 3, "Lock Box stack untouched when missing key");

            // Give player Golden Key
            player.inventory[10].type = ItemID.GoldenKey;
            player.inventory[10].stack = 2;

            bool lockBoxWithKey = OpenController.OpenSingle(lockBox, player, false);
            assert(lockBoxWithKey, "Lock Box opens successfully when Golden Key is present");
            assert(lockBox.stack == 2, "Lock Box stack decremented to 2");
            assert(player.inventory[10].stack == 1, "Golden Key was consumed (2 -> 1)");

            // 7. Obsidian Lock Box (Requires Shadow Key, non-consumable)
            var obsLockBox = new Item();
            obsLockBox.type = ItemID.ObsidianLockbox;
            obsLockBox.stack = 2;

            bool obsNoKey = OpenController.OpenSingle(obsLockBox, player, false);
            assert(!obsNoKey, "Obsidian Lock Box fails to open without Shadow Key");
            assert(obsLockBox.stack == 2, "Obsidian Lock Box stack untouched without Shadow Key");

            // Give Shadow Key
            player.inventory[11].type = ItemID.ShadowKey;
            player.inventory[11].stack = 1;

            bool obsWithKey = OpenController.OpenSingle(obsLockBox, player, false);
            assert(obsWithKey, "Obsidian Lock Box opens successfully with Shadow Key");
            assert(obsLockBox.stack == 1, "Obsidian Lock Box stack decremented to 1");
            assert(player.inventory[11].stack == 1, "Shadow Key was NOT consumed (reusable)");

            // 8. Batch Processing Calculation
            var multiCrates = new Item[59];
            for (int i = 0; i < 59; i++) multiCrates[i] = new Item();
            multiCrates[0].type = ItemID.IronCrate;
            multiCrates[0].stack = 10;

            config.BatchSize = 3;
            config.OpenDelayTicks = 0;
            OpenController.Reset();

            OpenController.ProcessRightClick(multiCrates, 0, 0, player, config);
            assert(multiCrates[0].stack == 7, "Batch processing opened 3 crates in a single cycle (10 -> 7)");

            // Cleanup
            OpenController.Reset();
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();
        }
    }
}
