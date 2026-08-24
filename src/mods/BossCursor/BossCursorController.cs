using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace BossCursor
{
    /// <summary>
    /// Core state machine and rendering controller for BossCursor.
    /// Handles boss discovery, proximity mathematics, screen transformation, asset loading, and hotkey toggling.
    /// </summary>
    public static class BossCursorController
    {
        public const float DefaultHeadDistance = 45f;
        public const float HeadDistance = 45f;

        // Celestial Pillars NPC IDs
        public const int PillarSolar = 422;
        public const int PillarNebula = 493;
        public const int PillarVortex = 507;
        public const int PillarStardust = 517;

        private static readonly HashSet<int> _runtimeBlacklist = new HashSet<int>();
        private static readonly Dictionary<int, Texture2D> _runtimeWhitelist = new Dictionary<int, Texture2D>();

        private static Texture2D _cursorTexture;
        private static Texture2D _fallbackTexture;

        public static HashSet<int> RuntimeBlacklist => _runtimeBlacklist;
        public static Dictionary<int, Texture2D> RuntimeWhitelist => _runtimeWhitelist;

        /// <summary>
        /// Resets all runtime caches and clears state.
        /// </summary>
        public static void Reset()
        {
            _runtimeBlacklist.Clear();
            _runtimeWhitelist.Clear();
            _cursorTexture?.Dispose();
            _cursorTexture = null;
            _fallbackTexture?.Dispose();
            _fallbackTexture = null;
            _npcHeadBossArray = null;
            _npcArray = null;
            _assetValueProperty = null;
        }

        /// <summary>
        /// Checks whether a given NPC type is one of the four Celestial Pillars.
        /// </summary>
        public static bool IsCelestialPillar(int npcType)
        {
            return npcType == PillarSolar ||
                   npcType == PillarNebula ||
                   npcType == PillarVortex ||
                   npcType == PillarStardust;
        }

        /// <summary>
        /// Determines if an NPC is considered a boss according to the mod rules and active configurations.
        /// </summary>
        public static bool IsBoss(NPC npc, BossCursorConfig config = null)
        {
            if (npc == null || !npc.active || npc.dontCountMe)
                return false;

            if (_runtimeBlacklist.Contains(npc.type))
                return false;

            if (config?.ExcludedNpcIds != null && config.ExcludedNpcIds.Contains(npc.type))
                return false;

            if (config != null && config.BlacklistPillars && IsCelestialPillar(npc.type))
                return false;

            if (_runtimeWhitelist.ContainsKey(npc.type))
                return true;

            if (config?.IncludedNpcIds != null && config.IncludedNpcIds.Contains(npc.type))
                return true;

            if (npc.boss)
                return true;

            try
            {
                int headIndex = npc.GetBossHeadTextureIndex();
                if (headIndex != -1)
                    return true;
            }
            catch
            {
                // Headless test or uninitialized NPCID sets safety
            }

            return false;
        }

        /// <summary>
        /// Scans all active NPCs in the world and returns a list of active bosses.
        /// </summary>
        public static List<NPC> GetActiveBosses(BossCursorConfig config = null)
        {
            var result = new List<NPC>();
            if (Main.npc == null) return result;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (IsBoss(npc, config))
                {
                    result.Add(npc);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if an NPC is currently within the active screen camera viewport.
        /// </summary>
        public static bool IsOnScreen(NPC npc)
        {
            if (npc == null) return false;

            float uiScale = 1.0f;
            try
            {
                uiScale = Main.UIScale > 0f ? Main.UIScale : 1f;
            }
            catch { }

            Vector2 p = npc.Center - Main.screenPosition;
            float screenW = Main.screenWidth * uiScale;
            float screenH = Main.screenHeight * uiScale;

            return p.X >= 0 && p.Y >= 0 && p.X <= screenW && p.Y <= screenH;
        }

        /// <summary>
        /// Calculates directional vectors, proximity modifiers, rotation angles, and screen positions for drawing.
        /// </summary>
        public static void CalculateCursorTransform(
            Vector2 playerCenter,
            Vector2 bossCenter,
            float gravDir,
            float screenWidth,
            float screenHeight,
            float uiScale,
            float cursorDistance,
            float cursorSize,
            Vector2 screenPosition,
            out Vector2 bossVector,
            out float rotation,
            out float modifier,
            out float alpha,
            out float scale,
            out Vector2 arrowPos,
            out Vector2 headPos,
            float headDistance = DefaultHeadDistance)
        {
            bossVector = bossCenter - playerCenter;

            // Invert Y direction if under gravitation potion effect
            if (gravDir == -1f)
            {
                bossVector.Y *= -1f;
            }

            float distance = bossVector.Length();
            float safeScreenWidth = screenWidth > 0f ? screenWidth : 1920f;
            modifier = MathHelper.Clamp(1.15f - (1f / (2f * safeScreenWidth)) * distance, 0.02f, 1f);
            alpha = modifier * 0.9f;
            scale = modifier * 1.2f;

            Vector2 dir = distance > 0.0001f ? Vector2.Normalize(bossVector) : new Vector2(0, -1);
            rotation = (float)Math.Atan2(dir.Y, dir.X);

            Vector2 playerScreenPos = playerCenter - screenPosition;
            if (gravDir == -1f)
            {
                playerScreenPos.Y = screenHeight - playerScreenPos.Y;
            }

            float effectiveUiScale = uiScale > 0f ? uiScale : 1f;
            float posScaleFactor = 1f / effectiveUiScale;

            arrowPos = (playerScreenPos + dir * cursorDistance) * posScaleFactor;
            headPos = (playerScreenPos + dir * (cursorDistance - (headDistance * effectiveUiScale) * cursorSize)) * posScaleFactor;
        }

        /// <summary>
        /// Loads the cursor arrow texture from embedded resources or fallback.
        /// </summary>
        public static Texture2D GetCursorTexture(GraphicsDevice graphicsDevice = null)
        {
            if (_cursorTexture != null && !_cursorTexture.IsDisposed)
                return _cursorTexture;

            var gd = graphicsDevice ?? Main.instance?.GraphicsDevice;
            if (gd == null)
                return null;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("BossCursor.UI.Cursor.png"))
                {
                    if (stream != null)
                    {
                        _cursorTexture = Texture2D.FromStream(gd, stream);
                        return _cursorTexture;
                    }
                }
            }
            catch { }

            // Procedural fallback texture (20x20 white arrow shape)
            try
            {
                if (_fallbackTexture == null || _fallbackTexture.IsDisposed)
                {
                    _fallbackTexture = new Texture2D(gd, 20, 20);
                    Color[] data = new Color[20 * 20];
                    for (int y = 0; y < 20; y++)
                    {
                        for (int x = 0; x < 20; x++)
                        {
                            // Draw a triangular arrowhead pointing right
                            if (x >= 4 && y >= 10 - (x - 4) && y <= 10 + (x - 4) && x <= 16)
                            {
                                data[y * 20 + x] = Color.White;
                            }
                            else
                            {
                                data[y * 20 + x] = Color.Transparent;
                            }
                        }
                    }
                    _fallbackTexture.SetData(data);
                }
                return _fallbackTexture;
            }
            catch
            {
                return null;
            }
        }

        private static Array _npcHeadBossArray;
        private static Array _npcArray;
        private static PropertyInfo _assetValueProperty;

        /// <summary>
        /// Safely extracts the Texture2D from a ReLogic Asset object without compile-time ReLogic dependency.
        /// </summary>
        private static Texture2D ExtractAssetValue(object asset)
        {
            if (asset == null) return null;

            try
            {
                if (_assetValueProperty == null)
                {
                    _assetValueProperty = asset.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                }
                return _assetValueProperty?.GetValue(asset, null) as Texture2D;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the head texture for the given boss.
        /// </summary>
        public static Texture2D GetHeadTexture(NPC boss)
        {
            if (boss == null) return null;

            if (_runtimeWhitelist.TryGetValue(boss.type, out var customTex) && customTex != null)
                return customTex;

            int headIndex = -1;
            try
            {
                headIndex = boss.GetBossHeadTextureIndex();
            }
            catch { }

            if (_npcHeadBossArray == null)
            {
                var field = typeof(TextureAssets).GetField("NpcHeadBoss", BindingFlags.Public | BindingFlags.Static);
                _npcHeadBossArray = field?.GetValue(null) as Array;
            }

            if (headIndex >= 0 && _npcHeadBossArray != null && headIndex < _npcHeadBossArray.Length)
            {
                object asset = _npcHeadBossArray.GetValue(headIndex);
                var tex = ExtractAssetValue(asset);
                if (tex != null)
                    return tex;
            }

            // Fallback to NPC sprite texture if head texture is not available
            if (_npcArray == null)
            {
                var field = typeof(TextureAssets).GetField("Npc", BindingFlags.Public | BindingFlags.Static);
                _npcArray = field?.GetValue(null) as Array;
            }

            if (_npcArray != null && boss.type >= 0 && boss.type < _npcArray.Length)
            {
                object npcAsset = _npcArray.GetValue(boss.type);
                var tex = ExtractAssetValue(npcAsset);
                if (tex != null)
                    return tex;
            }

            return null;
        }

        /// <summary>
        /// Renders all active boss cursors and head icons onto the active SpriteBatch.
        /// </summary>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null || Main.gameMenu || Main.dedServ)
                return;

            // Fullscreen map open hides the cursors
            if (Main.mapStyle == 2)
                return;

            var config = BossCursorMod.Instance?.Config;
            if (config == null || !config.Enabled)
                return;

            if (Main.myPlayer < 0 || Main.myPlayer >= Main.player.Length)
                return;

            Player player = Main.player[Main.myPlayer];
            if (player == null || !player.active || player.dead)
                return;

            List<NPC> bosses = GetActiveBosses(config);
            if (bosses.Count == 0)
                return;

            Texture2D cursorTex = GetCursorTexture();
            Vector2 playerCenter = player.Center;
            float gravDir = player.gravDir;
            float screenW = Main.screenWidth;
            float screenH = Main.screenHeight;
            float uiScale = 1.0f;
            try { uiScale = Main.UIScale > 0f ? Main.UIScale : 1f; } catch { }
            Vector2 screenPos = Main.screenPosition;

            foreach (var boss in bosses)
            {
                if (config.HideOnScreen && IsOnScreen(boss))
                    continue;

                float headOffset = config.HeadOffset > 0f ? config.HeadOffset : DefaultHeadDistance;

                CalculateCursorTransform(
                    playerCenter,
                    boss.Center,
                    gravDir,
                    screenW,
                    screenH,
                    uiScale,
                    config.CursorDistance,
                    config.CursorSize,
                    screenPos,
                    out _,
                    out float rotation,
                    out _,
                    out float alpha,
                    out float scale,
                    out Vector2 arrowPos,
                    out Vector2 headPos,
                    headOffset);

                // 1. Draw Boss Head Icon (rendered underneath the arrow)
                Texture2D headTex = GetHeadTexture(boss);
                if (headTex != null)
                {
                    Vector2 headOrigin = new Vector2(headTex.Width * 0.5f, headTex.Height * 0.5f);
                    spriteBatch.Draw(
                        headTex,
                        headPos,
                        null,
                        Color.White * alpha,
                        0f,
                        headOrigin,
                        scale * config.CursorSize,
                        boss.GetBossHeadSpriteEffects(),
                        0f);
                }

                // 2. Draw Directional Arrow (rendered on top in front of the boss head icon)
                if (cursorTex != null)
                {
                    Vector2 origin = new Vector2(cursorTex.Width * 0.5f, cursorTex.Height * 0.5f);
                    spriteBatch.Draw(
                        cursorTex,
                        arrowPos,
                        null,
                        Color.White * alpha,
                        rotation,
                        origin,
                        1.2f * config.CursorSize,
                        SpriteEffects.None,
                        0f);
                }
            }
        }

        /// <summary>
        /// Evaluates keyboard input to handle hotkey toggling.
        /// </summary>
        public static void UpdateInput()
        {
            if (Main.gameMenu || Main.dedServ || Main.drawingPlayerChat || Main.editSign || Main.editChest)
                return;

            var config = BossCursorMod.Instance?.Config;
            if (config == null || string.IsNullOrWhiteSpace(config.ToggleKey) || config.ToggleKey.Equals("None", StringComparison.OrdinalIgnoreCase))
                return;

            if (Enum.TryParse<Keys>(config.ToggleKey, true, out var key))
            {
                if (Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key))
                {
                    config.Enabled = !config.Enabled;
                    try
                    {
                        BossCursorMod.Instance.Context?.ConfigManager?.Save(config);
                    }
                    catch { }

                    string msg = config.Enabled ? "Boss Cursor enabled" : "Boss Cursor disabled";
                    Main.NewText(msg, Color.Cyan);
                }
            }
        }
    }
}
