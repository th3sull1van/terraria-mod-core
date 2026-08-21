using System.Collections.Generic;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Information entry representing a mod registered in TMC.
    /// </summary>
    public class ModInfo
    {
        public ModManifest Manifest { get; set; }
        public IMod Instance { get; set; }
        public IModContext Context { get; set; }
        public ModState State { get; set; }
        public string ErrorDetails { get; set; }
    }

    /// <summary>
    /// Central registry containing metadata and state of all discovered mods.
    /// </summary>
    public interface IModRegistry
    {
        /// <summary>
        /// Gets all discovered mods.
        /// </summary>
        IReadOnlyList<ModInfo> GetAllMods();

        /// <summary>
        /// Gets a specific mod entry by its mod ID.
        /// </summary>
        ModInfo GetMod(string id);

        /// <summary>
        /// Checks whether a mod is currently loaded and active.
        /// </summary>
        bool IsLoaded(string id);

        /// <summary>
        /// Checks whether a mod is enabled in its manifest.
        /// </summary>
        bool IsEnabled(string id);
    }
}
