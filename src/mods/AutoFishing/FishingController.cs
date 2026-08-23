using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;

namespace AutoFishing
{
    /// <summary>
    /// Coordinates fishing state, bite detection, auto-casting, and auto-reeling in sync with vanilla Terraria tick cycle.
    /// Automation only begins after the player makes their first manual cast click, and stops when manually reeled in.
    /// </summary>
    public static class FishingController
    {
        [ThreadStatic]
        private static bool _isInternalAction;

        private static bool _isAutomating = false;
        private static int _castTimer = 0;
        private static int _reelTimer = 0;
        private static int _lastSelectedItemIndex = -1;

        private static readonly MethodInfo PullBobbersMethod = typeof(Player).GetMethod(
            "ItemCheck_PullFishingBobbers",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Indicates whether automated continuous fishing is currently active.
        /// </summary>
        public static bool IsAutomating
        {
            get => _isAutomating;
            set => _isAutomating = value;
        }

        /// <summary>
        /// Thread-static reentrancy flag indicating whether a cast or reel action was triggered internally by the mod.
        /// </summary>
        public static bool IsInternalAction
        {
            get => _isInternalAction;
            set => _isInternalAction = value;
        }

        public static int CastTimer => _castTimer;
        public static int ReelTimer => _reelTimer;

        public static void Reset()
        {
            _isAutomating = false;
            _castTimer = 0;
            _reelTimer = 0;
            _lastSelectedItemIndex = -1;
        }

        /// <summary>
        /// Called when the player manually casts their fishing rod to engage automation.
        /// </summary>
        public static void OnManualCast(Player player, AutoFishingConfig config)
        {
            _isAutomating = true;
            _castTimer = config != null ? config.CastDelayTicks : 30;
            _reelTimer = 0;
        }

        /// <summary>
        /// Called when the player manually reels in / pulls their fishing line to cancel automation.
        /// </summary>
        public static void OnManualPull(Player player, AutoFishingConfig config)
        {
            _isAutomating = false;
            _castTimer = 0;
            _reelTimer = 0;
        }

        public static void Update(Player player, AutoFishingConfig config)
        {
            if (config == null || !config.Enabled || player == null)
            {
                return;
            }

            if (Main.gameMenu || WorldGen.isGeneratingOrLoadingWorld || player.dead || player.ghost)
            {
                Reset();
                return;
            }

            // Only run for active local player
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Detect slot change and reset state
            if (_lastSelectedItemIndex != player.selectedItem)
            {
                _lastSelectedItemIndex = player.selectedItem;
                _isAutomating = false;
                _castTimer = 0;
                _reelTimer = 0;
            }

            Item selectedItem = player.inventory[player.selectedItem];
            if (selectedItem == null || selectedItem.fishingPole <= 0)
            {
                Reset();
                return;
            }

            // Scan player's active bobbers
            var activeBobbers = GetActiveBobbers(player.whoAmI);

            if (activeBobbers.Count > 0)
            {
                _castTimer = config.CastDelayTicks;

                // Check for bite on any bobber
                bool biteDetected = false;
                for (int i = 0; i < activeBobbers.Count; i++)
                {
                    Projectile bobber = activeBobbers[i];
                    // In Terraria 1.4.5.7: ai[1] < 0 and localAI[1] != 0 indicates a bite has occurred
                    if (bobber.ai[1] < 0f && bobber.localAI[1] != 0f)
                    {
                        biteDetected = true;
                        break;
                    }
                }

                if (biteDetected && config.AutoReel)
                {
                    _reelTimer++;
                    if (_reelTimer >= config.ReelDelayTicks)
                    {
                        // Reel in using vanilla pull method with internal action flag
                        if (PullBobbersMethod != null)
                        {
                            try
                            {
                                _isInternalAction = true;
                                PullBobbersMethod.Invoke(player, new object[] { selectedItem });
                            }
                            finally
                            {
                                _isInternalAction = false;
                            }
                        }
                        _reelTimer = 0;
                        _castTimer = config.CastDelayTicks;
                    }
                }
                else
                {
                    _reelTimer = 0;
                }
            }
            else
            {
                _reelTimer = 0;

                // Only auto-cast if automation was started by user and AutoCast is enabled
                if (_isAutomating && config.AutoCast)
                {
                    // Verify if player has bait when required
                    if (config.RequireBait && !HasBait(player))
                    {
                        return;
                    }

                    if (_castTimer > 0)
                    {
                        _castTimer--;
                    }
                    else
                    {
                        try
                        {
                            _isInternalAction = true;
                            // Trigger cast by simulating controlUseItem and executing ItemCheck
                            player.controlUseItem = true;
                            player.releaseUseItem = true;
                            player.ItemCheck();
                        }
                        finally
                        {
                            _isInternalAction = false;
                        }
                        _castTimer = config.CastDelayTicks;
                    }
                }
            }
        }

        public static bool HasBait(Player player)
        {
            if (player == null || player.inventory == null) return false;
            for (int i = 0; i < 58; i++)
            {
                Item item = player.inventory[i];
                if (item != null && item.stack > 0 && item.bait > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Projectile> GetActiveBobbers(int ownerIndex)
        {
            var list = new List<Projectile>();
            if (Main.projectile == null) return list;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p != null && p.active && p.owner == ownerIndex && p.bobber)
                {
                    list.Add(p);
                }
            }
            return list;
        }
    }
}
