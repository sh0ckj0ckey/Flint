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

        /// <summary>
        /// 应用程序系统托盘
        /// </summary>
        private NotifyIcon _notifyIcon = null;

        /// <summary>
        /// 燧石的主窗口，点击桌面图标或者右下角托盘时一定打开这个窗口
        /// </summary>
        public MainWindow FlintMainWindow { get; private set; } = null;

        /// <summary>
        /// 燧石的简洁搜索窗口，设置中开启简洁窗口后，按下快捷键会唤起这个窗口
        /// </summary>
        public LiteWindow FlintLiteWindow { get; private set; } = null;

        private MainViewModel() { }

        public void Initialize(Microsoft.UI.Dispatching.DispatcherQueue dispatcher)
        {
            this.Dispatcher = dispatcher;

            this.FlintMainWindow = new MainWindow(this);

            // 创建常驻托盘图标
            var hwndMain = this.FlintMainWindow.GetWindowHandle();
            _notifyIcon = new NotifyIcon(hwndMain, @"Assets\Logos\flint_logo.ico");
            _notifyIcon.OnClickShowMainWindow += () =>
            {
                this.ShowMainWindow();
            };

            _notifyIcon.OnClickExitApp += () =>
            {
                this.ExitApp();
            };

            _notifyIcon.OnClickCloseWindow += () =>
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

            _notifyIcon.CreateNotifyIcon();

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
            _notifyIcon?.Destroy();
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
            this.FlintMainWindow.Restore();
            this.FlintMainWindow.BringToFront();
            this.FlintMainWindow.Activate();

            if (this.AppSettings.AutoClearLastInput)
            {
                this.SearchingWord = "";
                this.FlintMainWindow.CenterOnScreen();
            }
        }

        public void ShowLiteWindow()
        {
            this.FlintMainWindow?.Hide();

            this.FlintLiteWindow ??= new LiteWindow(this);
            IntPtr hwnd = this.FlintLiteWindow.GetWindowHandle();

            if (this.AppSettings.AutoClearLastInput)
            {
                this.SearchingWord = "";
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
            int height = string.IsNullOrWhiteSpace(this.SearchingWord) ? 64 : 386;

            PInvoke.GetDpiForMonitor(hwndDesktop, Windows.Win32.UI.HiDpi.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
            var scalingFactor = dpiX / 96d;

            //var dpi = PInvoke.GetDpiForWindow(new HWND(focusedWindow));
            //var scalingFactor = dpi / 96d;

            var w = (int)(width * scalingFactor);
            var h = (int)(height * scalingFactor);
            var cx = (info.rcMonitor.left + info.rcMonitor.right) / 2;
            var cy = (info.rcMonitor.bottom + info.rcMonitor.top) / 4;
            var left = cx - (w / 2);
            var top = cy - (h / 2);

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
