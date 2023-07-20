using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Models;
using interop;
using Microsoft.VisualBasic;
using WinUIEx;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private HotkeyManager _hotkeyManager = null;
        private ushort _hotkeyHandle;

        /// <summary>
        /// 默认快捷键 Alt+Space
        /// </summary>
        private HotkeySettings _defaultActivationShortcut => new(false, false, true, false, 0x20);

        // 快捷键变更回调
        public Action<HotkeySettings/*PreviousHotkey*/, HotkeySettings/*NewHotkey*/> ActActivationShortcutChanged { get; set; } = null;

        // 快捷键
        private HotkeySettings _activationShortcut;
        public HotkeySettings ActivationShortcut
        {
            get => _activationShortcut;
            set
            {
                HotkeySettings newValue = (value == null || !value.IsValid() || value.IsEmpty()) ? _defaultActivationShortcut : value;
                HotkeySettings previousValue = _activationShortcut?.Clone();
                SetProperty(ref _activationShortcut, newValue);
                ActActivationShortcutChanged?.Invoke(previousValue, _activationShortcut);
            }
        }

        public void RegisterShortcut()
        {
            ActActivationShortcutChanged += (preSettings, newSettings) =>
            {
                if (preSettings != null && !preSettings.IsEmpty())
                {
                    if (_hotkeyHandle != 0)
                    {
                        _hotkeyManager?.UnregisterHotkey(_hotkeyHandle);
                        _hotkeyHandle = 0;
                    }
                }

                if (newSettings?.IsValid() == true)
                {
                    SetShortcut(newSettings, OnShortcutActivated);
                }
            };

            if (ActivationShortcut?.IsValid() == true)
            {
                SetShortcut(ActivationShortcut, OnShortcutActivated);
            }
        }

        private void SetShortcut(HotkeySettings hotkeySettings, HotkeyCallback action)
        {
            try
            {
                Hotkey hotkey = new()
                {
                    Alt = hotkeySettings.Alt,
                    Shift = hotkeySettings.Shift,
                    Ctrl = hotkeySettings.Ctrl,
                    Win = hotkeySettings.Win,
                    Key = (byte)hotkeySettings.Code,
                };

                if (_hotkeyHandle != 0)
                {
                    _hotkeyManager?.UnregisterHotkey(_hotkeyHandle);
                    _hotkeyHandle = 0;
                    Debug.WriteLine("Unregistering previous low level key handler", GetType());
                }

                _hotkeyManager ??= new HotkeyManager();
                _hotkeyHandle = _hotkeyManager.RegisterHotkey(hotkey, action);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void OnShortcutActivated()
        {
            App.MainWindow.Restore();
            App.MainWindow.BringToFront();
        }

        #region 快捷键设置存取

        private async void ReadShortcutSettings()
        {
            try
            {
                string json = await StorageFilesService.ReadFileAsync("shortcutsettings");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var shortcut = JsonSerializer.Deserialize<HotkeySettings>(json);
                    ActivationShortcut = shortcut;
                    return;
                }
            }
            catch { }
            ActivationShortcut = _defaultActivationShortcut;
        }

        private async void SaveShortcutSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(ActivationShortcut);
                await StorageFilesService.WriteFileAsync("shortcutsettings", json);
            }
            catch { }
        }

        #endregion

        private static bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    SaveShortcutSettings();

                    if (_hotkeyHandle != 0)
                    {
                        _hotkeyManager?.UnregisterHotkey(_hotkeyHandle);
                    }

                    _hotkeyManager?.Dispose();
                    _disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
