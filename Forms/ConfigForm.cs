using System;
using System.Windows.Forms;
using System.Collections.Generic;
using PornPaster.Models;

namespace PornPaster.Forms
{
    public class ConfigForm : Form
    {
        private readonly ListBox folderListBox = new ListBox();
        private readonly Button addButton = new Button();
        private readonly Button removeButton = new Button();
        private readonly Button saveButton = new Button();
        private readonly Button addHotkeyButton = new Button();
        private readonly Button removeHotkeyButton = new Button();
        private readonly CheckBox searchSubdirectoriesCheckBox = new CheckBox();
        private readonly CheckBox autoPasteCheckBox = new CheckBox();
        private readonly ComboBox folderSelectionModeComboBox = new ComboBox();
        private readonly ComboBox hotkeyComboBox = new ComboBox();
        private readonly TextBox hotkeyNameTextBox = new TextBox();
        private readonly ComboBox hotkeySelectorComboBox = new ComboBox();
        private readonly Config config;
        private HotkeyConfig? currentHotkey;

        public ConfigForm(Config config)
        {
            this.config = config;
            InitializeComponent();
            LoadHotkeys();
            if (config.Hotkeys.Count > 0)
            {
                hotkeySelectorComboBox.SelectedIndex = 0;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "PornPaster Configuration";
            this.Size = new System.Drawing.Size(500, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Hotkey selector label
            var hotkeySelectorLabel = new Label
            {
                Text = "Select Hotkey:",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(100, 30)
            };

            // Hotkey selector combo box
            hotkeySelectorComboBox.Location = new System.Drawing.Point(118, 12);
            hotkeySelectorComboBox.Size = new System.Drawing.Size(200, 30);
            hotkeySelectorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hotkeySelectorComboBox.SelectedIndexChanged += HotkeySelectorComboBox_SelectedIndexChanged;

            // Add hotkey button
            addHotkeyButton.Text = "Add Hotkey";
            addHotkeyButton.Location = new System.Drawing.Point(324, 12);
            addHotkeyButton.Size = new System.Drawing.Size(75, 30);
            addHotkeyButton.Click += AddHotkeyButton_Click;

            // Remove hotkey button
            removeHotkeyButton.Text = "Remove";
            removeHotkeyButton.Location = new System.Drawing.Point(405, 12);
            removeHotkeyButton.Size = new System.Drawing.Size(75, 30);
            removeHotkeyButton.Click += RemoveHotkeyButton_Click;

            // Hotkey name label
            var hotkeyNameLabel = new Label
            {
                Text = "Hotkey Name:",
                Location = new System.Drawing.Point(12, 50),
                Size = new System.Drawing.Size(100, 30)
            };

            // Hotkey name text box
            hotkeyNameTextBox.Location = new System.Drawing.Point(118, 50);
            hotkeyNameTextBox.Size = new System.Drawing.Size(200, 30);
            hotkeyNameTextBox.TextChanged += HotkeyNameTextBox_TextChanged;

            // Hotkey label
            var hotkeyLabel = new Label
            {
                Text = "Key:",
                Location = new System.Drawing.Point(12, 88),
                Size = new System.Drawing.Size(100, 30)
            };

            // Hotkey combo box
            hotkeyComboBox.Location = new System.Drawing.Point(118, 88);
            hotkeyComboBox.Size = new System.Drawing.Size(200, 30);
            hotkeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hotkeyComboBox.Items.AddRange(Enum.GetNames(typeof(Keys)));
            hotkeyComboBox.SelectedIndexChanged += HotkeyComboBox_SelectedIndexChanged;

            // Folder list box
            folderListBox.Location = new System.Drawing.Point(12, 126);
            folderListBox.Size = new System.Drawing.Size(460, 150);
            folderListBox.SelectionMode = SelectionMode.One;

            // Add button
            addButton.Text = "Add Folder";
            addButton.Location = new System.Drawing.Point(12, 284);
            addButton.Size = new System.Drawing.Size(100, 30);
            addButton.Click += AddButton_Click;

            // Remove button
            removeButton.Text = "Remove Selected";
            removeButton.Location = new System.Drawing.Point(118, 284);
            removeButton.Size = new System.Drawing.Size(100, 30);
            removeButton.Click += RemoveButton_Click;

            // Search subdirectories checkbox
            searchSubdirectoriesCheckBox.Text = "Search Subdirectories";
            searchSubdirectoriesCheckBox.Location = new System.Drawing.Point(12, 322);
            searchSubdirectoriesCheckBox.Size = new System.Drawing.Size(200, 30);
            searchSubdirectoriesCheckBox.CheckedChanged += SearchSubdirectoriesCheckBox_CheckedChanged;

            // Auto paste checkbox
            autoPasteCheckBox.Text = "Auto Paste";
            autoPasteCheckBox.Location = new System.Drawing.Point(220, 322);
            autoPasteCheckBox.Size = new System.Drawing.Size(200, 30);
            autoPasteCheckBox.CheckedChanged += AutoPasteCheckBox_CheckedChanged;

            // Folder selection mode label
            var folderSelectionLabel = new Label
            {
                Text = "Folder Selection Mode:",
                Location = new System.Drawing.Point(12, 362),
                Size = new System.Drawing.Size(150, 30)
            };

            // Folder selection mode combo box
            folderSelectionModeComboBox.Location = new System.Drawing.Point(170, 362);
            folderSelectionModeComboBox.Size = new System.Drawing.Size(150, 30);
            folderSelectionModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            folderSelectionModeComboBox.Items.AddRange(Enum.GetNames(typeof(FolderSelectionMode)));
            folderSelectionModeComboBox.SelectedIndexChanged += FolderSelectionModeComboBox_SelectedIndexChanged;

            // Save button
            saveButton.Text = "Save";
            saveButton.Location = new System.Drawing.Point(372, 430);
            saveButton.Size = new System.Drawing.Size(100, 30);
            saveButton.Click += SaveButton_Click;

            this.Controls.AddRange(new Control[] { 
                hotkeySelectorLabel,
                hotkeySelectorComboBox,
                addHotkeyButton,
                removeHotkeyButton,
                hotkeyNameLabel,
                hotkeyNameTextBox,
                hotkeyLabel,
                hotkeyComboBox,
                folderListBox, 
                addButton,
                removeButton,
                searchSubdirectoriesCheckBox,
                autoPasteCheckBox,
                folderSelectionLabel,
                folderSelectionModeComboBox,
                saveButton 
            });
        }

        private void LoadHotkeys()
        {
            hotkeySelectorComboBox.Items.Clear();
            foreach (var hotkey in config.Hotkeys)
            {
                hotkeySelectorComboBox.Items.Add(hotkey.Name);
            }
        }

        private void HotkeySelectorComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (hotkeySelectorComboBox.SelectedIndex >= 0)
            {
                currentHotkey = config.Hotkeys[hotkeySelectorComboBox.SelectedIndex];
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            if (currentHotkey == null) return;

            hotkeyNameTextBox.Text = currentHotkey.Name;
            hotkeyComboBox.SelectedItem = currentHotkey.Key.ToString();
            searchSubdirectoriesCheckBox.Checked = currentHotkey.SearchSubdirectories;
            autoPasteCheckBox.Checked = currentHotkey.AutoPaste;
            folderSelectionModeComboBox.SelectedItem = currentHotkey.FolderSelectionMode.ToString();

            folderListBox.Items.Clear();
            foreach (var folder in currentHotkey.ImageFolders)
            {
                folderListBox.Items.Add(folder);
            }
        }

        private void AddHotkeyButton_Click(object? sender, EventArgs e)
        {
            var newHotkey = new HotkeyConfig
            {
                Name = $"Hotkey {config.Hotkeys.Count + 1}",
                Key = Keys.Insert
            };
            config.Hotkeys.Add(newHotkey);
            LoadHotkeys();
            hotkeySelectorComboBox.SelectedIndex = config.Hotkeys.Count - 1;
        }

        private void RemoveHotkeyButton_Click(object? sender, EventArgs e)
        {
            if (hotkeySelectorComboBox.SelectedIndex >= 0 && config.Hotkeys.Count > 1)
            {
                config.Hotkeys.RemoveAt(hotkeySelectorComboBox.SelectedIndex);
                LoadHotkeys();
                if (config.Hotkeys.Count > 0)
                {
                    hotkeySelectorComboBox.SelectedIndex = 0;
                }
            }
        }

        private void HotkeyNameTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.Name = hotkeyNameTextBox.Text;
                LoadHotkeys();
                hotkeySelectorComboBox.SelectedItem = currentHotkey.Name;
            }
        }

        private void HotkeyComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (currentHotkey != null && hotkeyComboBox.SelectedItem != null)
            {
                currentHotkey.Key = (Keys)Enum.Parse(typeof(Keys), hotkeyComboBox.SelectedItem.ToString()!);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            if (currentHotkey == null) return;

            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!currentHotkey.ImageFolders.Contains(folderDialog.SelectedPath))
                    {
                        currentHotkey.ImageFolders.Add(folderDialog.SelectedPath);
                        folderListBox.Items.Add(folderDialog.SelectedPath);
                    }
                }
            }
        }

        private void RemoveButton_Click(object? sender, EventArgs e)
        {
            if (currentHotkey == null || folderListBox.SelectedItem == null) return;

            currentHotkey.ImageFolders.Remove(folderListBox.SelectedItem.ToString()!);
            folderListBox.Items.Remove(folderListBox.SelectedItem);
        }

        private void SearchSubdirectoriesCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.SearchSubdirectories = searchSubdirectoriesCheckBox.Checked;
            }
        }

        private void AutoPasteCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (currentHotkey != null)
            {
                currentHotkey.AutoPaste = autoPasteCheckBox.Checked;
            }
        }

        private void FolderSelectionModeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (currentHotkey != null && folderSelectionModeComboBox.SelectedItem != null)
            {
                currentHotkey.FolderSelectionMode = (FolderSelectionMode)Enum.Parse(
                    typeof(FolderSelectionMode),
                    folderSelectionModeComboBox.SelectedItem.ToString()!
                );
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            config.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 