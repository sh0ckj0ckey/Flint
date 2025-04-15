using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Helpers;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static readonly Lazy<MainViewModel> _instance = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _instance.Value;

        public readonly Microsoft.UI.Dispatching.DispatcherQueue Dispatcher = null;

        /// <summary>
        /// 应用程序设置
        /// </summary>
        public SettingsService AppSettings { get; set; } = new SettingsService();

        private string _appVersion = string.Empty;

        /// <summary>
        /// 应用程序版本号
        /// </summary>
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appVersion))
                {
                    _appVersion = $"Flint {AppVersionUtil.GetAppVersion()}";
                }

                return _appVersion;
            }
        }

        private MainViewModel()
        {
            this.Dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            InitViewModel4Flint();
            InitViewModel4Glossary();
            InitViewModel4ShortcutKeys();
        }
    }
}
