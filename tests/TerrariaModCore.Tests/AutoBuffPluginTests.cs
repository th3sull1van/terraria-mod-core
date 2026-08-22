using System;
using System.Collections.Generic;
using AutoBuff;
using Terraria;
using Terraria.ID;

namespace TerrariaModCore.Tests
{
    public static class AutoBuffPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing AutoBuff Plugin Logic ---");

            // 1. Config Defaults & Deserialization
            var config = new AutoBuffConfig();
            assert(config.Enabled, "Default AutoBuffConfig Enabled is true");
            assert(config.CheckIntervalTicks == 15, "Default CheckIntervalTicks is 15");
            assert(config.IncludeFood, "Default IncludeFood is true");
            assert(config.IncludeFlasks, "Default IncludeFlasks is true");
            assert(config.IncludeVoidBag, "Default IncludeVoidBag is true");
            assert(config.ExcludedBuffIds != null && config.ExcludedBuffIds.Contains(18), "Gravitation buff (18) is excluded by default");
            assert(config.ExcludedItemIds != null && config.ExcludedItemIds.Contains(1344), "Red Potion item (1344) is excluded by default");

            // 2. Food Identification Logic
            assert(BuffController.IsFoodBuff(BuffID.WellFed), "BuffID.WellFed (26/27) is recognized as food buff");
            assert(BuffController.IsFoodBuff(BuffID.WellFed2), "BuffID.WellFed2 is recognized as food buff");
            assert(BuffController.IsFoodBuff(BuffID.WellFed3), "BuffID.WellFed3 is recognized as food buff");
            assert(!BuffController.IsFoodBuff(BuffID.Ironskin), "Ironskin (BuffID.Ironskin) is NOT recognized as food buff");
            assert(!BuffController.IsFoodBuff(0), "Buff 0 is NOT recognized as food buff");

            // Setup Player test mock
            var player = new Player();
            player.whoAmI = 0;
            Main.myPlayer = 0;
            Main.gameMenu = false;
            WorldGen.isGeneratingOrLoadingWorld = false;

            player.buffType = new int[Player.maxBuffs];
            player.buffTime = new int[Player.maxBuffs];
            player.inventory = new Item[59];
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();

            // 3. Active Buff Duration Detection (Prevents Wasted Potions)
            var ironskinItem = new Item();
            ironskinItem.type = ItemID.IronskinPotion;
            ironskinItem.buffType = BuffID.Ironskin;
            ironskinItem.buffTime = 18000;
            ironskinItem.stack = 5;
            ironskinItem.consumable = true;

            // Give player active Ironskin buff with 1000 ticks remaining
            player.buffType[0] = BuffID.Ironskin;
            player.buffTime[0] = 1000;

            bool consumedWhileActive = BuffController.TryProcessItem(player, ironskinItem, config);
            assert(!consumedWhileActive, "TryProcessItem returns false when buff is actively running");
            assert(ironskinItem.stack == 5, "Potion stack was not consumed while buff is active");

            // 4. Expired Buff Auto-Consumption
            player.buffType[0] = 0;
            player.buffTime[0] = 0;

            bool consumedAfterExpired = BuffController.TryProcessItem(player, ironskinItem, config);
            assert(consumedAfterExpired, "TryProcessItem returns true and consumes potion when buff is absent/expired");
            assert(ironskinItem.stack == 4, "Potion stack decremented from 5 to 4 upon consumption");
            assert(player.FindBuffIndex(BuffID.Ironskin) >= 0, "Player gained Ironskin buff upon consumption");

            // 5. Stack Depletion to Air
            var singlePotion = new Item();
            singlePotion.type = ItemID.SwiftnessPotion;
            singlePotion.buffType = BuffID.Swiftness;
            singlePotion.buffTime = 14400;
            singlePotion.stack = 1;
            singlePotion.consumable = true;

            assert(!singlePotion.IsAir, "Single potion initially is not air");
            bool consumedSingle = BuffController.TryProcessItem(player, singlePotion, config);
            assert(consumedSingle, "Consumed single stack potion successfully");
            assert(singlePotion.IsAir || singlePotion.stack == 0, "Single stack item became air/empty after consumption");

            // 6. Blacklist Exclusion Enforcement
            var redPotion = new Item();
            redPotion.type = 1344; // Red Potion item ID
            redPotion.buffType = 10;
            redPotion.stack = 3;
            redPotion.consumable = true;

            bool redPotionConsumed = BuffController.TryProcessItem(player, redPotion, config);
            assert(!redPotionConsumed, "Blacklisted Item ID (Red Potion) is strictly rejected");
            assert(redPotion.stack == 3, "Blacklisted Item stack remained untouched");

            var gravPotion = new Item();
            gravPotion.type = ItemID.GravitationPotion;
            gravPotion.buffType = 18; // Gravitation Buff ID
            gravPotion.stack = 10;
            gravPotion.consumable = true;

            bool gravPotionConsumed = BuffController.TryProcessItem(player, gravPotion, config);
            assert(!gravPotionConsumed, "Blacklisted Buff ID (Gravitation) is strictly rejected");
            assert(gravPotion.stack == 10, "Gravitation Potion stack remained untouched");

            // 7. Summon / Weapon Safety
            var summonItem = new Item();
            summonItem.type = ItemID.BabyBirdStaff;
            summonItem.buffType = BuffID.BabyBird;
            summonItem.stack = 1;
            summonItem.summon = true;

            bool summonConsumed = BuffController.TryProcessItem(player, summonItem, config);
            assert(!summonConsumed, "Summoning items are rejected by auto-buff controller");

            // 8. Food Buff Auto-Refresh
            // Clear buffs
            for (int i = 0; i < Player.maxBuffs; i++) { player.buffType[i] = 0; player.buffTime[i] = 0; }

            assert(!BuffController.HasActiveFoodBuff(player), "Player has no active food buff initially");

            var burgerFood = new Item();
            burgerFood.type = ItemID.Burger;
            burgerFood.buffType = BuffID.WellFed3; // Exquisitely Stuffed
            burgerFood.buffTime = 36000;
            burgerFood.stack = 2;
            burgerFood.consumable = true;

            player.inventory[0] = burgerFood;

            bool foodConsumed = BuffController.TryConsumeBestFood(player, config);
            assert(foodConsumed, "Best food item consumed when player has no Well-Fed buff");
            assert(burgerFood.stack == 1, "Food stack decremented by 1");
            assert(BuffController.HasActiveFoodBuff(player), "Player now has active food buff");

            // Cleanup
            for (int i = 0; i < Player.maxBuffs; i++) { player.buffType[i] = 0; player.buffTime[i] = 0; }
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();
            BuffController.Reset();
        }
    }
}
