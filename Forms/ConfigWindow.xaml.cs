using System;
using System.Windows;
using System.Windows.Forms;
using PornPaster.Models;
using Microsoft.Win32;
using System.Windows.Forms.Integration;

namespace PornPaster.Forms
{
    public partial class ConfigWindow : Window
    {
        private readonly Config config;
        private HotkeyConfig? currentHotkey;

        public ConfigWindow(Config config)
        {
            InitializeComponent();
            this.config = config;
            LoadHotkeys();
            PopulateHotkeyComboBox();
            PopulateFolderSelectionModeComboBox();
        }

        private void LoadHotkeys()
        {
            HotkeySelectorComboBox.Items.Clear();
            foreach (var hotkey in config.Hotkeys)
            {
                HotkeySelectorComboBox.Items.Add(hotkey.Name);
            }

            if (config.Hotkeys.Count > 0)
            {
                HotkeySelectorComboBox.SelectedIndex = 0;
            }
        }

        private void PopulateHotkeyComboBox()
        {
            HotkeyComboBox.Items.Clear();
            foreach (var key in Enum.GetNames(typeof(Keys)))
            {
                HotkeyComboBox.Items.Add(key);
            }
        }

        private void PopulateFolderSelectionModeComboBox()
        {
            FolderSelectionModeComboBox.Items.Clear();
            foreach (var mode in Enum.GetNames(typeof(FolderSelectionMode)))
            {
                FolderSelectionModeComboBox.Items.Add(mode);
            }
        }

        private void UpdateControls()
        {
            if (currentHotkey == null) return;

            HotkeyNameTextBox.Text = currentHotkey.Name;
            HotkeyComboBox.SelectedItem = currentHotkey.Key.ToString();
            SearchSubdirectoriesCheckBox.IsChecked = currentHotkey.SearchSubdirectories;
            AutoPasteCheckBox.IsChecked = currentHotkey.AutoPaste;
            FolderSelectionModeComboBox.SelectedItem = currentHotkey.FolderSelectionMode.ToString();

            FolderListBox.Items.Clear();
            foreach (var folder in currentHotkey.ImageFolders)
            {
                FolderListBox.Items.Add(folder);
            }
        }

        private void HotkeySelectorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (HotkeySelectorComboBox.SelectedIndex >= 0)
            {
                currentHotkey = config.Hotkeys[HotkeySelectorComboBox.SelectedIndex];
                UpdateControls();
            }
        }

        private void AddHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            var newHotkey = new HotkeyConfig
            {
                Name = $"Hotkey {config.Hotkeys.Count + 1}",
                Key = Keys.Insert
            };
            config.Hotkeys.Add(newHotkey);
            LoadHotkeys();
            HotkeySelectorComboBox.SelectedIndex = config.Hotkeys.Count - 1;
        }

        private void RemoveHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (HotkeySelectorComboBox.SelectedIndex >= 0 && config.Hotkeys.Count > 1)
            {
                config.Hotkeys.RemoveAt(HotkeySelectorComboBox.SelectedIndex);
                LoadHotkeys();
                if (config.Hotkeys.Count > 0)
                {
                    HotkeySelectorComboBox.SelectedIndex = 0;
                }
            }
        }

        private void HotkeyNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.Name = HotkeyNameTextBox.Text;
                LoadHotkeys();
                HotkeySelectorComboBox.SelectedItem = currentHotkey.Name;
            }
        }

        private void HotkeyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (currentHotkey != null && HotkeyComboBox.SelectedItem != null)
            {
                currentHotkey.Key = (Keys)Enum.Parse(typeof(Keys), HotkeyComboBox.SelectedItem.ToString()!);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentHotkey == null) return;

            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!currentHotkey.ImageFolders.Contains(dialog.SelectedPath))
                {
                    currentHotkey.ImageFolders.Add(dialog.SelectedPath);
                    FolderListBox.Items.Add(dialog.SelectedPath);
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentHotkey == null || FolderListBox.SelectedItem == null) return;

            currentHotkey.ImageFolders.Remove(FolderListBox.SelectedItem.ToString()!);
            FolderListBox.Items.Remove(FolderListBox.SelectedItem);
        }

        private void SearchSubdirectoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.SearchSubdirectories = true;
            }
        }

        private void SearchSubdirectoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.SearchSubdirectories = false;
            }
        }

        private void AutoPasteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.AutoPaste = true;
            }
        }

        private void AutoPasteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.AutoPaste = false;
            }
        }

        private void FolderSelectionModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (currentHotkey != null && FolderSelectionModeComboBox.SelectedItem != null)
            {
                currentHotkey.FolderSelectionMode = (FolderSelectionMode)Enum.Parse(
                    typeof(FolderSelectionMode),
                    FolderSelectionModeComboBox.SelectedItem.ToString()!
                );
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            config.Save();
            DialogResult = true;
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
} 