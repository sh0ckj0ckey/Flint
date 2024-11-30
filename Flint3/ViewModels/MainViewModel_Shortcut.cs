using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Models;
using interop;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private HotkeyManager _hotkeyManager = null;

        private ushort _hotkeyHandle;

        private HotkeySettings _activationShortcut;

        /// <summary>
        /// 默认快捷键 Alt+Space
        /// </summary>
        private HotkeySettings _defaultActivationShortcut => new(false, false, true, false, 0x20);

        /// <summary>
        /// 快捷键变更回调
        /// </summary>
        public event Action<HotkeySettings/*PreviousHotkey*/, HotkeySettings/*NewHotkey*/> OnActivationShortcutChanged  = null;

        /// <summary>
        /// 快捷键
        /// </summary>
        public HotkeySettings ActivationShortcut
        {
            get => _activationShortcut;
            set
            {
                if (_activationShortcut?.ToString() != value?.ToString())
                {
                    HotkeySettings newValue = (value == null || !value.IsValid() || value.IsEmpty()) ? _defaultActivationShortcut : value;
                    HotkeySettings previousValue = _activationShortcut?.Clone();
                    SetProperty(ref _activationShortcut, newValue);
                    this.OnActivationShortcutChanged?.Invoke(previousValue, _activationShortcut);
                }
            }
        }

        /// <summary>
        /// 进行对快捷键相关的初始化
        /// </summary>
        private async void InitViewModel4ShortcutKeys()
        {
            await ReadShortcutSettings();

            if (this.ActivationShortcut?.IsValid() == true)
            {
                RegisterShortcut(this.ActivationShortcut, OnShortcutActivated);
            }

            this.OnActivationShortcutChanged += async (preSettings, newSettings) =>
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
                    RegisterShortcut(newSettings, OnShortcutActivated);
                }

                await SaveShortcutSettings();
            };
        }

        private void RegisterShortcut(HotkeySettings hotkeySettings, HotkeyCallback action)
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
                System.Diagnostics.Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// 按下快捷键，唤出主窗口或精简窗口
        /// </summary>
        private void OnShortcutActivated()
        {
            if (this.AppSettings.UseLiteWindow)
            {
                ShowLiteWindow();
            }
            else
            {
                ShowMainWindow();
            }
        }

        #region 快捷键设置存取

        private async Task ReadShortcutSettings()
        {
            try
            {
                string json = await StorageFilesService.ReadFileAsync("shortcutsettings");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var shortcut = JsonSerializer.Deserialize<HotkeySettings>(json);
                    this.ActivationShortcut = shortcut;
                    return;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            this.ActivationShortcut = _defaultActivationShortcut;
        }

        private async Task SaveShortcutSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(this.ActivationShortcut);
                bool ret = await StorageFilesService.WriteFileAsync("shortcutsettings", json);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        #endregion

        #region Dispose

        private static bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
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

        #endregion

    }
}
