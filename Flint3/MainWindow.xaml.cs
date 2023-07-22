using System;
using System.IO;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.ViewManagement;
using WinUIEx;
using Flint3.Views;
using Flint3.Helpers;
using Flint3.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private NotifyIcon _notifyIcon = null;

        private UISettings _uiSettings;

        private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue = null;

        public MainWindow()
        {
            this.InitializeComponent();
            this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
            this.AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logos/flint_logo.ico"));
            this.PersistenceId = "FlintMainWindow";

            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            MainViewModel.Instance.ActSwitchAppTheme = this.SwitchAppTheme;
            MainViewModel.Instance.ActPinWindow = (on) => { this.IsAlwaysOnTop = on; };
            MainViewModel.Instance.ActShowWindow = this.ShowApp;
            MainViewModel.Instance.ActHideWindow = this.HideApp;
            MainViewModel.Instance.ActExitWindow = this.ExitApp;

            // 监听系统主题变化
            ListenThemeColorChange();

            // 创建常驻托盘图标
            var hwndMain = this.GetWindowHandle();
            _notifyIcon = new NotifyIcon(hwndMain, @"Assets\Logos\flint_logo.ico");
            _notifyIcon.OnShowWindow += this.ShowApp;
            _notifyIcon.OnHideWindow += this.HideApp;
            _notifyIcon.OnExitWindow += this.ExitApp;
            _notifyIcon.CreateNotifyIcon();
        }

        /// <summary>
        /// MainFrame 加载完成后，导航到首页，注册快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainFramLoaded(object sender, RoutedEventArgs e)
        {
            // 初始导航页面
            MainFrame.Navigate(typeof(FlintPage));

            // 处理系统的返回键和退出键
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack, null, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.XButton1, null, OnGoBackKeyboardAcceleratorInvoked));
            MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Escape, null, OnHideKeyboardAcceleratorInvoked));

            // 设置全局快捷键
            MainViewModel.Instance.RegisterShortcut();
        }

        /// <summary>
        /// 主窗口获得焦点时自动聚焦到文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                MainViewModel.Instance.ActFocusOnTextBox?.Invoke();
                Debug.WriteLine("Focus on SearchTextBox");
            }
        }

        /// <summary>
        /// 主窗口关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainWindowClosed(object sender, WindowEventArgs args)
        {
            Application.Current.Exit();
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
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        SwitchAppTheme();
                    });
                }
            };
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

                TitleBarHelper.UpdateTitleBar(App.MainWindow, isLight ? ElementTheme.Light : ElementTheme.Dark);

                // 设置应用程序颜色
                if (App.MainWindow.Content is FrameworkElement rootElement)
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
        /// 显示并激活窗口
        /// </summary>
        private void ShowApp()
        {
            this.Restore();
            this.BringToFront();
        }

        /// <summary>
        /// 退出应用，移除钩子，清理托盘图标
        /// </summary>
        private void ExitApp()
        {
            MainViewModel.Instance.Dispose();
            _notifyIcon?.Destroy();
            this.Close();
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        private void HideApp()
        {
            // 弹出系统通知 TrayMessage(hWndMain, null, m_hIcon, m_hBalloonIcon, NIM.MODIFY, NIIF.USER | NIIF.LARGE_ICON, "按下快捷键或点击图标可以唤出窗口", "燧石 已隐藏至系统托盘", 0);
            this.Hide();
        }

        #region Go Back & Hide

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
                this.HideApp();
                args.Handled = true;
            }
            catch { args.Handled = false; }
        }

        private bool TryGoBack()
        {
            try
            {
                if (!MainFrame.CanGoBack) return false;
                MainFrame.GoBack();
                return true;
            }
            catch { }
            return false;
        }

        #endregion

    }
}
