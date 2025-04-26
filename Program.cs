using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using PornPaster.Models;
using PornPaster.Forms;
using PornPaster.Services;

namespace PornPaster
{
    static class Program
    {
        // WinAPI for global hotkey
        [DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);

        const uint MOD_NONE = 0x0000;
        const int BASE_HOTKEY_ID = 0xB000;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var trayApp = new TrayApplicationContext();
            Application.Run(trayApp);
        }

        class TrayApplicationContext : ApplicationContext
        {
            private readonly NotifyIcon trayIcon;
            private readonly Config config;
            private readonly Dictionary<int, HotkeyConfig> hotkeyMap = new Dictionary<int, HotkeyConfig>();
            private readonly HotkeyForm hiddenForm;
            private readonly Dictionary<HotkeyConfig, DateTime> lastHotkeyPress = new Dictionary<HotkeyConfig, DateTime>();
            private const int COOLDOWN_MS = 500; // 0.5 second cooldown
            private ToolStripMenuItem? pauseResumeMenuItem;

            public TrayApplicationContext()
            {
                // Load configuration
                config = Config.Load();
                
                // Initialize lastHotkeyPress for all hotkeys
                foreach (var hotkey in config.Hotkeys)
                {
                    lastHotkeyPress[hotkey] = DateTime.MinValue;
                }

                // Create a hidden form to handle hotkeys
                hiddenForm = new HotkeyForm(OnHotkey, ViewLastImage);
                hiddenForm.ShowInTaskbar = false;
                hiddenForm.WindowState = FormWindowState.Minimized;
                hiddenForm.Load += (s, e) => hiddenForm.Hide();

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Configure", null, ShowConfig);
                contextMenu.Items.Add("Exit", null, Exit);

                // Set up tray icon
                trayIcon = new NotifyIcon {
                    Icon = SystemIcons.Application,
                    Text = "Random Image Paster",
                    Visible = true,
                    ContextMenuStrip = contextMenu
                };

                // Register hotkeys
                RegisterAllHotkeys();
            }

            private void RegisterAllHotkeys()
            {
                // Unregister all existing hotkeys
                foreach (var id in hotkeyMap.Keys)
                {
                    UnregisterHotKey(hiddenForm.Handle, id);
                }
                hotkeyMap.Clear();

                // Register new hotkeys
                for (int i = 0; i < config.Hotkeys.Count; i++)
                {
                    var hotkey = config.Hotkeys[i];
                    var id = BASE_HOTKEY_ID + i;
                    RegisterHotKey(hiddenForm.Handle, id, MOD_NONE, (uint)hotkey.Key);
                    hotkeyMap[id] = hotkey;
                }
            }

            void ShowConfig(object? sender, EventArgs e)
            {
                var configWindow = new ConfigWindow(config);
                if (configWindow.ShowDialog() == true)
                {
                    RegisterAllHotkeys();
                }
            }

            bool IsOnCooldown(HotkeyConfig hotkey)
            {
                var timeSinceLastPress = DateTime.Now - lastHotkeyPress[hotkey];
                return timeSinceLastPress.TotalMilliseconds < COOLDOWN_MS;
            }

            void OnHotkey(int hotkeyId)
            {
                if (!hotkeyMap.TryGetValue(hotkeyId, out var hotkey)) return;
                if (IsOnCooldown(hotkey)) return;
                lastHotkeyPress[hotkey] = DateTime.Now;

                if (hotkey.IsPaused) return;

                var imageService = new ImageService(hotkey);
                if (imageService.TryGetRandomImage(out var imagePath) && imagePath != null)
                {
                    if (imageService.TryPasteImage(imagePath))
                    {
                        if (hotkey.AutoPaste)
                        {
                            // Get the current foreground window and send Ctrl+V
                            var foregroundWindow = GetForegroundWindow();
                            SetForegroundWindow(foregroundWindow);
                            SendKeys.SendWait("^v");

                            // If auto-send is enabled, wait a short delay and send Enter key
                            if (hotkey.AutoSend)
                            {
                                System.Threading.Thread.Sleep(hotkey.AutoSendDelay);
                                SendKeys.SendWait("{ENTER}");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to paste image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            void ViewLastImage()
            {
                // TODO: Implement view last image functionality for multiple hotkeys
                MessageBox.Show("View last image functionality not yet implemented for multiple hotkeys", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            void Exit(object? sender, EventArgs e)
            {
                foreach (var id in hotkeyMap.Keys)
                {
                    UnregisterHotKey(hiddenForm.Handle, id);
                }
                trayIcon.Visible = false;
                Application.Exit();
                Process.GetCurrentProcess().Kill();
            }
        }

        class HotkeyForm : Form
        {
            private readonly Action<int> hotkeyCallback;
            private readonly Action viewCallback;

            public HotkeyForm(Action<int> hotkeyCb, Action viewCb)
            {
                hotkeyCallback = hotkeyCb;
                viewCallback = viewCb;
            }

            protected override void WndProc(ref Message m)
            {
                const int WM_HOTKEY = 0x0312;

                if (m.Msg == WM_HOTKEY)
                {
                    var id = m.WParam.ToInt32();
                    hotkeyCallback(id);
                }

                base.WndProc(ref m);
            }
        }
    }
} 