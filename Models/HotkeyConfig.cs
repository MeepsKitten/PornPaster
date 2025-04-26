using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PornPaster.Models
{
    public class HotkeyConfig
    {
        public string Name { get; set; } = "New Hotkey";
        public Keys Key { get; set; } = Keys.Insert;
        public List<string> ImageFolders { get; set; } = new List<string>();
        public bool SearchSubdirectories { get; set; } = true;
        public FolderSelectionMode FolderSelectionMode { get; set; } = FolderSelectionMode.Randomly;
        public bool IsPaused { get; set; } = false;
        public bool AutoPaste { get; set; } = true;
        public bool AutoSend { get; set; } = false;
        public int AutoSendDelay { get; set; } = 500;
        private int lastFolderIndex = -1;

        public string GetNextFolder()
        {
            if (ImageFolders.Count == 0) return string.Empty;

            if (FolderSelectionMode == FolderSelectionMode.Randomly)
            {
                var random = new Random();
                return ImageFolders[random.Next(ImageFolders.Count)];
            }
            else // RoundRobin
            {
                lastFolderIndex = (lastFolderIndex + 1) % ImageFolders.Count;
                return ImageFolders[lastFolderIndex];
            }
        }
    }
} 