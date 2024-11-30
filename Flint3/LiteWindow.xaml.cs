using System;
using System.IO;
using Flint3.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
    public sealed partial class LiteWindow : WindowEx
    {
        private readonly MainViewModel _viewModel = null;

        private UISettings _uiSettings = null;

        public LiteWindow()
        {
            _viewModel = MainViewModel.Instance;

            this.InitializeComponent();
            this.PersistenceId = "FlintLiteWindow";
            this.IsShownInSwitchers = false;
            this.ExtendsContentIntoTitleBar = true;
            this.IsTitleBarVisible = false;
            this.IsAlwaysOnTop = true;
            this.IsResizable = false;
            this.IsMaximizable = false;
            this.IsMinimizable = false;

            this.UpdateAppBackdrop();

            string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets/Logos/flint_logo.ico");
            this.SetIcon(iconPath);
            this.SetTaskBarIcon(Icon.FromFile(iconPath));

            _viewModel.AppSettings.OnAppearanceSettingChanged += (_) =>
            {
                this.UpdateAppTheme();
            };

            _viewModel.AppSettings.OnBackdropSettingChanged += (_) =>
            {
                this.UpdateAppBackdrop();
            };

            // 监听系统主题变化
            ListenThemeColorChange();
        }

        private void WindowEx_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                this.Hide();
            }
            else
            {
                FocusOnTextBox?.Invoke();
            }
        }

        /// <summary>
        /// 内容加载完成后，注册快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAppTheme();

            if (sender is Grid grid)
            {
                grid.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Escape, null, OnHideKeyboardAcceleratorInvoked));
                grid.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Tab, null, OnSearchKeyboardAcceleratorInvoked));
            }
        }

        /// <summary>
        /// 监听系统颜色设置变更，处理主题为"跟随系统"时主题切换
        /// </summary>
        private void ListenThemeColorChange()
        {
            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += (s, args) =>
            {
                if (_viewModel.AppSettings.AppearanceIndex == 0)
                {
                    _viewModel.Dispatcher.TryEnqueue(() =>
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
                if (_viewModel.AppSettings.AppearanceIndex == 0)
                {
                    var color = _uiSettings?.GetColorValue(UIColorType.Foreground) ?? Colors.Black;

                    // g越小，颜色越深
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLight = g < 100;
                }
                else
                {
                    isLight = _viewModel.AppSettings.AppearanceIndex != 1;
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
            this.SystemBackdrop = _viewModel.AppSettings.BackdropIndex == 1 ? new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop() : new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }

        #region Hide KeyboardAccelerators

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
                SearchTextBox.Focus(FocusState.Keyboard);
            }
            catch { args.Handled = false; }
        }

        #endregion
    }
}
