using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using TerrariaModCore.API;

namespace TerrariaModCore.Configuration
{
    /// <summary>
    /// Implements per-mod strongly typed configuration persistence with defaults fallback and caching.
    /// </summary>
    public class ModConfigManager : IConfigManager
    {
        private readonly string _configDirectory;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public ModConfigManager(string configDirectory, ILogger logger)
        {
            _configDirectory = configDirectory;
            _logger = logger;

            try
            {
                if (!Directory.Exists(_configDirectory))
                {
                    Directory.CreateDirectory(_configDirectory);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to create config directory: {_configDirectory}", ex);
            }
        }

        public T Get<T>() where T : class, new()
        {
            return Get<T>("config.json");
        }

        public T Get<T>(string fileName) where T : class, new()
        {
            string key = typeof(T).FullName + ":" + fileName;
            if (_cache.TryGetValue(key, out object cached) && cached is T casted)
            {
                return casted;
            }

            string filePath = Path.Combine(_configDirectory, fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);
                    T loaded = SimpleJson.Deserialize<T>(json);
                    if (loaded != null)
                    {
                        _cache[key] = loaded;
                        _logger?.Debug($"Configuration loaded successfully from {fileName}");
                        return loaded;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Warning($"Failed to parse {fileName} ({ex.Message}). Creating default configuration.");
                }
            }

            // Fallback: create default instance and save it
            var defaultInstance = new T();
            Save(defaultInstance, fileName);
            _cache[key] = defaultInstance;
            return defaultInstance;
        }

        public void Save<T>(T config) where T : class
        {
            Save(config, "config.json");
        }

        public void Save<T>(T config, string fileName) where T : class
        {
            if (config == null) return;
            string key = typeof(T).FullName + ":" + fileName;
            _cache[key] = config;

            string filePath = Path.Combine(_configDirectory, fileName);
            try
            {
                string json = SimpleJson.Serialize(config, true);
                File.WriteAllText(filePath, json, Encoding.UTF8);
                _logger?.Debug($"Configuration saved to {fileName}");
            }
            catch (Exception ex)
            {
                _logger?.Error($"Failed to save configuration to {fileName}", ex);
            }
        }
    }
}
