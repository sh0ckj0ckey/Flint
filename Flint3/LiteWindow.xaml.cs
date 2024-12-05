using System;
using System.IO;
using Flint3.Controls;
using Flint3.Data.Models;
using Flint3.ViewModels;
using Flint3.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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

        public LiteWindow(MainViewModel viewModel)
        {
            _viewModel = viewModel;

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

            // ����ϵͳ����仯
            ListenThemeColorChange();
        }

        /// <summary>
        /// ����ര�ڵĽ������õ�������
        /// </summary>
        public void TryFocusOnSearchTextBox()
        {
            var searchTextBox = FlintLitePage.SearchTextBox;
            searchTextBox?.Focus(FocusState.Keyboard);
            if (!string.IsNullOrWhiteSpace(searchTextBox?.Text))
            {
                searchTextBox?.SelectAll();
            }
        }

        /// <summary>
        /// �������������
        /// </summary>
        public void TryClearSearchTextBox()
        {
            if (FlintLitePage?.SearchTextBox is not null)
            {
                FlintLitePage.SearchTextBox.Text = "";
            }
        }

        /// <summary>
        /// ���ڼ���ʱ�Զ��۽���������ʧȥ����ʱ���ش���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnLiteWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                this.Hide();
            }
            else
            {
                TryFocusOnSearchTextBox();
            }
        }

        /// <summary>
        /// ���ݼ�����ɺ�ע���ݼ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAppTheme();

            if (sender is LitePage page)
            {
                page.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Escape, null, OnHideKeyboardAcceleratorInvoked));
                page.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Tab, null, OnSearchKeyboardAcceleratorInvoked));
            }
        }

        /// <summary>
        /// ����ϵͳ��ɫ���ñ������������Ϊ"����ϵͳ"ʱ�����л�
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
        /// �л�Ӧ�ó��������
        /// </summary>
        private void UpdateAppTheme()
        {
            try
            {
                // ���ñ�������ɫ ���� 0-System 1-Dark 2-Light
                bool isLight = true;
                if (_viewModel.AppSettings.AppearanceIndex == 0)
                {
                    var color = _uiSettings?.GetColorValue(UIColorType.Foreground) ?? Colors.Black;

                    // gԽС����ɫԽ��
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLight = g < 100;
                }
                else
                {
                    isLight = _viewModel.AppSettings.AppearanceIndex != 1;
                }

                // �޸ı�������ť��ɫ
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

                // ����Ӧ�ó�����ɫ
                if (this.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = isLight ? ElementTheme.Light : ElementTheme.Dark;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// �л�Ӧ�ó���ı�������
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
                TryFocusOnSearchTextBox();
            }
            catch { args.Handled = false; }
        }

        #endregion
    }
}
