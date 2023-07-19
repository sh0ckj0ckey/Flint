using System;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Models;
using interop;
using Microsoft.VisualBasic;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        internal HotkeyManager HotkeyManager { get; private set; }

        /// <summary>
        /// 默认快捷键 Alt+Space
        /// </summary>
        public HotkeySettings DefaultActivationShortcut => new HotkeySettings(false, false, true, false, 0x20);

        public Action<HotkeySettings> ActActivationShortcutChanged { get; set; } = null;

        private HotkeySettings _activationShortcut;
        public HotkeySettings ActivationShortcut
        {
            get => _activationShortcut;
            set
            {
                SetProperty(ref _activationShortcut, (value == null || !value.IsValid() || value.IsEmpty()) ? DefaultActivationShortcut : value);
                ActActivationShortcutChanged?.Invoke(_activationShortcut);
            }
        }

        private void InitializeViewModelForShortcut()
        {
            ReadActivationSettings();
            ActActivationShortcutChanged += (setting) => { SaveActivationSettings(); };
        }

        #region 快捷键设置存取

        private async void ReadActivationSettings()
        {
            try
            {
                string json = await StorageFilesService.ReadFileAsync("shortcutsettings.json");
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var shortcut = JsonSerializer.Deserialize<HotkeySettings>(json);
                    ActivationShortcut = shortcut;
                    return;
                }
            }
            catch { }
            ActivationShortcut = DefaultActivationShortcut;
        }

        private async void SaveActivationSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(ActivationShortcut);
                await StorageFilesService.WriteFileAsync("shortcutsettings.json", json);
            }
            catch { }
        }

        #endregion
    }
}
