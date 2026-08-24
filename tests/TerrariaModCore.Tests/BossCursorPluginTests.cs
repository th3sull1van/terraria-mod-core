using System;
using System.Collections.Generic;
using System.IO;
using BossCursor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaModCore.API;

namespace TerrariaModCore.Tests
{
    public static class BossCursorPluginTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing BossCursor Plugin Logic ---");

            // 1. Configuration Defaults
            var config = new BossCursorConfig();
            assert(config.Enabled, "Default BossCursorConfig Enabled is true");
            assert(!config.HideOnScreen, "Default HideOnScreen is false");
            assert(config.CursorDistance == 150, "Default CursorDistance is 150 px");
            assert(Math.Abs(config.CursorSize - 1.0f) < 0.001f, "Default CursorSize is 1.0x");
            assert(config.BlacklistPillars, "Default BlacklistPillars is true");
            assert(config.ToggleKey == "B", "Default ToggleKey is 'B'");
            assert(config.ExcludedNpcIds != null && config.ExcludedNpcIds.Count == 0, "Default ExcludedNpcIds is empty");
            assert(config.IncludedNpcIds != null && config.IncludedNpcIds.Count == 0, "Default IncludedNpcIds is empty");

            // 2. Boss Identification & Filtering
            BossCursorController.Reset();

            // Null / Inactive / DontCountMe
            assert(!BossCursorController.IsBoss(null, config), "Null NPC is not a boss");

            var inactiveNpc = new NPC { active = false, boss = true, type = NPCID.EyeofCthulhu };
            assert(!BossCursorController.IsBoss(inactiveNpc, config), "Inactive boss NPC is ignored");

            var dontCountNpc = new NPC { active = true, boss = true, dontCountMe = true, type = NPCID.EyeofCthulhu };
            assert(!BossCursorController.IsBoss(dontCountNpc, config), "dontCountMe NPC is ignored");

            // Standard Boss
            var bossNpc = new NPC { active = true, boss = true, type = NPCID.EyeofCthulhu };
            assert(BossCursorController.IsBoss(bossNpc, config), "Active EyeofCthulhu (boss=true) is detected as boss");

            var normalNpc = new NPC { active = true, boss = false, type = NPCID.GreenSlime };
            assert(!BossCursorController.IsBoss(normalNpc, config), "Standard GreenSlime is not a boss");

            // 3. Celestial Pillar Blacklisting
            var solarPillar = new NPC { active = true, boss = false, type = BossCursorController.PillarSolar };
            var nebulaPillar = new NPC { active = true, boss = false, type = BossCursorController.PillarNebula };
            var vortexPillar = new NPC { active = true, boss = false, type = BossCursorController.PillarVortex };
            var stardustPillar = new NPC { active = true, boss = false, type = BossCursorController.PillarStardust };

            assert(BossCursorController.IsCelestialPillar(BossCursorController.PillarSolar), "PillarSolar recognized as celestial pillar");
            assert(BossCursorController.IsCelestialPillar(BossCursorController.PillarNebula), "PillarNebula recognized as celestial pillar");
            assert(BossCursorController.IsCelestialPillar(BossCursorController.PillarVortex), "PillarVortex recognized as celestial pillar");
            assert(BossCursorController.IsCelestialPillar(BossCursorController.PillarStardust), "PillarStardust recognized as celestial pillar");

            assert(!BossCursorController.IsBoss(solarPillar, config), "Solar Pillar excluded when BlacklistPillars=true");
            assert(!BossCursorController.IsBoss(nebulaPillar, config), "Nebula Pillar excluded when BlacklistPillars=true");

            var allowPillarsConfig = new BossCursorConfig { BlacklistPillars = false, IncludedNpcIds = new List<int> { BossCursorController.PillarSolar } };
            assert(BossCursorController.IsBoss(solarPillar, allowPillarsConfig), "Solar Pillar allowed when Whitelisted and BlacklistPillars=false");

            // 4. Custom Blacklist and Whitelist
            var customBlacklistConfig = new BossCursorConfig
            {
                ExcludedNpcIds = new List<int> { NPCID.EyeofCthulhu }
            };
            assert(!BossCursorController.IsBoss(bossNpc, customBlacklistConfig), "EyeofCthulhu excluded via ExcludedNpcIds config");

            var customWhitelistConfig = new BossCursorConfig
            {
                IncludedNpcIds = new List<int> { NPCID.DungeonGuardian }
            };
            var guardianNpc = new NPC { active = true, boss = false, type = NPCID.DungeonGuardian };
            assert(BossCursorController.IsBoss(guardianNpc, customWhitelistConfig), "DungeonGuardian detected when whitelisted in config");

            // 5. Runtime API Blacklist & Whitelist
            BossCursorAPI.AddToBlacklist(NPCID.Plantera);
            var plantera = new NPC { active = true, boss = true, type = NPCID.Plantera };
            assert(!BossCursorController.IsBoss(plantera, config), "Plantera excluded via RuntimeBlacklist API");

            BossCursorAPI.RemoveFromBlacklist(NPCID.Plantera);
            assert(BossCursorController.IsBoss(plantera, config), "Plantera restored after RemoveFromBlacklist API");

            BossCursorAPI.AddToWhitelist(NPCID.Zombie);
            var zombie = new NPC { active = true, boss = false, type = NPCID.Zombie };
            assert(BossCursorController.IsBoss(zombie, config), "Zombie tracked via RuntimeWhitelist API");

            BossCursorAPI.RemoveFromWhitelist(NPCID.Zombie);
            assert(!BossCursorController.IsBoss(zombie, config), "Zombie removed after RemoveFromWhitelist API");

            // 6. Mathematical Vector & Proximity Calculations
            Vector2 playerCenter = new Vector2(1000, 1000);
            Vector2 bossCenterRight = new Vector2(1500, 1000); // 500 px to the right
            Vector2 screenPos = new Vector2(0, 0);

            BossCursorController.CalculateCursorTransform(
                playerCenter,
                bossCenterRight,
                gravDir: 1.0f,
                screenWidth: 1920f,
                screenHeight: 1080f,
                uiScale: 1.0f,
                cursorDistance: 150f,
                cursorSize: 1.0f,
                screenPosition: screenPos,
                out Vector2 bossVector,
                out float rotation,
                out float modifier,
                out float alpha,
                out float scale,
                out Vector2 arrowPos,
                out Vector2 headPos);

            assert(Math.Abs(bossVector.X - 500f) < 0.01f && Math.Abs(bossVector.Y) < 0.01f, "Boss vector correctly calculated to the right");
            assert(Math.Abs(rotation) < 0.01f, "Rotation angle is 0 rad when pointing directly right");
            assert(modifier > 0.02f && modifier <= 1.0f, "Proximity modifier within valid range [0.02, 1.0]");
            assert(Math.Abs(alpha - (modifier * 0.9f)) < 0.001f, "Alpha is proportional to proximity modifier (modifier * 0.9)");
            assert(Math.Abs(scale - (modifier * 1.2f)) < 0.001f, "Scale is proportional to proximity modifier (modifier * 1.2)");
            assert(Math.Abs(arrowPos.X - 1150f) < 0.01f && Math.Abs(arrowPos.Y - 1000f) < 0.01f, "Arrow positioned 150px to the right of player");

            // Boss Directly Above
            Vector2 bossCenterAbove = new Vector2(1000, 500); // 500 px above
            BossCursorController.CalculateCursorTransform(
                playerCenter,
                bossCenterAbove,
                gravDir: 1.0f,
                screenWidth: 1920f,
                screenHeight: 1080f,
                uiScale: 1.0f,
                cursorDistance: 150f,
                cursorSize: 1.0f,
                screenPosition: screenPos,
                out _,
                out float rotationAbove,
                out _,
                out _,
                out _,
                out Vector2 arrowPosAbove,
                out _);

            assert(Math.Abs(rotationAbove - (-MathHelper.PiOver2)) < 0.01f, "Rotation angle is -Pi/2 rad when pointing directly up");
            assert(Math.Abs(arrowPosAbove.X - 1000f) < 0.01f && Math.Abs(arrowPosAbove.Y - 850f) < 0.01f, "Arrow positioned 150px above player");

            // 7. Gravitation Potion (Inverted Y) Math
            BossCursorController.CalculateCursorTransform(
                playerCenter,
                bossCenterAbove,
                gravDir: -1.0f, // upside down
                screenWidth: 1920f,
                screenHeight: 1080f,
                uiScale: 1.0f,
                cursorDistance: 150f,
                cursorSize: 1.0f,
                screenPosition: screenPos,
                out Vector2 invertedBossVec,
                out float invertedRotation,
                out _,
                out _,
                out _,
                out _,
                out _);

            assert(invertedBossVec.Y > 0, "Inverted gravitation flips direction vector Y to positive");
            assert(Math.Abs(invertedRotation - MathHelper.PiOver2) < 0.01f, "Inverted rotation flips pointing angle to +Pi/2 rad");

            // 8. Viewport Intersection (HideOnScreen)
            Main.screenPosition = new Vector2(1000, 1000);
            Main.screenWidth = 1920;
            Main.screenHeight = 1080;

            var onScreenNpc = new NPC { active = true, position = new Vector2(1500, 1500), width = 32, height = 32 };
            assert(BossCursorController.IsOnScreen(onScreenNpc), "NPC inside [1000..2920, 1000..2080] is detected on-screen");

            var offScreenNpc = new NPC { active = true, position = new Vector2(4000, 4000), width = 32, height = 32 };
            assert(!BossCursorController.IsOnScreen(offScreenNpc), "NPC at [4000, 4000] is detected off-screen");

            // 9. Texture Fallback & Headless Safety
            var cursorTex = BossCursorController.GetCursorTexture(null);
            // In unit tests Main.instance.GraphicsDevice is null, so it returns null safely without exception
            assert(cursorTex == null || !cursorTex.IsDisposed, "GetCursorTexture handles headless/null GraphicsDevice without crashing");

            var headTex = BossCursorController.GetHeadTexture(bossNpc);
            assert(headTex == null || !headTex.IsDisposed, "GetHeadTexture handles uninitialized head assets safely");

            // 10. Lifecycle and Reset
            var mod = new BossCursorMod();
            assert(BossCursorMod.Instance == null, "Instance is null before Initialize");

            // API Enabled Toggle
            BossCursorAPI.SetEnabled(true);
            assert(BossCursorAPI.IsEnabled() == false, "API IsEnabled returns false when mod instance config is null");

            BossCursorController.Reset();
            assert(BossCursorController.RuntimeBlacklist.Count == 0, "RuntimeBlacklist is empty after Reset");
            assert(BossCursorController.RuntimeWhitelist.Count == 0, "RuntimeWhitelist is empty after Reset");
        }
    }
}
