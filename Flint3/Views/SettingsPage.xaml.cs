using System;
using Flint3.Core.Utils;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainViewModel _viewModel = null;
        private string _appVersion = string.Empty;

        public SettingsPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = $"Flint {AppVersionUtil.GetAppVersion()}";
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
        /// 访问 ECDICT on GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/skywind3000/ECDICT"));
            }
            catch { }
        }

        /// <summary>
        /// 访问 燧石 on GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickFlintGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Flint"));
            }
            catch { }
        }

        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(150);
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
