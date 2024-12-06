using System;
using System.Drawing;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Flint3.Core;
using Flint3.Data;
using Flint3.Helpers;
using Microsoft.UI.Xaml;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace Flint3.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private static readonly Lazy<MainViewModel> _instance = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _instance.Value;

        public Microsoft.UI.Dispatching.DispatcherQueue Dispatcher { get; private set; } = null;

        /// <summary>
        /// 应用程序设置
        /// </summary>
        public SettingsService AppSettings { get; set; } = new SettingsService();

        private MainViewModel() { }

        public void Initialize(Microsoft.UI.Dispatching.DispatcherQueue dispatcher)
        {
            this.Dispatcher = dispatcher;

            this.FlintMainWindow = new MainWindow(this);

            // 创建常驻托盘图标
            var hwndMain = this.FlintMainWindow.GetWindowHandle();
            NotifyIcon = new NotifyIcon(hwndMain, @"Assets\Logos\flint_logo.ico");
            NotifyIcon.OnClickShowMainWindow += () =>
            {
                this.ShowMainWindow();
            };

            NotifyIcon.OnClickExitApp += () =>
            {
                this.ExitApp();
            };

            NotifyIcon.OnClickCloseWindow += () =>
            {
                if (this.AppSettings.CloseButtonMode == 0)
                {
                    this.HideApp();
                }
                else
                {
                    this.ExitApp();
                }
            };

            NotifyIcon.CreateNotifyIcon();

            InitViewModel4Flint();
            InitViewModel4Glossary();
            InitViewModel4ShortcutKeys();
        }

        /// <summary>
        /// 隐藏燧石的所有窗口
        /// </summary>
        public void HideApp()
        {
            this.FlintLiteWindow?.Hide();
            this.FlintMainWindow?.Hide();

            // 弹出系统通知
            // TrayMessage(hWndMain, null, m_hIcon, m_hBalloonIcon, NIM.MODIFY, NIIF.USER | NIIF.LARGE_ICON, "按下快捷键或点击图标可以唤出窗口", "燧石 已隐藏至系统托盘", 0);
        }

        /// <summary>
        /// 退出燧石
        /// </summary>
        public void ExitApp()
        {
            this.HideApp();
            GlossaryDataAccess.CloseDatabase();
            StarDictDataAccess.CloseDatabase();
            NotifyIcon?.Destroy();
            this.FlintLiteWindow?.Close();
            this.FlintMainWindow?.Close();
            this.Dispose();

            // Environment.Exit(0);
            Application.Current.Exit();
        }

        public void ShowMainWindow()
        {
            this.FlintLiteWindow?.Hide();

            this.FlintMainWindow ??= new MainWindow(this);

            if (this.AppSettings.AutoClearLastInput)
            {
                this.SearchResultWordItems.Clear();
                this.FlintMainWindow.TryClearSearchTextBox();
                this.FlintMainWindow.CenterOnScreen();
            }

            this.FlintMainWindow.Restore();
            this.FlintMainWindow.BringToFront();
            this.FlintMainWindow.Activate();
        }

        public void ShowLiteWindow()
        {
            this.FlintMainWindow?.Hide();

            this.FlintLiteWindow ??= new LiteWindow(this);
            IntPtr hwnd = this.FlintLiteWindow.GetWindowHandle();

            if (this.AppSettings.AutoClearLastInput)
            {
                this.LiteSearchResultWordItems.Clear();
                this.FlintLiteWindow.TryClearSearchTextBox();
            }

            // 获取鼠标当前所在的显示器
            PInvoke.GetCursorPos(out Point pt);
            var hwndDesktop = PInvoke.MonitorFromPoint(pt, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);

            // 获取当前激活窗口所在的显示器
            //IntPtr focusedWindow = PInvoke.GetForegroundWindow();
            //var hwndDesktop = PInvoke.MonitorFromWindow(new(focusedWindow), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);

            MONITORINFO info = new MONITORINFO();
            info.cbSize = 40;
            PInvoke.GetMonitorInfo(hwndDesktop, ref info);
            int width = 540;
            int height = this.LiteSearchResultWordItems.Count <= 0 ? 64 : 386;

            PInvoke.GetDpiForMonitor(hwndDesktop, Windows.Win32.UI.HiDpi.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
            var scalingFactor = dpiX / 96d;

            //var dpi = PInvoke.GetDpiForWindow(new HWND(focusedWindow));
            //var scalingFactor = dpi / 96d;

            var w = (int)(width * scalingFactor);
            var h = (int)(height * scalingFactor);
            var cx = (info.rcMonitor.left + info.rcMonitor.right) / 2;
            var cy = (info.rcMonitor.bottom + info.rcMonitor.top) / 4;
            var left = cx - (w / 2);
            var top = cy;

            bool result = PInvoke.SetWindowPos(new HWND(hwnd), new HWND(), left, top, w, h, (SET_WINDOW_POS_FLAGS)0);
            if (!result)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
            }

            this.FlintLiteWindow.Restore();
            this.FlintLiteWindow.BringToFront();
            this.FlintLiteWindow.Activate();
        }
    }
}
