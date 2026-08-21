using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TerrariaModCore.API;

namespace TerrariaModCore
{
    /// <summary>
    /// Implements central storage for discovered and active mod metadata.
    /// </summary>
    public class ModRegistry : IModRegistry
    {
        private readonly ConcurrentDictionary<string, ModInfo> _mods = new ConcurrentDictionary<string, ModInfo>(StringComparer.OrdinalIgnoreCase);

        public void Register(ModInfo modInfo)
        {
            if (modInfo?.Manifest == null || string.IsNullOrEmpty(modInfo.Manifest.Id)) return;
            _mods[modInfo.Manifest.Id] = modInfo;
        }

        public void UpdateState(string modId, ModState state, string errorDetails = null)
        {
            if (_mods.TryGetValue(modId, out var info))
            {
                info.State = state;
                if (!string.IsNullOrEmpty(errorDetails))
                {
                    info.ErrorDetails = errorDetails;
                }
            }
        }

        public IReadOnlyList<ModInfo> GetAllMods()
        {
            return _mods.Values.ToList();
        }

        public ModInfo GetMod(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            _mods.TryGetValue(id, out var info);
            return info;
        }

        public bool IsLoaded(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return _mods.TryGetValue(id, out var info) && info.State == ModState.Loaded;
        }

        public bool IsEnabled(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return _mods.TryGetValue(id, out var info) && info.Manifest != null && info.Manifest.Enabled;
        }
    }
}
