using System.Collections.Generic;
using Terraria.ID;

namespace OreCascade
{
    /// <summary>
    /// Classifies tiles into ores, gems, and extractable blocks (Silt, Slush, Desert Fossil) and evaluates vein matching rules.
    /// </summary>
    public static class OreClassifier
    {
        private static readonly HashSet<ushort> StandardOres = new HashSet<ushort>
        {
            TileID.Copper,          // 7
            TileID.Tin,             // 166
            TileID.Iron,            // 6
            TileID.Lead,            // 167
            TileID.Silver,          // 9
            TileID.Tungsten,        // 168
            TileID.Gold,            // 8
            TileID.Platinum,        // 169
            TileID.Demonite,        // 22
            TileID.Crimtane,        // 204
            TileID.Meteorite,       // 37
            TileID.Obsidian,        // 56
            TileID.Hellstone,       // 58
            TileID.Cobalt,          // 107
            TileID.Palladium,       // 221
            TileID.Mythril,         // 108
            TileID.Orichalcum,      // 222
            TileID.Adamantite,      // 111
            TileID.Titanium,        // 223
            TileID.Chlorophyte,     // 211
            TileID.DesertFossil,    // 404
            TileID.FossilOre,       // 407
            TileID.LunarOre         // 408
        };

        private static readonly HashSet<ushort> Extractables = new HashSet<ushort>
        {
            TileID.Silt,            // 123
            TileID.Slush            // 224
        };

        private static readonly HashSet<ushort> Gems = new HashSet<ushort>
        {
            TileID.Sapphire,        // 63
            TileID.Ruby,            // 64
            TileID.Emerald,         // 65
            TileID.Topaz,           // 66
            TileID.Amethyst,        // 67
            TileID.Diamond,         // 68
            TileID.AmberStoneBlock, // 566
            178                     // Gemstones embedded on stone
        };

        public static bool IsOre(ushort tileType) => StandardOres.Contains(tileType);

        public static bool IsExtractable(ushort tileType) => Extractables.Contains(tileType);

        public static bool IsGem(ushort tileType) => Gems.Contains(tileType);

        public static bool IsEligible(ushort tileType, CascadeConfig config)
        {
            if (StandardOres.Contains(tileType))
            {
                return true;
            }

            if (config != null && config.IncludeExtractables && Extractables.Contains(tileType))
            {
                return true;
            }

            if (config != null && config.IncludeGems && Gems.Contains(tileType))
            {
                return true;
            }

            return false;
        }

        public static bool IsMatching(ushort initialType, ushort targetType, CascadeConfig config)
        {
            if (!IsEligible(targetType, config))
            {
                return false;
            }

            if (config == null || config.RequireSameOreType)
            {
                if (initialType == targetType) return true;

                // DesertFossil and FossilOre match each other as fossil variants
                if ((initialType == TileID.DesertFossil || initialType == TileID.FossilOre) &&
                    (targetType == TileID.DesertFossil || targetType == TileID.FossilOre))
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
