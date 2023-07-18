using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Helpers;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// 默认快捷键 Alt+Space
        /// </summary>
        public HotkeySettings DefaultActivationShortcut => new HotkeySettings(false, false, true, false, 0x20);

        private HotkeySettings _activationShortcut;
        public HotkeySettings ActivationShortcut
        {
            get => _activationShortcut;
            set
            {
                SetProperty(ref _activationShortcut, value ?? DefaultActivationShortcut);
                SaveActivationSettings();
            }
        }

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
    }
}
