using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using WinUIEx;
using Flint3.Views;
using Flint3.Helpers;

using static Flint3.Helpers.NotifyIconTools;
using ABI.Windows.Foundation;
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
        public MainWindow()
        {
            this.InitializeComponent();
            this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();

            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logos/flint_logo.ico"));
            PersistenceId = "FlintMainWindow";

            // 初始导航页面，注册后退快捷键
            MainFrame.Loaded += (s, e) =>
            {
                MainViewModel.Instance.ActHideWindow = () => HideInTray();

                MainViewModel.Instance.ActPinWindow = (on) => { this.IsAlwaysOnTop = on; };

                MainFrame.Navigate(typeof(FlintPage));

                // 处理系统的返回键
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
                MainFrame.KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.XButton1));
            };

            hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(this);
            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bRet = SetWindowSubclass(hWndMain, SubClassDelegate, 0, 0);

            m_hIcon = LoadImage(IntPtr.Zero, @"Assets\Logos\flint_logo.ico", IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
            m_hBalloonIcon = LoadImage(IntPtr.Zero, @"Assets\Logos\flint_logo.ico", IMAGE_ICON, 128, 128, LR_LOADFROMFILE);

            // 创建常驻托盘图标
            TrayMessage(hWndMain, "燧石", m_hIcon, IntPtr.Zero, NIM.ADD, NIIF.NONE, null, null, 0);
        }

        private void OnMainWindowClosed(object sender, WindowEventArgs args)
        {
            CleanTray();
        }

        #region Go Back

        private KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

            return keyboardAccelerator;
        }

        private void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = TryGoBack();
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

        #region NotifyIcon

        // 来源 https://github.com/castorix/WinUI3_NotifyIcon

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }
        public static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_CONTEXTMENU = 0x007B;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_ENTERMENULOOP = 0x0211;
        public const int WM_EXITMENULOOP = 0x0212;
        public const int WM_INITMENUPOPUP = 0x0117;
        public const int WM_UNINITMENUPOPUP = 0x0125;

        public const int WM_DRAWITEM = 0x002B;
        public const int WM_MEASUREITEM = 0x002C;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetCursorPos(out Windows.Graphics.PointInt32 lpPoint);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreatePopupMenu();

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool AppendMenu(IntPtr hMenu, uint uFlags, IntPtr uIDNewItem, string lpNewItem);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ModifyMenu(IntPtr hMnu, uint uPosition, uint uFlags, IntPtr uIDNewItem, IntPtr lpNewItem);

        public const int MF_STRING = 0x00000000;
        public const int MF_BITMAP = 0x00000004;
        public const int MF_OWNERDRAW = 0x00000100;

        public const int MF_POPUP = 0x00000010;
        public const int MF_MENUBARBREAK = 0x00000020;
        public const int MF_MENUBREAK = 0x00000040;
        public const int MF_SEPARATOR = 0x00000800;

        public const int MF_BYCOMMAND = 0x00000000;
        public const int MF_BYPOSITION = 0x00000400;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        public const int TPM_LEFTBUTTON = 0x0000;
        public const int TPM_RIGHTBUTTON = 0x0002;
        public const int TPM_LEFTALIGN = 0x0000;
        public const int TPM_CENTERALIGN = 0x0004;
        public const int TPM_RIGHTALIGN = 0x0008;
        public const int TPM_TOPALIGN = 0x0000;
        public const int TPM_VCENTERALIGN = 0x0010;
        public const int TPM_BOTTOMALIGN = 0x0020;
        public const int TPM_HORIZONTAL = 0x0000;     /* Horz alignment matters more */
        public const int TPM_VERTICAL = 0x0040;     /* Vert alignment matters more */
        public const int TPM_NONOTIFY = 0x0080;     /* Don't send any notification msgs */
        public const int TPM_RETURNCMD = 0x0100;
        public const int TPM_RECURSE = 0x0001;
        public const int TPM_HORPOSANIMATION = 0x0400;
        public const int TPM_HORNEGANIMATION = 0x0800;
        public const int TPM_VERPOSANIMATION = 0x1000;
        public const int TPM_VERNEGANIMATION = 0x2000;
        public const int TPM_NOANIMATION = 0x4000;
        public const int TPM_LAYOUTRTL = 0x8000;
        public const int TPM_WORKAREA = 0x10000;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetProcAddress")]
        public static extern IntPtr GetProcAddressByOrdinal(IntPtr hModule, int lpProcName);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadImage(IntPtr hInst, string lpszName, UInt32 uType, int cxDesired, int cyDesired, UInt32 fuLoad);

        public const int IMAGE_BITMAP = 0;
        public const int IMAGE_ICON = 1;
        public const int IMAGE_CURSOR = 2;
        public const int IMAGE_ENHMETAFILE = 3;

        public const int LR_DEFAULTCOLOR = 0x00000000;
        public const int LR_MONOCHROME = 0x00000001;
        public const int LR_COLOR = 0x00000002;
        public const int LR_COPYRETURNORG = 0x00000004;
        public const int LR_COPYDELETEORG = 0x00000008;
        public const int LR_LOADFROMFILE = 0x00000010;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        private IntPtr hWndMain = IntPtr.Zero;
        private IntPtr m_hIcon = IntPtr.Zero;
        private IntPtr m_hBalloonIcon = IntPtr.Zero;

        private SUBCLASSPROC SubClassDelegate;

        private void HideInTray()
        {
            // 弹出系统通知 TrayMessage(hWndMain, null, m_hIcon, m_hBalloonIcon, NIM.MODIFY, NIIF.USER | NIIF.LARGE_ICON, "按下快捷键或点击图标可以唤出窗口", "燧石 已隐藏至系统托盘", 0);
            this.Hide();
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_TRAYMOUSEMESSAGE:
                    {
                        switch (LOWORD((int)lParam))
                        {
                            case WM_LBUTTONUP:
                                this.Restore();
                                this.BringToFront();
                                break;
                            case WM_CONTEXTMENU:
                            case WM_RBUTTONUP:
                                {
                                    Windows.Graphics.PointInt32 ptCursor;
                                    GetCursorPos(out ptCursor);

                                    IntPtr hMenu = CreatePopupMenu();
                                    int i = 1;
                                    AppendMenu(hMenu, MF_STRING, (IntPtr)i, "显示窗口");
                                    i += 1;
                                    // 添加分割线 AppendMenu(hMenu, MF_SEPARATOR, (IntPtr)i, null); i += 1;
                                    AppendMenu(hMenu, MF_STRING, (IntPtr)i, "退出");
                                    SetForegroundWindow(hWnd);
                                    uint nCmd = TrackPopupMenu(hMenu, TPM_LEFTALIGN | TPM_LEFTBUTTON | TPM_RIGHTBUTTON | TPM_RETURNCMD, ptCursor.X, ptCursor.Y, 0, hWnd, IntPtr.Zero);
                                    PostMessage(hWnd, 0x0000, IntPtr.Zero, IntPtr.Zero);
                                    if (nCmd != 0)
                                    {
                                        if (nCmd == i)
                                        {
                                            this.Close();
                                        }
                                        else if (nCmd == 1)
                                        {
                                            this.Restore();
                                            this.BringToFront();
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        private void CleanTray()
        {
            // 移除托盘图标
            TrayMessage(hWndMain, null, IntPtr.Zero, IntPtr.Zero, NIM.DELETE, NIIF.NONE, null, null, 0);

            if (m_hIcon != IntPtr.Zero)
                DestroyIcon(m_hIcon);
            if (m_hBalloonIcon != IntPtr.Zero)
                DestroyIcon(m_hBalloonIcon);
        }

        #endregion

    }
}
