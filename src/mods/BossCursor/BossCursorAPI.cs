using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BossCursor
{
    /// <summary>
    /// Public API for BossCursor to allow inter-mod communication and custom enemy registration.
    /// </summary>
    public static class BossCursorAPI
    {
        /// <summary>
        /// Adds an NPC ID to the runtime blacklist so it will not have a cursor drawn.
        /// </summary>
        public static void AddToBlacklist(int npcId)
        {
            BossCursorController.RuntimeBlacklist.Add(npcId);
        }

        /// <summary>
        /// Removes an NPC ID from the runtime blacklist.
        /// </summary>
        public static void RemoveFromBlacklist(int npcId)
        {
            BossCursorController.RuntimeBlacklist.Remove(npcId);
        }

        /// <summary>
        /// Adds an NPC ID to the runtime whitelist so a cursor will always be drawn for it, with an optional custom head texture.
        /// </summary>
        public static void AddToWhitelist(int npcId, Texture2D headTexture = null)
        {
            BossCursorController.RuntimeWhitelist[npcId] = headTexture;
        }

        /// <summary>
        /// Removes an NPC ID from the runtime whitelist.
        /// </summary>
        public static void RemoveFromWhitelist(int npcId)
        {
            BossCursorController.RuntimeWhitelist.Remove(npcId);
        }

        /// <summary>
        /// Checks whether the specified NPC is currently tracked by BossCursor.
        /// </summary>
        public static bool IsBossTracked(NPC npc)
        {
            return BossCursorController.IsBoss(npc, BossCursorMod.Instance?.Config);
        }

        /// <summary>
        /// Enables or disables BossCursor at runtime.
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            if (BossCursorMod.Instance?.Config != null)
            {
                BossCursorMod.Instance.Config.Enabled = enabled;
            }
        }

        /// <summary>
        /// Gets whether BossCursor is currently enabled.
        /// </summary>
        public static bool IsEnabled()
        {
            return BossCursorMod.Instance?.Config?.Enabled ?? false;
        }
    }
}
