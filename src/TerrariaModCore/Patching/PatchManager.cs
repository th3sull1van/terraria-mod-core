using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TerrariaModCore.API;
using PatchInfo = TerrariaModCore.API.PatchInfo;

namespace TerrariaModCore.Patching
{
    /// <summary>
    /// Centralized Harmony patch manager. Controls a single Harmony instance, tracks ownership of patches by mod ID,
    /// evaluates conflicts, and cleanly rolls back patches when a mod unloads.
    /// </summary>
    public class PatchManager : IPatchManager
    {
        public const string CentralHarmonyId = "com.tmc.host.patcher";

        private readonly Harmony _harmony;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        private readonly List<PatchInfo> _allPatches = new List<PatchInfo>();
        private readonly Dictionary<string, List<PatchInfo>> _patchesByMod = new Dictionary<string, List<PatchInfo>>(StringComparer.OrdinalIgnoreCase);

        public Harmony HarmonyInstance => _harmony;

        public PatchManager(ILogger logger)
        {
            _logger = logger;
            _harmony = new Harmony(CentralHarmonyId);
        }

        public void RegisterAll(string modId, Assembly assembly)
        {
            if (string.IsNullOrEmpty(modId)) throw new ArgumentNullException(nameof(modId));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            lock (_lock)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }

                foreach (var type in types)
                {
                    try
                    {
                        var patchAttrs = type.GetCustomAttributes(typeof(HarmonyPatch), true).Cast<HarmonyPatch>().ToList();
                        if (patchAttrs.Count == 0) continue;

                        // Check if type has Prefix, Postfix, Transpiler methods
                        MethodInfo prefixMethod = null;
                        MethodInfo postfixMethod = null;
                        MethodInfo transpilerMethod = null;

                        int priority = (int)PatchPriority.Normal;
                        var prioAttr = type.GetCustomAttribute<HarmonyPriority>();
                        if (prioAttr != null && prioAttr.info != null) priority = prioAttr.info.priority;

                        foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (m.GetCustomAttribute<HarmonyPrefix>() != null || m.Name.Equals("Prefix", StringComparison.OrdinalIgnoreCase))
                                prefixMethod = m;
                            if (m.GetCustomAttribute<HarmonyPostfix>() != null || m.Name.Equals("Postfix", StringComparison.OrdinalIgnoreCase))
                                postfixMethod = m;
                            if (m.GetCustomAttribute<HarmonyTranspiler>() != null || m.Name.Equals("Transpiler", StringComparison.OrdinalIgnoreCase))
                                transpilerMethod = m;
                        }

                        // Determine target method(s) from class attributes
                        MethodBase target = ResolveTargetMethod(patchAttrs);
                        if (target == null)
                        {
                            _logger?.Warning($"[Patch Manager] Could not resolve target method for patch class {type.FullName} in mod '{modId}'.");
                            continue;
                        }

                        var hPrefix = prefixMethod != null ? new HarmonyMethod(prefixMethod, priority) : null;
                        var hPostfix = postfixMethod != null ? new HarmonyMethod(postfixMethod, priority) : null;
                        var hTranspiler = transpilerMethod != null ? new HarmonyMethod(transpilerMethod, priority) : null;

                        _harmony.Patch(target, hPrefix, hPostfix, hTranspiler);

                        TrackPatch(modId, target, "Prefix", type, priority, prefixMethod);
                        TrackPatch(modId, target, "Postfix", type, priority, postfixMethod);
                        TrackPatch(modId, target, "Transpiler", type, priority, transpilerMethod);

                        _logger?.Debug($"[Patch Manager] Successfully patched {target.DeclaringType?.Name}.{target.Name} for mod '{modId}'.");
                    }
                    catch (Exception ex)
                    {
                        _logger?.Error($"[Patch Manager] Failed to apply patch class {type.FullName} for mod '{modId}'", ex);
                        throw;
                    }
                }
            }
        }

        public void RegisterPrefix(string modId, MethodBase original, MethodInfo prefix, PatchPriority priority = PatchPriority.Normal)
            => RegisterPatch(modId, original, prefix, "Prefix", priority);

        public void RegisterPostfix(string modId, MethodBase original, MethodInfo postfix, PatchPriority priority = PatchPriority.Normal)
            => RegisterPatch(modId, original, postfix, "Postfix", priority);

        public void RegisterTranspiler(string modId, MethodBase original, MethodInfo transpiler, PatchPriority priority = PatchPriority.Normal)
            => RegisterPatch(modId, original, transpiler, "Transpiler", priority);

        private void RegisterPatch(string modId, MethodBase original, MethodInfo patchMethod, string patchType, PatchPriority priority)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (patchMethod == null) throw new ArgumentNullException(nameof(patchMethod));

            lock (_lock)
            {
                var hMethod = new HarmonyMethod(patchMethod, (int)priority);
                switch (patchType)
                {
                    case "Prefix": _harmony.Patch(original, prefix: hMethod); break;
                    case "Postfix": _harmony.Patch(original, postfix: hMethod); break;
                    default: _harmony.Patch(original, transpiler: hMethod); break;
                }

                RegisterInternal(new PatchInfo
                {
                    ModId = modId,
                    TargetMethod = original,
                    PatchMethod = patchMethod,
                    PatchType = patchType,
                    Priority = priority,
                    Description = $"{patchMethod.DeclaringType?.Name}.{patchMethod.Name}"
                });
            }
        }

        public void UnpatchAll(string modId)
        {
            if (string.IsNullOrEmpty(modId)) return;

            lock (_lock)
            {
                if (!_patchesByMod.TryGetValue(modId, out var patches) || patches.Count == 0)
                {
                    return;
                }

                foreach (var patch in patches)
                {
                    try
                    {
                        if (patch.TargetMethod != null && patch.PatchMethod != null)
                        {
                            _harmony.Unpatch(patch.TargetMethod, patch.PatchMethod);
                            _logger?.Debug($"[Patch Manager] Unpatched {patch.Description} for mod '{modId}'.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.Warning($"[Patch Manager] Error unpatching {patch.Description} for mod '{modId}': {ex.Message}");
                    }
                }

                _allPatches.RemoveAll(p => string.Equals(p.ModId, modId, StringComparison.OrdinalIgnoreCase));
                _patchesByMod.Remove(modId);
            }
        }

        public IReadOnlyList<PatchInfo> GetAllPatches()
        {
            lock (_lock) { return _allPatches.ToList(); }
        }

        public IReadOnlyList<PatchInfo> GetPatchesByMod(string modId)
        {
            lock (_lock)
            {
                if (_patchesByMod.TryGetValue(modId, out var list)) return list.ToList();
                return new List<PatchInfo>();
            }
        }

        private void TrackPatch(string modId, MethodBase target, string patchType, Type patchClass, int priority, MethodInfo method)
        {
            if (method == null) return;

            RegisterInternal(new PatchInfo
            {
                ModId = modId,
                TargetMethod = target,
                PatchMethod = method,
                PatchType = patchType,
                Priority = (PatchPriority)priority,
                Description = $"{patchClass.Name}.{method.Name}"
            });
        }

        private void RegisterInternal(PatchInfo info)
        {
            // Shared-hook notice: Harmony resolves overlaps by priority; logged for traceability.
            var otherMods = _allPatches
                .Where(p => p.TargetMethod == info.TargetMethod && !string.Equals(p.ModId, info.ModId, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.ModId)
                .Distinct()
                .ToList();
            if (otherMods.Count > 0)
            {
                string methodName = $"{info.TargetMethod.DeclaringType?.Name}.{info.TargetMethod.Name}";
                _logger?.Info($"[Patch Manager] Shared hook on '{methodName}'. Active mods touching this method: [{string.Join(", ", otherMods)}, {info.ModId}]. Resolution: Priority-based execution order.");
            }

            _allPatches.Add(info);

            if (!_patchesByMod.TryGetValue(info.ModId, out var list))
            {
                list = new List<PatchInfo>();
                _patchesByMod[info.ModId] = list;
            }
            list.Add(info);
        }

        private MethodBase ResolveTargetMethod(List<HarmonyPatch> attrs)
        {
            Type targetType = null;
            string methodName = null;
            Type[] argumentTypes = null;

            foreach (var attr in attrs)
            {
                var info = attr.info;
                if (info != null)
                {
                    if (info.declaringType != null) targetType = info.declaringType;
                    if (!string.IsNullOrEmpty(info.methodName)) methodName = info.methodName;
                    if (info.argumentTypes != null) argumentTypes = info.argumentTypes;
                }
            }

            if (targetType == null || string.IsNullOrEmpty(methodName)) return null;

            if (methodName == ".ctor")
            {
                return argumentTypes != null
                    ? targetType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, argumentTypes, null)
                    : targetType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
            }

            return argumentTypes != null
                ? targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, argumentTypes, null)
                : targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}
