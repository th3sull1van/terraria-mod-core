using System;
using System.Collections.Generic;
using AutoResearch;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace TerrariaModCore.Tests
{
    public static class AutoResearchPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing AutoResearch Plugin Logic ---");

            // 1. Config Defaults & Boundaries
            var config = new AutoResearchConfig();
            assert(config.Enabled, "Default AutoResearchConfig Enabled is true");
            assert(config.ScanIntervalTicks == 1, "Default ScanIntervalTicks is 1 tick");
            assert(config.IncludeVoidBag, "Default IncludeVoidBag is true");
            assert(config.PlaySound, "Default PlaySound is true");
            assert(config.ShowNotifications, "Default ShowNotifications is true");
            assert(config.ExcludedItemIds != null && config.ExcludedItemIds.Count == 0, "Default ExcludedItemIds is empty");

            // 2. Setup Player & Creative Tracker Mock
            var player = new Player();
            player.whoAmI = 0;
            player.position = new Microsoft.Xna.Framework.Vector2(100f, 100f);
            player.difficulty = 3; // Creative / Journey mode
            player.inventory = new Item[59];
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();
            player.creativeTracker = new CreativeUnlocksTracker();
            if (player.bank4.item == null)
            {
                player.bank4.item = new Item[40];
            }
            for (int i = 0; i < player.bank4.item.Length; i++) player.bank4.item[i] = new Item();

            Main.myPlayer = 0;
            Main.player = new Player[256];
            Main.player[0] = player;
            Main.gameMenu = false;
            Main.ServerSideCharacter = false;
            Main.netMode = 0;
            Main.mouseItem = new Item();

            // Initialize ContentSamples & Sacrifice Catalog
            try
            {
                Terraria.ID.ContentSamples.Initialize();
            }
            catch { }

            if (Terraria.ID.ContentSamples.ItemPersistentIdsByNetIds != null)
            {
                Terraria.ID.ContentSamples.ItemPersistentIdsByNetIds[ItemID.Wood] = "Terraria/Wood";
                Terraria.ID.ContentSamples.ItemPersistentIdsByNetIds[ItemID.IronBroadsword] = "Terraria/IronBroadsword";
                Terraria.ID.ContentSamples.ItemPersistentIdsByNetIds[ItemID.IronOre] = "Terraria/IronOre";
                Terraria.ID.ContentSamples.ItemPersistentIdsByNetIds[ItemID.DirtBlock] = "Terraria/DirtBlock";
            }

            var catalog = CreativeItemSacrificesCatalog.Instance;
            if (catalog != null)
            {
                try { catalog.Initialize(); } catch { }
                catalog.SacrificeCountNeededByItemId[ItemID.Wood] = 100;
                catalog.SacrificeCountNeededByItemId[ItemID.IronBroadsword] = 1;
                catalog.SacrificeCountNeededByItemId[ItemID.IronOre] = 25;
                catalog.SacrificeCountNeededByItemId[ItemID.DirtBlock] = 100;
            }

            // 3. Non-Journey Mode Immunity
            player.difficulty = 0; // Classic / Softcore
            var classicItem = new Item();
            classicItem.type = ItemID.Wood;
            classicItem.stack = 50;

            int sacAmount;
            bool isDone;
            bool sacrificedInClassic = ResearchController.TrySacrificeItem(classicItem, player, config, out sacAmount, out isDone);
            assert(!sacrificedInClassic, "Classic/Softcore character is immune to auto-research");
            assert(classicItem.stack == 50, "Item stack unchanged for non-Journey character");
            assert(sacAmount == 0, "Sacrifice amount is 0 for non-Journey character");

            // Restore Journey Mode
            player.difficulty = 3;

            // 4. Single-Item Full Research (Iron Broadsword: 1 needed)
            var swordItem = new Item();
            swordItem.type = ItemID.IronBroadsword;
            swordItem.stack = 1;

            bool swordSacrificed = ResearchController.TrySacrificeItem(swordItem, player, config, out sacAmount, out isDone);
            assert(swordSacrificed, "Iron Broadsword (1 needed) is successfully sacrificed");
            assert(sacAmount == 1, "Consumed 1 Iron Broadsword");
            assert(isDone, "Iron Broadsword reached 100% research completion");
            assert(swordItem.IsAir || swordItem.stack == 0, "Iron Broadsword item stack turned to air");
            assert(Main.LocalPlayerCreativeTracker.ItemSacrifices.IsFullyResearched(ItemID.IronBroadsword), "Iron Broadsword is marked fully researched in tracker");

            // 5. Multi-Item Partial Research Progression (Wood: 100 needed, give 40)
            var woodPart1 = new Item();
            woodPart1.type = ItemID.Wood;
            woodPart1.stack = 40;

            bool woodPart1Researched = ResearchController.TrySacrificeItem(woodPart1, player, config, out sacAmount, out isDone);
            assert(woodPart1Researched, "Wood (0/100) partially researched with 40 stack");
            assert(sacAmount == 40, "Consumed 40 Wood for research");
            assert(!isDone, "Wood is not yet complete (40/100)");
            assert(woodPart1.IsAir || woodPart1.stack == 0, "Wood stack turned to air after consuming full 40");

            int currentWoodHave = 0;
            int currentWoodNeed = 0;
            Main.LocalPlayerCreativeTracker.ItemSacrifices.TryGetSacrificeNumbers(ItemID.Wood, out currentWoodHave, out currentWoodNeed);
            assert(currentWoodHave == 40 && currentWoodNeed == 100, "Tracker records exactly 40/100 Wood researched");

            // 6. Multi-Item Completion with Remainder / Overflow (Wood: 40/100, give 90 stack -> needs 60, remainder 30)
            var woodPart2 = new Item();
            woodPart2.type = ItemID.Wood;
            woodPart2.stack = 90;

            bool woodPart2Researched = ResearchController.TrySacrificeItem(woodPart2, player, config, out sacAmount, out isDone);
            assert(woodPart2Researched, "Wood (40/100) researched with 90 stack");
            assert(sacAmount == 60, "Only consumed 60 Wood to satisfy exact 100 cap");
            assert(isDone, "Wood reached 100% research completion (100/100)");
            assert(woodPart2.stack == 30, "Remainder of 30 Wood preserved in item stack (90 - 60 = 30)");
            assert(!woodPart2.IsAir && woodPart2.type == ItemID.Wood, "Remainder Wood is not air");
            assert(Main.LocalPlayerCreativeTracker.ItemSacrifices.IsFullyResearched(ItemID.Wood), "Wood is now fully researched");

            // 7. Already Researched Item Immunity (Wood is 100/100)
            var woodFull = new Item();
            woodFull.type = ItemID.Wood;
            woodFull.stack = 50;

            bool woodFullResearched = ResearchController.TrySacrificeItem(woodFull, player, config, out sacAmount, out isDone);
            assert(!woodFullResearched, "Already fully researched Wood is ignored");
            assert(woodFull.stack == 50, "Fully researched Wood stack untouched (50)");
            assert(sacAmount == 0, "Sacrifice amount is 0 for completed item");

            // 8. Blacklist Exclusion Enforcement
            config.ExcludedItemIds.Add(ItemID.IronOre);
            var oreItem = new Item();
            oreItem.type = ItemID.IronOre;
            oreItem.stack = 20;

            bool oreResearched = ResearchController.TrySacrificeItem(oreItem, player, config, out sacAmount, out isDone);
            assert(!oreResearched, "Blacklisted Iron Ore is rejected by TrySacrificeItem");
            assert(oreItem.stack == 20, "Blacklisted Iron Ore stack preserved");
            config.ExcludedItemIds.Clear();

            // 9. Invalid / Air Items
            var airItem = new Item();
            assert(!ResearchController.TrySacrificeItem(airItem, player, config, out _, out _), "Air item returns false");
            assert(!ResearchController.TrySacrificeItem(null, player, config, out _, out _), "Null item returns false");

            // 10. Inventory Entry Processing
            player.inventory[5].type = ItemID.IronOre;
            player.inventory[5].stack = 25;

            ResearchController.Reset();
            ResearchController.UpdateInventoryScan(player, config);
            assert(player.inventory[5].IsAir || player.inventory[5].stack == 0, "Iron Ore turned to air after full sacrifice via inventory scan");
            assert(Main.LocalPlayerCreativeTracker.ItemSacrifices.IsFullyResearched(ItemID.IronOre), "Iron Ore is now fully researched");

            // 11. Background Inventory & Void Bag Scan (Mouse item is preserved until placed in inventory)
            // Setup catalog for DirtBlock (100 needed)
            catalog.SacrificeCountNeededByItemId[ItemID.DirtBlock] = 100;

            // Place 30 dirt in inventory[0]
            player.inventory[0].type = ItemID.DirtBlock;
            player.inventory[0].stack = 30;

            // Place 20 dirt on cursor (crafted item held on mouse in inventory[58] and Main.mouseItem)
            Main.mouseItem.type = ItemID.DirtBlock;
            Main.mouseItem.stack = 20;
            player.inventory[58].type = ItemID.DirtBlock;
            player.inventory[58].stack = 20;

            // Place 20 dirt in Void Bag
            player.bank4.item[0].type = ItemID.DirtBlock;
            player.bank4.item[0].stack = 20;

            config.ScanIntervalTicks = 1;
            ResearchController.Reset();
            ResearchController.UpdateInventoryScan(player, config);

            int dirtHave = 0;
            int dirtNeed = 0;
            Main.LocalPlayerCreativeTracker.ItemSacrifices.TryGetSacrificeNumbers(ItemID.DirtBlock, out dirtHave, out dirtNeed);
            assert(dirtHave == 50 && dirtNeed == 100, "Background scan researched 30 (inv) + 20 (void) = 50 Dirt");
            assert(player.inventory[0].IsAir || player.inventory[0].stack == 0, "Inventory Dirt consumed");
            assert(!Main.mouseItem.IsAir && Main.mouseItem.stack == 20, "Cursor Dirt preserved while being held on mouse");
            assert(!player.inventory[58].IsAir && player.inventory[58].stack == 20, "inventory[58] preserved while being held on mouse");
            assert(player.bank4.item[0].IsAir || player.bank4.item[0].stack == 0, "Void Bag Dirt consumed");

            // Now place the cursor item into inventory[1]
            player.inventory[1] = Main.mouseItem.Clone();
            Main.mouseItem.TurnToAir();

            ResearchController.UpdateInventoryScan(player, config);
            Main.LocalPlayerCreativeTracker.ItemSacrifices.TryGetSacrificeNumbers(ItemID.DirtBlock, out dirtHave, out dirtNeed);
            assert(dirtHave == 70 && dirtNeed == 100, "After placing cursor item into inventory, research reached 70 Dirt");
            assert(player.inventory[1].IsAir || player.inventory[1].stack == 0, "Inventory slot [1] Dirt consumed upon placement");

            // Cleanup
            ResearchController.Reset();
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();
            for (int i = 0; i < 40; i++) player.bank4.item[i] = new Item();
            Main.mouseItem = new Item();
        }
    }
}
