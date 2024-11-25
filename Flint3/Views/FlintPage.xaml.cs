using System;
using System.Threading.Tasks;
using Flint3.Controls.KeyVisual;
using Flint3.Data;
using Flint3.Data.Models;
using Flint3.Helpers;
using Flint3.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using WinUIEx;

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

        public FlintPage()
        {
            this.InitializeComponent();

            ViewModel = MainViewModel.Instance;

            MainViewModel.Instance.Dispatcher = this.DispatcherQueue;

            MainViewModel.Instance.ActSwitchAppTheme?.Invoke();

            MainViewModel.Instance.ActFocusOnTextBox = () => { SearchTextBox?.Focus(FocusState.Keyboard); };

            MainViewModel.Instance.ActClearTextBox = () => { SearchTextBox.Text = ""; };

            MainViewModel.Instance.ActHideAddingPopup = () => { if (AddWordToGlossaryPopup != null) { AddWordToGlossaryPopup.IsOpen = false; } };

            // 加载单词数据库
            MainViewModel.Instance.LoadStarDict();

            // 加载生词本数据库
            MainViewModel.Instance.LoadMyGlossaries();
            MainViewModel.Instance.LoadExGlossaries();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //TitleBarHelper.UpdateTitleBar(App.MainWindow, ActualTheme);

            SearchTextBox.Style = GetSearchTextBoxStyle(MainViewModel.Instance.AppSettings.SearchBoxStyle);

            // 首次启动显示 TeachingTips
            TryShowNewFeatureButton();
        }

        /// <summary>
        /// 获取搜索输入框样式
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private Style GetSearchTextBoxStyle(int searchBoxStyle)
        {
            if (searchBoxStyle == 0)
            {
                return (Style)App.Current.Resources["ModernTextBoxStyle"];
            }
            else if (searchBoxStyle == 1)
            {
                return (Style)App.Current.Resources["RoundTextBoxStyle"];
            }
            else
            {
                return (Style)App.Current.Resources["ClassicTextBoxStyle"];
            }
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
        /// 即时搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is TextBox tb && !string.IsNullOrWhiteSpace(tb?.Text))
                {
                    ViewModel.MatchWord(tb.Text);
                }
                else
                {
                    ViewModel.MatchWord(string.Empty);
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 取得焦点后，选中所有文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchBoxGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is TextBox tb && !string.IsNullOrEmpty(tb?.Text))
                {
                    tb.SelectAll();
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
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
                    MainViewModel.Instance.ActPinWindow?.Invoke(true);
                }
                else
                {
                    MainViewModel.Instance.ActPinWindow?.Invoke(false);
                }
            }
            catch
            {
                PinToggleButton.IsChecked = false;
                MainViewModel.Instance.ActPinWindow?.Invoke(false);
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
                MainViewModel.Instance.AddingWordItem = item;
                AddWordControl.UpdateControl();
                AddWordToGlossaryPopup.IsOpen = true;
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
