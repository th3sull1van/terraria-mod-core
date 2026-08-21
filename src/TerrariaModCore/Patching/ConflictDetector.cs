using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TerrariaModCore.API;

namespace TerrariaModCore.Patching
{
    /// <summary>
    /// Analyzes registered patches across mods to detect collisions on shared target methods.
    /// Logs detailed diagnosis and confirms priority-based resolution.
    /// </summary>
    public class ConflictDetector
    {
        private readonly ILogger _logger;

        public ConflictDetector(ILogger logger)
        {
            _logger = logger;
        }

        public void CheckConflict(PatchInfo newPatch, IReadOnlyList<PatchInfo> existingPatches)
        {
            if (newPatch?.TargetMethod == null) return;

            var existingTargetPatches = existingPatches
                .Where(p => p.TargetMethod == newPatch.TargetMethod && !string.Equals(p.ModId, newPatch.ModId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (existingTargetPatches.Count > 0)
            {
                string methodName = $"{newPatch.TargetMethod.DeclaringType?.Name}.{newPatch.TargetMethod.Name}";
                var otherMods = string.Join(", ", existingTargetPatches.Select(p => p.ModId).Distinct());

                _logger?.Info($"[Patch Manager] Shared hook detected on method '{methodName}'. Active mods touching this method: [{otherMods}, {newPatch.ModId}]. Resolution: Priority-based execution order.");
            }
        }
    }
}
