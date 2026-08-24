using System;
using System.Collections.Generic;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Declarative metadata schema loaded from mod's manifest.json.
    /// </summary>
    public class ModManifest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "1.1.0";
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EntryAssembly { get; set; } = string.Empty;
        public string EntryType { get; set; } = string.Empty;
        public string TargetGameVersion { get; set; } = "1.4.5.8";
        public string CoreVersion { get; set; } = "1.1.0";
        public bool Enabled { get; set; } = true;

        public List<string> Dependencies { get; set; } = new List<string>();
        public List<string> OptionalDependencies { get; set; } = new List<string>();
        public List<string> LoadBefore { get; set; } = new List<string>();
        public List<string> LoadAfter { get; set; } = new List<string>();
        public List<string> IncompatibleWith { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{Name} ({Id}) v{Version} [Enabled: {Enabled}]";
        }
    }
}
