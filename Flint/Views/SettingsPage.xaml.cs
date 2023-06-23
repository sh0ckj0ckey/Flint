using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Flint.Core.Utils;
using Flint.ViewModels;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Flint.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel _viewModel = null;
        private string _appVersion = string.Empty;

        public SettingsPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = AppVersionUtil.GetAppVersion();
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
            catch { }
        }

        /// <summary>
        /// 切换黑白模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppearanceSelectiongChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MainViewModel.Instance.ActSwitchAppTheme?.Invoke();
            }
            catch { }
        }

        // 访问 GitHub
        private async void OnClickGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Flint"));
            }
            catch { }
        }

        private async void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            await Task.Delay(150);
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void OnSearchBoxStyleChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is RadioButton rb)
                {
                    string tag = rb?.Tag?.ToString();
                    switch (tag)
                    {
                        case "0":
                            _viewModel.AppSettings.SearchBoxStyle = 0;
                            break;
                        case "1":
                            _viewModel.AppSettings.SearchBoxStyle = 1;
                            break;
                        case "2":
                            _viewModel.AppSettings.SearchBoxStyle = 2;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch { }
        }
    }
}
