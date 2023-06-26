using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Flint.Views;
using System.Runtime;
using Flint.Core;
using Flint.ViewModels;
using System.Reflection;
using System.Threading.Tasks;
using Flint.Data;
using Windows.UI.Core;
using Windows.System;
using System.Diagnostics;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Flint
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel _viewModel = null;
        private UISettings _uiSettings;
        public MainPage()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;

            // 设置自定义标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // 监听系统颜色设置变更，处理主题为"跟随系统"时主题切换
            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += OnUISettingsColorValuesChanged;

            MainViewModel.Instance.ActSwitchAppTheme = () => SwitchAppTheme();
            MainViewModel.Instance.ActClearTextBoxes = () =>
            {
                SearchTextBox1.Text = string.Empty;
                SearchTextBox2.Text = string.Empty;
                SearchTextBox3.Text = string.Empty;
            };
            SwitchAppTheme();
        }

        private void OnUISettingsColorValuesChanged(UISettings sender, object args)
        {
            if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0)
            {
                _ = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SwitchAppTheme();
                });
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 处理系统的返回键
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        }

        /// <summary>
        /// 切换应用程序的主题
        /// </summary>
        private void SwitchAppTheme()
        {
            try
            {
                // 设置标题栏颜色
                bool isLight = true;
                if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0) // 主题 0-System 1-Dark 2-Light
                {
                    var color = _uiSettings?.GetColorValue(UIColorType.Foreground) ?? Colors.Black;
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLight = g < 100; // g越小，颜色越深
                }
                else
                {
                    isLight = MainViewModel.Instance.AppSettings.AppearanceIndex == 2;
                }

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                if (isLight)
                {
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.ButtonHoverForegroundColor = Colors.Black;
                    titleBar.ButtonPressedForegroundColor = Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = new Color() { A = 8, R = 0, G = 0, B = 0 };
                    titleBar.ButtonPressedBackgroundColor = new Color() { A = 16, R = 0, G = 0, B = 0 };
                }
                else
                {
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonHoverForegroundColor = Colors.White;
                    titleBar.ButtonPressedForegroundColor = Colors.White;
                    titleBar.ButtonHoverBackgroundColor = new Color() { A = 16, R = 255, G = 255, B = 255 };
                    titleBar.ButtonPressedBackgroundColor = new Color() { A = 24, R = 255, G = 255, B = 255 };
                }

                // 设置应用程序颜色
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    if (MainViewModel.Instance.AppSettings.AppearanceIndex == 1)
                    {
                        rootElement.RequestedTheme = ElementTheme.Dark;
                    }
                    else if (MainViewModel.Instance.AppSettings.AppearanceIndex == 2)
                    {
                        rootElement.RequestedTheme = ElementTheme.Light;
                    }
                    else
                    {
                        rootElement.RequestedTheme = ElementTheme.Default;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 点击空白区域，自动聚焦
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMainGridPointerAct(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                await Task.Delay(100);
                switch (MainViewModel.Instance.AppSettings.SearchBoxStyle)
                {
                    case 0:
                        SearchTextBox1.Focus(FocusState.Keyboard);
                        break;
                    case 1:
                        SearchTextBox2.Focus(FocusState.Keyboard);
                        break;
                    case 2:
                        SearchTextBox3.Focus(FocusState.Keyboard);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// 前往设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSettingsButton(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(100);
            this.Frame.Navigate(typeof(SettingsPage));
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
                    //_viewModel.QueryWord(tb.Text);
                    _viewModel.MatchWord(tb.Text);
                }
                else
                {
                    _viewModel.MatchWord(string.Empty);
                }
            }
            catch { }
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
            catch { }
        }

        #region 返回

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            // When Alt+Left are pressed navigate back
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            // Handle mouse back button.
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            try
            {
                if (!this.Frame.CanGoBack)
                    return false;

                this.Frame.GoBack();
                return true;
            }
            catch { }
            return false;
        }

        #endregion

    }
}
