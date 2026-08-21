using System;
using System.Collections.Generic;
using System.Linq;
using TerrariaModCore.API;

namespace TerrariaModCore.Dependencies
{
    /// <summary>
    /// Result structure produced by the dependency resolver.
    /// </summary>
    public class ResolutionResult
    {
        public bool Success { get; set; }
        public List<ModManifest> OrderedMods { get; set; } = new List<ModManifest>();
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>
    /// Resolves mod dependencies, validates incompatibilities, and calculates topological load order using Kahn's algorithm.
    /// </summary>
    public class DependencyResolver
    {
        private readonly ILogger _logger;

        public DependencyResolver(ILogger logger)
        {
            _logger = logger;
        }

        public ResolutionResult Resolve(IEnumerable<ModManifest> manifests)
        {
            var result = new ResolutionResult();
            var manifestMap = new Dictionary<string, ModManifest>(StringComparer.OrdinalIgnoreCase);

            // 1. Filter enabled manifests and register
            foreach (var m in manifests)
            {
                if (m == null || string.IsNullOrEmpty(m.Id)) continue;
                if (!m.Enabled) continue;

                if (manifestMap.ContainsKey(m.Id))
                {
                    result.Errors.Add($"Duplicate mod ID detected: '{m.Id}'. Only one instance can be loaded.");
                    result.Success = false;
                    return result;
                }
                manifestMap[m.Id] = m;
            }

            var graph = new DependencyGraph();
            foreach (var kvp in manifestMap)
            {
                graph.AddNode(kvp.Value);
            }

            // 2. Validate mandatory dependencies, optional dependencies, and incompatibilities
            foreach (var kvp in manifestMap)
            {
                var manifest = kvp.Value;
                string modId = manifest.Id;

                // IncompatibleWith
                if (manifest.IncompatibleWith != null)
                {
                    foreach (var incomp in manifest.IncompatibleWith)
                    {
                        if (manifestMap.ContainsKey(incomp))
                        {
                            result.Errors.Add($"Mod '{modId}' is declared incompatible with active mod '{incomp}'.");
                        }
                    }
                }

                // Mandatory Dependencies
                if (manifest.Dependencies != null)
                {
                    foreach (var dep in manifest.Dependencies)
                    {
                        if (!manifestMap.ContainsKey(dep))
                        {
                            result.Errors.Add($"Mod '{modId}' requires missing dependency '{dep}'.");
                        }
                        else
                        {
                            graph.AddEdge(dep, modId); // dep must load before modId
                        }
                    }
                }

                // Optional Dependencies
                if (manifest.OptionalDependencies != null)
                {
                    foreach (var optDep in manifest.OptionalDependencies)
                    {
                        if (manifestMap.ContainsKey(optDep))
                        {
                            graph.AddEdge(optDep, modId);
                        }
                    }
                }

                // LoadAfter
                if (manifest.LoadAfter != null)
                {
                    foreach (var after in manifest.LoadAfter)
                    {
                        if (manifestMap.ContainsKey(after))
                        {
                            graph.AddEdge(after, modId);
                        }
                    }
                }

                // LoadBefore
                if (manifest.LoadBefore != null)
                {
                    foreach (var before in manifest.LoadBefore)
                    {
                        if (manifestMap.ContainsKey(before))
                        {
                            graph.AddEdge(modId, before);
                        }
                    }
                }
            }

            if (result.Errors.Count > 0)
            {
                result.Success = false;
                return result;
            }

            // 3. Topological Sort (Kahn's Algorithm)
            var inDegree = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in graph.Nodes.Keys)
            {
                inDegree[node] = graph.ReverseEdges.ContainsKey(node) ? graph.ReverseEdges[node].Count : 0;
            }

            var queue = new Queue<string>();
            foreach (var kvp in inDegree)
            {
                if (kvp.Value == 0)
                {
                    queue.Enqueue(kvp.Key);
                }
            }

            var ordered = new List<ModManifest>();
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                ordered.Add(manifestMap[current]);

                if (graph.Edges.TryGetValue(current, out var children))
                {
                    foreach (var child in children)
                    {
                        inDegree[child]--;
                        if (inDegree[child] == 0)
                        {
                            queue.Enqueue(child);
                        }
                    }
                }
            }

            // 4. Cycle Detection
            if (ordered.Count < graph.Nodes.Count)
            {
                var cycleMembers = inDegree.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();
                result.Errors.Add($"Circular dependency detected involving mods: [{string.Join(", ", cycleMembers)}]");
                result.Success = false;
                return result;
            }

            result.Success = true;
            result.OrderedMods = ordered;
            return result;
        }
    }
}
