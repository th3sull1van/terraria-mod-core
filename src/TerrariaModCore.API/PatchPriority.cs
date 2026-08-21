using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Patch execution priority for resolving ordering when multiple mods target the same method.
    /// Higher priority patches execute first in Prefixes, and last in Postfixes.
    /// </summary>
    public enum PatchPriority
    {
        Lowest = 0,
        VeryLow = 100,
        Low = 200,
        Normal = 300,
        High = 400,
        VeryHigh = 500,
        Highest = 600,
        First = 700,
        Last = -1
    }
}
