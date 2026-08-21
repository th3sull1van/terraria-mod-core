namespace TerrariaModCore.API
{
    /// <summary>
    /// Represents the lifecycle state of a mod.
    /// </summary>
    public enum ModState
    {
        Discovered = 0,
        Validated = 1,
        Loaded = 2,
        Disabled = 3,
        Failed = 4,
        Unloaded = 5
    }
}
