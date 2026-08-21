using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Configuration manager providing strongly-typed configuration load, save, validation, and defaults.
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// Gets or loads the configuration of type <typeparamref name="T"/> from the default file (config.json).
        /// If the file does not exist, a default instance is created and saved.
        /// </summary>
        /// <typeparam name="T">Configuration data type (must have parameterless constructor).</typeparam>
        /// <returns>Loaded or default configuration instance.</returns>
        T Get<T>() where T : class, new();

        /// <summary>
        /// Gets or loads the configuration of type <typeparamref name="T"/> from the specified filename.
        /// </summary>
        /// <typeparam name="T">Configuration data type.</typeparam>
        /// <param name="fileName">File name relative to mod config directory.</param>
        /// <returns>Loaded or default configuration instance.</returns>
        T Get<T>(string fileName) where T : class, new();

        /// <summary>
        /// Saves the configuration instance to its default JSON file.
        /// </summary>
        /// <typeparam name="T">Configuration data type.</typeparam>
        /// <param name="config">Configuration instance to serialize.</param>
        void Save<T>(T config) where T : class;

        /// <summary>
        /// Saves the configuration instance to the specified JSON file.
        /// </summary>
        /// <typeparam name="T">Configuration data type.</typeparam>
        /// <param name="config">Configuration instance to serialize.</param>
        /// <param name="fileName">File name relative to mod config directory.</param>
        void Save<T>(T config, string fileName) where T : class;
    }
}
