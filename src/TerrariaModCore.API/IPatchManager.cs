using System;
using System.Collections.Generic;
using System.Reflection;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Centralized manager for registering, inspecting, applying, and unapplying Harmony patches.
    /// Prevents mods from creating rogue Harmony instances and guarantees conflict resolution and isolated unpatching.
    /// </summary>
    public interface IPatchManager
    {
        /// <summary>
        /// Registers all annotated patches from the calling mod's assembly.
        /// </summary>
        /// <param name="modId">The mod ID registering the patches.</param>
        /// <param name="assembly">The assembly containing [HarmonyPatch] annotated classes.</param>
        void RegisterAll(string modId, Assembly assembly);

        /// <summary>
        /// Registers a single prefix patch.
        /// </summary>
        void RegisterPrefix(string modId, MethodBase original, MethodInfo prefix, PatchPriority priority = PatchPriority.Normal);

        /// <summary>
        /// Registers a single postfix patch.
        /// </summary>
        void RegisterPostfix(string modId, MethodBase original, MethodInfo postfix, PatchPriority priority = PatchPriority.Normal);

        /// <summary>
        /// Registers a transpiler patch.
        /// </summary>
        void RegisterTranspiler(string modId, MethodBase original, MethodInfo transpiler, PatchPriority priority = PatchPriority.Normal);

        /// <summary>
        /// Removes all patches registered by the specified mod ID.
        /// </summary>
        /// <param name="modId">The mod ID whose patches should be removed.</param>
        void UnpatchAll(string modId);

        /// <summary>
        /// Gets all patches currently registered across all mods.
        /// </summary>
        IReadOnlyList<PatchInfo> GetAllPatches();

        /// <summary>
        /// Gets all patches registered by a specific mod.
        /// </summary>
        IReadOnlyList<PatchInfo> GetPatchesByMod(string modId);

        /// <summary>
        /// Gets all patches targeting a specific method.
        /// </summary>
        IReadOnlyList<PatchInfo> GetPatchesByTarget(MethodBase target);
    }
}
