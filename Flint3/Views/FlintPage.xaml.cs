using System;
using Flint3.Controls;
using Flint3.Data.Models;
using Flint3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlintPage : Page
    {
        public MainViewModel ViewModel { get; set; } = null;

        private Flyout _addToGlossaryFlyout = null;

        private AddToGlossaryControl _addToGlossaryControl = null;

        public FlintPage()
        {
            ViewModel = MainViewModel.Instance;
            this.InitializeComponent();
        }

        /// <summary>
        /// 更新搜索框样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int searchBoxStyle = MainViewModel.Instance.AppSettings.SearchBoxStyle;
            if (searchBoxStyle == 0)
            {
                SearchTextBox.Style = (Style)App.Current.Resources["ModernTextBoxStyle"];
            }
            else if (searchBoxStyle == 1)
            {
                SearchTextBox.Style = (Style)App.Current.Resources["RoundTextBoxStyle"];
            }
            else
            {
                SearchTextBox.Style = (Style)App.Current.Resources["ClassicTextBoxStyle"];
            }

            // 首次启动显示 TeachingTips
            TryShowNewFeatureButton();
        }

        /// <summary>
        /// 前往设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSettingsButton(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        /// <summary>
        /// 前往生词本页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGlossary(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GlossaryPage));
        }

        /// <summary>
        /// 点击置顶窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickPin(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PinToggleButton?.IsChecked == true)
                {
                    MainViewModel.Instance.FlintMainWindow.IsAlwaysOnTop = true;
                }
                else
                {
                    MainViewModel.Instance.FlintMainWindow.IsAlwaysOnTop = false;
                }
            }
            catch
            {
                PinToggleButton.IsChecked = false;
                MainViewModel.Instance.FlintMainWindow.IsAlwaysOnTop = false;
            }
        }

        /// <summary>
        /// 添加单词到生词本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickAddWordToGlossary(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StarDictWordItem item)
            {
                _addToGlossaryControl ??= new AddToGlossaryControl(() => { _addToGlossaryFlyout?.Hide(); });
                _addToGlossaryFlyout ??= new Flyout() { Content = _addToGlossaryControl };
                _addToGlossaryControl.PrepareToAddWord(item);

                FlyoutBase.SetAttachedFlyout(btn, _addToGlossaryFlyout);
                FlyoutBase.ShowAttachedFlyout(btn);
            }
        }

        #region 功能介绍

        private void TryShowNewFeatureButton()
        {
            try
            {
                NewFeatureButton.Visibility = Visibility.Collapsed;

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values["firstRunV300Teaching"] == null)
                {
                    NewFeatureButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickShowMeNewFeature(object sender, RoutedEventArgs e)
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                localSettings.Values["firstRunV300Teaching"] = true;
                NewFeatureButton.Visibility = Visibility.Collapsed;
                FirstTeachingTip.IsOpen = false;
                SecondTeachingTip.IsOpen = false;
                FirstTeachingTip.IsOpen = true;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private void OnClickCloseFirstTeachingTip(TeachingTip sender, object args)
        {
            FirstTeachingTip.IsOpen = false;
            SecondTeachingTip.IsOpen = true;
        }

        private void OnClickCloseSecondTeachingTip(TeachingTip sender, object args)
        {
            FirstTeachingTip.IsOpen = false;
            SecondTeachingTip.IsOpen = false;
        }

        #endregion
    }
}
