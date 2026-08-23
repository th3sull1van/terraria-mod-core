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

            // 9. Piggy Bank Access Detection & Consumption
            assert(config.IncludePiggyBank, "Default IncludePiggyBank is true");
            player.inventory[0].TurnToAir();

            if (player.bank.item == null) player.bank.item = new Item[40];
            for (int i = 0; i < player.bank.item.Length; i++) player.bank.item[i] = new Item();

            assert(!BuffController.CanAccessPiggyBank(player), "Cannot access piggy bank when empty inventory and not open");

            // Open piggy bank or carry Money Trough
            player.inventory[0].type = ItemID.MoneyTrough;
            player.inventory[0].stack = 1;
            assert(BuffController.CanAccessPiggyBank(player), "Can access piggy bank when carrying Money Trough");

            // Place Regeneration potion and Cooked Fish in Piggy Bank
            for (int i = 0; i < Player.maxBuffs; i++) { player.buffType[i] = 0; player.buffTime[i] = 0; }

            player.bank.item[0].type = ItemID.CookedFish;
            player.bank.item[0].stack = 3;
            player.bank.item[0].buffType = BuffID.WellFed;
            player.bank.item[0].buffTime = 18000;
            player.bank.item[0].consumable = true;

            player.bank.item[1].type = ItemID.RegenerationPotion;
            player.bank.item[1].stack = 4;
            player.bank.item[1].buffType = BuffID.Regeneration;
            player.bank.item[1].buffTime = 28800;
            player.bank.item[1].consumable = true;

            // AutoBuff should detect and consume food from Piggy Bank
            bool bankFoodConsumed = BuffController.TryConsumeBestFood(player, config);
            assert(bankFoodConsumed, "Food consumed directly from Piggy Bank by AutoBuff");
            assert(player.bank.item[0].stack == 2, "Piggy Bank food stack decremented (3 -> 2)");
            assert(BuffController.HasActiveFoodBuff(player), "Player gained food buff from Piggy Bank");

            // AutoBuff should detect and consume potion from Piggy Bank
            BuffController.ProcessBuffPotions(player, config);
            assert(player.FindBuffIndex(BuffID.Regeneration) >= 0, "Player gained Regeneration buff from Piggy Bank potion");
            assert(player.bank.item[1].stack == 3, "Piggy Bank potion stack decremented (4 -> 3)");

            // Cleanup
            for (int i = 0; i < Player.maxBuffs; i++) { player.buffType[i] = 0; player.buffTime[i] = 0; }
            for (int i = 0; i < 59; i++) player.inventory[i] = new Item();
            for (int i = 0; i < 40; i++) player.bank.item[i] = new Item();
            BuffController.Reset();
        }
    }
}
