using System;
using System.IO;
using Tomlyn;
using System.Runtime.Serialization;
using System.Collections.Generic;

class Config {
    [DataMember(Name = "window_transparency")]
    public Dictionary<String, byte> transparentWindows { get; set; }

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
    public Dictionary<String, TaskbarConfig> taskbars { get; set; }

    [DataMember(Name = "monitors")]
    public Dictionary<uint, int[]> monitors { get; set; }

    [DataMember(Name = "voicemeeter_media_strip_id")]
    public uint mediaStripId { get; set; }

    static Config config;

    public static Config GetConfig() {
        return config;
    }

    static String GetConfigPath() {
        String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        String configFilePath = Path.Combine(appDataPath, "DesktopManager", "config.toml");

        return configFilePath;
    }

    static void CreateConfig(String configFilePath) {
        Directory.CreateDirectory(Directory.GetParent(configFilePath).FullName);
        File.Copy(@"resources\config.toml", configFilePath);
    }

    static String ReadConfigFile(String configFilePath) {
        String[] lines = File.ReadAllLines(configFilePath);
        String data = String.Join('\n', lines);

        return data;
    }

    static Config ParseConfigData(String data) {
        return Toml.ToModel<Config>(data);
    }

    public static void LoadConfig() {
        String configFilePath = GetConfigPath();

        if (!File.Exists(configFilePath)) CreateConfig(configFilePath);

        String configData = ReadConfigFile(configFilePath);
        Config config = ParseConfigData(configData);

        Config.config = config;
    }
}
