using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static Flint3.Helpers.NotifyIconTools;

namespace Flint3.Helpers
{
    // https://github.com/castorix/WinUI3_NotifyIcon
    public class NotifyIcon
    {
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

        public const int WM_CLOSE = 0x0010;
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
        public static extern int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #region Menu

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

        #endregion

        #region Load Image

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

        #endregion


        [DllImport("User32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int RegisterWindowMessage(string msg);

        public static readonly int WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");


        private readonly IntPtr m_hWndMain = IntPtr.Zero;
        private readonly IntPtr m_hIcon = IntPtr.Zero;
        private readonly IntPtr m_hBalloonIcon = IntPtr.Zero;

        private readonly SUBCLASSPROC SubClassDelegate;

        internal Action OnWindowHide { get; set; } = null;
        internal Action OnWindowClose { get; set; } = null;
        internal Action OnWindowActivate { get; set; } = null;

        internal NotifyIcon(nint hwndMain, string iconPath)
        {
            m_hWndMain = hwndMain;

            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bRet = SetWindowSubclass(m_hWndMain, SubClassDelegate, 0, 0);

            m_hIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 32, 32, LR_LOADFROMFILE);
            m_hBalloonIcon = LoadImage(IntPtr.Zero, iconPath, IMAGE_ICON, 128, 128, LR_LOADFROMFILE);
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            // 任务栏重新创建时，重新添加托盘图标
            if ((int)uMsg == WM_TASKBARCREATED)
            {
                this.CreateNotifyIcon();
            }

            switch (uMsg)
            {
                case WM_CLOSE:
                    this.OnWindowHide?.Invoke();
                    return (int)IntPtr.Zero;
                case WM_TRAYMOUSEMESSAGE:
                    {
                        switch (LOWORD((int)lParam))
                        {
                            case WM_LBUTTONUP:
                                this.OnWindowActivate?.Invoke();
                                break;
                            case WM_CONTEXTMENU:
                            case WM_RBUTTONUP:
                                {
                                    GetCursorPos(out Windows.Graphics.PointInt32 ptCursor);

                                    IntPtr hMenu = CreatePopupMenu();
                                    int i = 1;
                                    AppendMenu(hMenu, MF_STRING, (IntPtr)i, "显示窗口");
                                    i += 1;
                                    // 添加分割线 AppendMenu(hMenu, MF_SEPARATOR, (IntPtr)i, null); i += 1;
                                    AppendMenu(hMenu, MF_STRING, (IntPtr)i, "退出");
                                    SetForegroundWindow(hWnd);
                                    uint nCmd = TrackPopupMenu(hMenu, TPM_LEFTALIGN | TPM_LEFTBUTTON | TPM_RIGHTBUTTON | TPM_RETURNCMD, ptCursor.X, ptCursor.Y, 0, hWnd, IntPtr.Zero);
                                    _ = PostMessage(hWnd, 0x0000, IntPtr.Zero, IntPtr.Zero);
                                    if (nCmd != 0)
                                    {
                                        if (nCmd == i)
                                        {
                                            this.OnWindowClose?.Invoke();
                                        }
                                        else if (nCmd == 1)
                                        {
                                            this.OnWindowActivate?.Invoke();
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

        /// <summary>
        /// 创建托盘图标
        /// </summary>
        internal void CreateNotifyIcon()
        {
            TrayMessage(m_hWndMain, "燧石", m_hIcon, IntPtr.Zero, NIM.ADD, NIIF.NONE, null, null, 0);
        }

        /// <summary>
        /// 移除托盘图标
        /// </summary>
        internal void Destroy()
        {
            TrayMessage(m_hWndMain, null, IntPtr.Zero, IntPtr.Zero, NIM.DELETE, NIIF.NONE, null, null, 0);

            if (m_hIcon != IntPtr.Zero) DestroyIcon(m_hIcon);
            if (m_hBalloonIcon != IntPtr.Zero) DestroyIcon(m_hBalloonIcon);
        }
    }
}
