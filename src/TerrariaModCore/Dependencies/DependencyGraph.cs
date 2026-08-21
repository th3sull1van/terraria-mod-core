using System;
using System.Collections.Generic;
using TerrariaModCore.API;

namespace TerrariaModCore.Dependencies
{
    /// <summary>
    /// Directed graph representing mod dependencies and load ordering constraints.
    /// </summary>
    public class DependencyGraph
    {
        public Dictionary<string, ModManifest> Nodes { get; } = new Dictionary<string, ModManifest>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, HashSet<string>> Edges { get; } = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase); // Parent -> Children (Parent must load before Child)
        public Dictionary<string, HashSet<string>> ReverseEdges { get; } = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase); // Child -> Parents (Child depends on Parent)

        public void AddNode(ModManifest manifest)
        {
            if (manifest == null || string.IsNullOrEmpty(manifest.Id)) return;
            Nodes[manifest.Id] = manifest;

            if (!Edges.ContainsKey(manifest.Id))
                Edges[manifest.Id] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!ReverseEdges.ContainsKey(manifest.Id))
                ReverseEdges[manifest.Id] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddEdge(string fromParent, string toChild)
        {
            if (!Edges.ContainsKey(fromParent))
                Edges[fromParent] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!ReverseEdges.ContainsKey(toChild))
                ReverseEdges[toChild] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Edges[fromParent].Add(toChild);
            ReverseEdges[toChild].Add(fromParent);
        }
    }
}
