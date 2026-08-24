using System;
using System.IO;
using TerrariaModCore.Configuration;

namespace TerrariaModCore.Tests
{
    public class SampleConfig
    {
        public bool Enabled { get; set; } = true;
        public int Count { get; set; } = 42;
        public float Multiplier { get; set; } = 3.14f;
        public string Title { get; set; } = "TerrariaModCore";
    }

    public static class ConfigManagerTests
    {
        public static void Run(Action<bool, string> assert)
        {
            Console.WriteLine("\n--- Testing ConfigManager & SimpleJson ---");

            // Test 1: SimpleJson Serialization / Deserialization
            var sample = new SampleConfig
            {
                Enabled = false,
                Count = 99,
                Multiplier = 2.5f,
                Title = "TestConfig"
            };

            string json = SimpleJson.Serialize(sample, true);
            var parsed = SimpleJson.Deserialize<SampleConfig>(json);

            assert(parsed != null, "SimpleJson successfully parsed SampleConfig");
            assert(parsed.Enabled == false, "Parsed Enabled is false");
            assert(parsed.Count == 99, "Parsed Count is 99");
            assert(Math.Abs(parsed.Multiplier - 2.5f) < 0.001f, "Parsed Multiplier is 2.5f");
            assert(parsed.Title == "TestConfig", "Parsed Title is 'TestConfig'");

            // Test 2: ModConfigManager Persistence
            string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_config_dir");
            if (Directory.Exists(testDir)) Directory.Delete(testDir, true);

            var configMgr = new ModConfigManager(testDir, null);
            var loadedDefault = configMgr.Get<SampleConfig>();

            assert(loadedDefault != null, "ModConfigManager creates default config if missing");
            assert(loadedDefault.Count == 42, "Default count is 42");
            assert(File.Exists(Path.Combine(testDir, "config.json")), "config.json file created on disk");

            // Test 3: Modify and Save
            loadedDefault.Count = 1000;
            configMgr.Save(loadedDefault);

            var reloaded = new ModConfigManager(testDir, null).Get<SampleConfig>();
            assert(reloaded.Count == 1000, "Reloaded config reflects modified count: 1000");

            // Test 4: GameVersionChecker
            assert(TerrariaModCore.Compatibility.GameVersionChecker.TargetTerrariaVersion == "1.4.5.8", "TargetTerrariaVersion is 1.4.5.8");
            assert(TerrariaModCore.Compatibility.GameVersionChecker.SupportedTerrariaVersions.Length >= 2, "SupportedTerrariaVersions includes multiple 1.4.5.x hotfixes");
            bool validVer = TerrariaModCore.Compatibility.GameVersionChecker.ValidateTerrariaVersion(out string detectedVer);
            assert(validVer, $"GameVersionChecker successfully validated Terraria assembly version (Detected: {detectedVer})");

            try { Directory.Delete(testDir, true); } catch { }
        }
    }
}
