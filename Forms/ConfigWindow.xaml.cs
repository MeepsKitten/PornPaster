using System;
using System.Windows;
using PornPaster.Models;
using Microsoft.Win32;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.Diagnostics;

namespace PornPaster.Forms
{
    public partial class ConfigWindow : Window
    {
        private readonly Config config;
        private HotkeyConfig? currentHotkey;
        private bool isUpdatingControls = false;

        public ConfigWindow(Config config)
        {
            try
            {
                this.config = config;
                InitializeComponent();
                LoadHotkeys();
                PopulateHotkeyComboBox();
                PopulateFolderSelectionModeComboBox();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error initializing config window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadHotkeys()
        {
            try
            {
                Debug.WriteLine("Loading hotkeys...");
                HotkeySelectorComboBox.Items.Clear();
                foreach (var hotkey in config.Hotkeys)
                {
                    Debug.WriteLine($"Adding hotkey: {hotkey.Name}");
                    HotkeySelectorComboBox.Items.Add(hotkey.Name);
                }

                if (config.Hotkeys.Count > 0)
                {
                    Debug.WriteLine("Setting initial hotkey selection");
                    HotkeySelectorComboBox.SelectedIndex = 0;
                    currentHotkey = config.Hotkeys[0];
                    UpdateControls();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading hotkeys: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateHotkeyComboBox()
        {
            try
            {
                HotkeyComboBox.Items.Clear();
                foreach (var key in Enum.GetNames(typeof(Keys)))
                {
                    HotkeyComboBox.Items.Add(key);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error populating hotkey combo box: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateFolderSelectionModeComboBox()
        {
            try
            {
                FolderSelectionModeComboBox.Items.Clear();
                foreach (var mode in Enum.GetNames(typeof(FolderSelectionMode)))
                {
                    FolderSelectionModeComboBox.Items.Add(mode);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error populating folder selection mode combo box: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateControls()
        {
            try
            {
                if (currentHotkey == null) return;
                if (isUpdatingControls) return;

                isUpdatingControls = true;
                Debug.WriteLine($"Updating controls for hotkey: {currentHotkey.Name}");

                HotkeyNameTextBox.Text = currentHotkey.Name;
                HotkeyComboBox.SelectedItem = currentHotkey.Key.ToString();
                SearchSubdirectoriesCheckBox.IsChecked = currentHotkey.SearchSubdirectories;
                AutoPasteCheckBox.IsChecked = currentHotkey.AutoPaste;
                AutoSendCheckBox.IsChecked = currentHotkey.AutoSend;
                FolderSelectionModeComboBox.SelectedItem = currentHotkey.FolderSelectionMode.ToString();

                FolderListBox.Items.Clear();
                foreach (var folder in currentHotkey.ImageFolders)
                {
                    FolderListBox.Items.Add(folder);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating controls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isUpdatingControls = false;
            }
        }

        private void HotkeySelectorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedIndex = HotkeySelectorComboBox.SelectedIndex;
                Debug.WriteLine($"Hotkey selection changed to index: {selectedIndex}");

                if (selectedIndex >= 0 && selectedIndex < config.Hotkeys.Count)
                {
                    currentHotkey = config.Hotkeys[selectedIndex];
                    Debug.WriteLine($"Selected hotkey: {currentHotkey.Name}");
                    UpdateControls();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error changing hotkey selection: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding hotkey: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HotkeySelectorComboBox.SelectedIndex >= 0 && config.Hotkeys.Count > 1)
                {
                    var index = HotkeySelectorComboBox.SelectedIndex;
                    config.Hotkeys.RemoveAt(index);
                    LoadHotkeys();
                    HotkeySelectorComboBox.SelectedIndex = Math.Min(index, config.Hotkeys.Count - 1);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error removing hotkey: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HotkeyNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.Name = HotkeyNameTextBox.Text;
                    var index = HotkeySelectorComboBox.SelectedIndex;
                    LoadHotkeys();
                    HotkeySelectorComboBox.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating hotkey name: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HotkeyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && HotkeyComboBox.SelectedItem != null && !isUpdatingControls)
                {
                    currentHotkey.Key = (Keys)Enum.Parse(typeof(Keys), HotkeyComboBox.SelectedItem.ToString()!);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error changing hotkey key: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey == null) return;

                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!currentHotkey.ImageFolders.Contains(dialog.SelectedPath))
                    {
                        currentHotkey.ImageFolders.Add(dialog.SelectedPath);
                        FolderListBox.Items.Add(dialog.SelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey == null || FolderListBox.SelectedItem == null) return;

                currentHotkey.ImageFolders.Remove(FolderListBox.SelectedItem.ToString()!);
                FolderListBox.Items.Remove(FolderListBox.SelectedItem);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error removing folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchSubdirectoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.SearchSubdirectories = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating search subdirectories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchSubdirectoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.SearchSubdirectories = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating search subdirectories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoPasteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.AutoPaste = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating auto paste: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoPasteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.AutoPaste = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating auto paste: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoSendCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.AutoSend = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating auto send: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoSendCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && !isUpdatingControls)
                {
                    currentHotkey.AutoSend = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating auto send: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FolderSelectionModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (currentHotkey != null && FolderSelectionModeComboBox.SelectedItem != null && !isUpdatingControls)
                {
                    currentHotkey.FolderSelectionMode = (FolderSelectionMode)Enum.Parse(
                        typeof(FolderSelectionMode),
                        FolderSelectionModeComboBox.SelectedItem.ToString()!
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating folder selection mode: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                config.Save();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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