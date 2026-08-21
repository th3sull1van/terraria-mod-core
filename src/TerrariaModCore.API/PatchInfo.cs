using System;
using System.Reflection;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Metadata describing a registered runtime patch.
    /// </summary>
    public class PatchInfo
    {
        public string ModId { get; set; }
        public MethodBase TargetMethod { get; set; }
        public MethodInfo PatchMethod { get; set; }
        public string PatchType { get; set; } // Prefix, Postfix, Transpiler, Finalizer
        public PatchPriority Priority { get; set; } = PatchPriority.Normal;
        public string Description { get; set; }

        public override string ToString()
        {
            string targetName = TargetMethod != null ? $"{TargetMethod.DeclaringType?.Name}.{TargetMethod.Name}" : "Unknown";
            return $"[{ModId}] {PatchType} on {targetName} (Priority: {Priority})";
        }
    }
}
