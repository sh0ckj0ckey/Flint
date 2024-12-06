using System;
using System.IO;
using Flint3.ViewModels;
using Flint3.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private UISettings _uiSettings = null;

        public MainWindow()
        {
            this.InitializeComponent();
            this.PersistenceId = "FlintMainWindow";
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            this.UpdateAppBackdrop();

            string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets/Logos/flint_logo.ico");
            this.SetIcon(iconPath);
            this.SetTaskBarIcon(Icon.FromFile(iconPath));

            MainViewModel.Instance.AppSettings.OnAppearanceSettingChanged += (_) =>
            {
                this.UpdateAppTheme();
            };

            MainViewModel.Instance.AppSettings.OnBackdropSettingChanged += (_) =>
            {
                this.UpdateAppBackdrop();
            };

            // 监听系统主题变化
            ListenThemeColorChange();

            // 首次启动设置默认窗口尺寸
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["firstRun"] == null)
            {
                localSettings.Values["firstRun"] = true;
                this.Height = 386;
                this.Width = 580;
                this.CenterOnScreen();
            }
        }

        /// <summary>
        /// 将主窗口的焦点设置到搜索框，如果主窗口此时不在搜索页面则不会生效
        /// </summary>
        public void TryFocusOnSearchTextBox()
        {
            if (MainFrame.Content is FlintFullPage flintPage)
            {
                var searchTextBox = flintPage.SearchTextBox;
                searchTextBox?.Focus(FocusState.Keyboard);
                if (!string.IsNullOrWhiteSpace(searchTextBox?.Text))
                {
                    searchTextBox?.SelectAll();
                }
            }
        }

        /// <summary>
        /// 尝试清空搜索框
        /// </summary>
        public void TryClearSearchTextBox()
        {
            if (MainFrame.Content is FlintFullPage flintPage)
            {
                var searchTextBox = flintPage.SearchTextBox;
                if (searchTextBox is not null)
                {
                    searchTextBox.Text = "";
                }
            }
        }

        /// <summary>
        /// 窗口激活时自动聚焦到搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                TryFocusOnSearchTextBox();
            }
        }

        /// <summary>
        /// MainFrame 加载完成后，导航到首页，注册快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainFrameLoaded(object sender, RoutedEventArgs e)
        {
            // 初始导航页面
            MainFrame.Navigate(typeof(FlintFullPage));

            this.UpdateAppTheme();

            // 处理系统的返回键和退出键
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack, null, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.XButton1, null, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Escape, null, OnHideKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Tab, null, OnSearchKeyboardAcceleratorInvoked));
        }

        /// <summary>
        /// 监听系统颜色设置变更，处理主题为"跟随系统"时主题切换
        /// </summary>
        private void ListenThemeColorChange()
        {
            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += (s, args) =>
            {
                if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0)
                {
                    MainViewModel.Instance.Dispatcher.TryEnqueue(() =>
                    {
                        UpdateAppTheme();
                    });
                }
            };
        }

        /// <summary>
        /// 切换应用程序的主题
        /// </summary>
        private void UpdateAppTheme()
        {
            try
            {
                // 设置标题栏颜色 主题 0-System 1-Dark 2-Light
                bool isLight = true;
                if (MainViewModel.Instance.AppSettings.AppearanceIndex == 0)
                {
                    var color = _uiSettings?.GetColorValue(UIColorType.Foreground) ?? Colors.Black;

                    // g越小，颜色越深
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLight = g < 100;
                }
                else
                {
                    isLight = MainViewModel.Instance.AppSettings.AppearanceIndex != 1;
                }

                // 修改标题栏按钮颜色
                // TitleBarHelper.UpdateTitleBar(App.MainWindow, isLight ? ElementTheme.Light : ElementTheme.Dark);
                var titleBar = this.AppWindow.TitleBar;
                // Set active window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.ForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.BackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonHoverBackgroundColor = isLight ? Windows.UI.Color.FromArgb(10, 0, 0, 0) : Windows.UI.Color.FromArgb(16, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonPressedBackgroundColor = isLight ? Windows.UI.Color.FromArgb(08, 0, 0, 0) : Windows.UI.Color.FromArgb(10, 255, 255, 255);

                // Set inactive window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                // 设置应用程序颜色
                if (this.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = isLight ? ElementTheme.Light : ElementTheme.Dark;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 切换应用程序的背景材质
        /// </summary>
        private void UpdateAppBackdrop()
        {
            this.SystemBackdrop = MainViewModel.Instance.AppSettings.BackdropIndex == 1 ? new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop() : new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }

        #region Go Back & Hide KeyboardAccelerators

        private KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers, Action<KeyboardAccelerator, KeyboardAcceleratorInvokedEventArgs> callback)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += (s, args) => callback?.Invoke(s, args);

            return keyboardAccelerator;
        }

        private void OnGoBackKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = TryGoBack();
        }

        private void OnHideKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            try
            {
                args.Handled = true;
                this.Hide();
            }
            catch { args.Handled = false; }
        }

        private void OnSearchKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            try
            {
                args.Handled = true;
                TryFocusOnSearchTextBox();
            }
            catch { args.Handled = false; }
        }

        private bool TryGoBack()
        {
            try
            {
                if (!MainFrame.CanGoBack)
                {
                    return false;
                }

                MainFrame.GoBack();
                return true;
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return false;
        }

        #endregion
    }
}
