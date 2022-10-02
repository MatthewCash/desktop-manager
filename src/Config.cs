using System;
using System.IO;
using Tomlyn;
using System.Runtime.Serialization;
using System.Collections.Generic;

class Config {
    [DataMember(Name = "window_transparency")]
    public Dictionary<String, byte> transparentWindows { get; set; }

    public class TaskbarConfig {
        [DataMember(Name = "primary_accentstate")]
        public uint primaryAccentState { get; set; }
        
        [DataMember(Name = "secondary_accentstate")]
        public uint secondaryAccentState { get; set; }
        
        [DataMember(Name = "secondary_position")]
        public int[] secondaryPosition { get; set; }
    }

    [DataMember(Name = "taskbars")]
    public TaskbarConfig taskbars { get; set; }

    [DataMember(Name = "monitors")]
    public Dictionary<uint, int[]> monitors { get; set; }

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