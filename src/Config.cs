using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Tomlyn;

class Config {
    [DataMember(Name = "window_transparency")]
    public Dictionary<string, byte> transparentWindows { get; set; }

    public class TaskbarConfig {
        [DataMember(Name = "accent_state")]
        public uint accentState { get; set; }

        [DataMember(Name = "position")]
        public int[] position { get; set; }

        [DataMember(Name = "hide_start")]
        public bool hideStart { get; set; }

        [DataMember(Name = "clock_to_start")]
        public bool clockToStart { get; set; }
    }

    [DataMember(Name = "taskbars")]
    public Dictionary<string, TaskbarConfig> taskbars { get; set; }

    [DataMember(Name = "monitors")]
    public Dictionary<uint, int[]> monitors { get; set; }

    [DataMember(Name = "voicemeeter_media_strip_id")]
    public uint mediaStripId { get; set; }

    static Config config;

    public static Config GetConfig() {
        return config;
    }

    static string GetConfigPath() {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string configFilePath = Path.Combine(appDataPath, "DesktopManager", "config.toml");

        return configFilePath;
    }

    static void CreateConfig(string configFilePath) {
        Directory.CreateDirectory(Directory.GetParent(configFilePath).FullName);
        File.Copy(@"resources\config.toml", configFilePath);
    }

    static string ReadConfigFile(string configFilePath) {
        string[] lines = File.ReadAllLines(configFilePath);
        string data = string.Join("\n", lines);

        return data;
    }

    static Config ParseConfigData(string data) {
        return Toml.ToModel<Config>(data);
    }

    public static void LoadConfig() {
        string configFilePath = GetConfigPath();

        if (!File.Exists(configFilePath)) CreateConfig(configFilePath);

        string configData = ReadConfigFile(configFilePath);
        Config config = ParseConfigData(configData);

        Config.config = config;
    }
}
