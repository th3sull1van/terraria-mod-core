using System;

namespace OreCascade
{
    /// <summary>
    /// Configuration data model for OreCascade plugin. Managed and persisted via TMC IConfigManager.
    /// </summary>
    public class CascadeConfig
    {
        public bool Enabled { get; set; } = true;
        public int MaxBlocksPerActivation { get; set; } = 100;
        public bool AllowDiagonalConnections { get; set; } = false;
        public bool RequireSameOreType { get; set; } = true;
        public bool IncludeGems { get; set; } = true;
        public bool IncludeExtractables { get; set; } = true;
    }
}
