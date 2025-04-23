using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace PornPaster.Models
{
    public enum FolderSelectionMode
    {
        Randomly,
        RoundRobin
    }

    public class Config
    {
        public List<HotkeyConfig> Hotkeys { get; set; } = new List<HotkeyConfig>();
        public Keys ViewKey { get; set; } = Keys.F12;

        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PornPaster",
            "config.json"
        );

        public static Config Load()
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<Config>(json) ?? new Config();
                
                // Ensure at least one hotkey exists
                if (config.Hotkeys.Count == 0)
                {
                    config.Hotkeys.Add(new HotkeyConfig
                    {
                        Name = "Default",
                        Key = Keys.Insert,
                        SearchSubdirectories = true,
                        FolderSelectionMode = FolderSelectionMode.Randomly
                    });
                }
                
                return config;
            }
            else
            {
                var config = new Config();
                // Add default hotkey
                config.Hotkeys.Add(new HotkeyConfig
                {
                    Name = "Default",
                    Key = Keys.Insert,
                    SearchSubdirectories = true,
                    FolderSelectionMode = FolderSelectionMode.Randomly
                });
                return config;
            }
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
} 