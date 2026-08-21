using System;
using System.Reflection;
using TerrariaModCore.API;

namespace TerrariaModCore.Compatibility
{
    /// <summary>
    /// Implements shared game state access and environment queries.
    /// </summary>
    public class GameServices : IGameServices
    {
        private string _gameVersion;

        public string GameVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_gameVersion))
                {
                    GameVersionChecker.ValidateTerrariaVersion(out _gameVersion);
                }
                return _gameVersion;
            }
        }

        public bool IsInWorld
        {
            get
            {
                try
                {
                    return !Terraria.Main.gameMenu && !Terraria.WorldGen.isGeneratingOrLoadingWorld && Terraria.Main.tile != null;
                }
                catch { return false; }
            }
        }

        public bool IsMultiplayer
        {
            get
            {
                try { return Terraria.Main.netMode != 0; }
                catch { return false; }
            }
        }

        public bool IsDedicatedServer
        {
            get
            {
                try { return Terraria.Main.dedServ; }
                catch { return false; }
            }
        }
    }
}
