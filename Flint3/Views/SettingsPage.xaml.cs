using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Flint3.Core.Utils;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Storage;
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

        private bool _startupEnabled = false;

        public SettingsPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = $"Flint {AppVersionUtil.GetAppVersion()}";
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateStartupState();
        }

        /// <summary>
        /// 获取当前开机自启动状态，更新按钮
        /// </summary>
        private async void UpdateStartupState()
        {
            try
            {
                var task = await StartupTask.GetAsync("FlintByNoMewing");
                switch (task.State)
                {
                    case StartupTaskState.Disabled:
                        _startupEnabled = false;
                        StartupSettingToggleSwitch.IsEnabled = true;
                        StartupSettingToggleSwitch.IsOn = false;
                        StartupDescTextBlock.Text = "登录到 Windows 时自动启动燧石";
                        break;
                    case StartupTaskState.Enabled:
                        _startupEnabled = true;
                        StartupSettingToggleSwitch.IsEnabled = true;
                        StartupSettingToggleSwitch.IsOn = true;
                        StartupDescTextBlock.Text = "登录到 Windows 时自动启动燧石";
                        break;
                    case StartupTaskState.DisabledByUser:
                        _startupEnabled = false;
                        StartupSettingToggleSwitch.IsEnabled = false;
                        StartupSettingToggleSwitch.IsOn = false;
                        StartupDescTextBlock.Text = "无法修改，已被系统设置禁用";
                        break;
                    case StartupTaskState.DisabledByPolicy:
                        _startupEnabled = false;
                        StartupSettingToggleSwitch.IsEnabled = false;
                        StartupSettingToggleSwitch.IsOn = false;
                        StartupDescTextBlock.Text = "无法修改，已被策略禁用";
                        break;
                    case StartupTaskState.EnabledByPolicy:
                        _startupEnabled = true;
                        StartupSettingToggleSwitch.IsEnabled = false;
                        StartupSettingToggleSwitch.IsOn = true;
                        StartupDescTextBlock.Text = "无法修改，已被策略启用";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickBackButton(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 打开生词本文件目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickDbPath(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder documentsFolder = await StorageFolder.GetFolderFromPathAsync(UserDataPaths.GetDefault().Documents);
                var noMewingFolder = await documentsFolder.CreateFolderAsync("NoMewing", CreationCollisionOption.OpenIfExists);
                var flintFolder = await noMewingFolder.CreateFolderAsync("Flint", CreationCollisionOption.OpenIfExists);
                await Launcher.LaunchFolderAsync(flintFolder);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 设置开机自启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStartupToggled(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = await StartupTask.GetAsync("FlintByNoMewing");
                switch (task.State)
                {
                    case StartupTaskState.Disabled:
                        {
                            _startupEnabled = false;
                            if (StartupSettingToggleSwitch.IsOn != false)
                            {
                                await task.RequestEnableAsync();
                            }
                        }
                        break;
                    case StartupTaskState.Enabled:
                        {
                            _startupEnabled = true;
                            if (StartupSettingToggleSwitch.IsOn != true)
                            {
                                task.Disable();
                            }
                        }
                        break;
                }

                UpdateStartupState();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 选择搜索框样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 滚动页面时，显露顶部分割线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var verticalOffset = scrollViewer.VerticalOffset;
                var maxOffset = Math.Min(160, scrollViewer.ScrollableHeight);

                if (maxOffset <= 0) return;

                // 透明度按滚动比例变化，从全透明到不透明
                double newOpacity = verticalOffset / maxOffset;
                if (newOpacity > 1)
                {
                    newOpacity = 1;
                }
                if (newOpacity < 0)
                {
                    newOpacity = 0;
                }

                SettingsPageHeaderSeperatorLineBorder.Opacity = newOpacity;
                // LogoGrid.Opacity = 1 - newOpacity;
            }
        }
    }
}
