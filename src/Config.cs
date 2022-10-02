using System;
using System.IO;
using Tomlyn;
using System.Runtime.Serialization;
using System.Collections.Generic;

class Config {
    [DataMember(Name = "window_transparency")]
    public Dictionary<string, byte> transparentWindows { get; set; }

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