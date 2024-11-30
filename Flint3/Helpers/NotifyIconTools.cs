using System;
using System.Runtime.InteropServices;

namespace Flint3.Helpers
{
    // https://github.com/castorix/WinUI3_NotifyIcon
    public class NotifyIconTools
    {
        public const int WM_USER = 0x0400;
        public const int WM_TRAYMOUSEMESSAGE = WM_USER + 1024;

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] NOTIFYICONDATA lpdata);

        public static bool TrayMessage(IntPtr hWnd, string sMessage, IntPtr hIcon, IntPtr hBalloonIcon, NIM nMessage, NIIF dwInfoFlags, string sInfo, string sTitle, int nTimeOut)
        {
            NOTIFYICONDATA nid = new NOTIFYICONDATA();

            //nid.cbSize = 956;

            /*if (IsOsVistaOrLater())
            nid.cbSize = sizeof(NOTIFYICONDATA);
            else
            nid.cbSize = NOTIFYICONDATA_V3_SIZE;*/
            nid.uID = 1;
            nid.hWnd = hWnd;
            nid.uFlags = NIF.MESSAGE | NIF.INFO;
            nid.uCallbackMessage = WM_TRAYMOUSEMESSAGE;

            if (sMessage != null && sMessage != "")
            {
                nid.szTip = sMessage;
                nid.uFlags |= NIF.TIP;
            }

            if (hIcon != IntPtr.Zero)
            {
                nid.hIcon = hIcon;
                nid.uFlags |= NIF.ICON;
            }

            if ((dwInfoFlags & NIIF.USER) != 0)
            {
                nid.hIcon = hIcon;
                nid.hBalloonIcon = hBalloonIcon;
                nid.uFlags |= NIF.ICON;
            }

            nid.dwInfoFlags = dwInfoFlags;
            if (dwInfoFlags != 0 && sInfo != null && sTitle != null)
            {
                nid.szInfo = sInfo;
                nid.szInfoTitle = sTitle;
            }
            else
            {
                nid.szInfo = null;
                nid.szInfoTitle = null;
            }

            bool bRet;
            nid.uVersion = 4;
            if (nMessage == NIM.ADD)
            {
                Shell_NotifyIcon(NIM.SETVERSION, nid);
            }

            bRet = Shell_NotifyIcon(nMessage, nid);
            return bRet;
        }

        public const int NIS_HIDDEN = 0x00000001;
        public const int NIS_SHAREDICON = 0x00000002;
    }

    public enum NIM : uint
    {
        ADD = 0,
        MODIFY = 1,
        DELETE = 2,
        SETFOCUS = 3,
        SETVERSION = 4,
    }

    // From WPF/Winforms source
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class NOTIFYICONDATA
    {
        public int cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
        public IntPtr hWnd;
        public int uID;
        public NIF uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public int dwState = 0;
        public int dwStateMask = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        //public char[] szInfo = new char[256];
        // Prior to Vista this was a union of uTimeout and uVersion.  As of Vista, uTimeout has been deprecated.
        public uint uVersion;  // Used with Shell_NotifyIcon flag NIM_SETVERSION. 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public NIIF dwInfoFlags;
        public Guid guidItem;
        // Vista only
        public IntPtr hBalloonIcon;
    }

    [Flags]
    public enum NIS : uint
    {
        HIDDEN = 0x00000001,
        SHAREDICON = 0x00000002
    }

    [Flags]
    public enum NIF : uint
    {
        MESSAGE = 0x0001,
        ICON = 0x0002,
        TIP = 0x0004,
        STATE = 0x0008,
        INFO = 0x0010,
        GUID = 0x0020,
        REALTIME = 0x0040,
        SHOWTIP = 0x0080,
        XP_MASK = MESSAGE | ICON | STATE | INFO | GUID,
        VISTA_MASK = XP_MASK | REALTIME | SHOWTIP,
    }

    [Flags]
    public enum NIIF : uint
    {
        NONE = 0x00000000,
        // icon flags are mutually exclusive
        // and take only the lowest 2 bits
        INFO = 0x00000001,
        WARNING = 0x00000002,
        ERROR = 0x00000003,
        USER = 0x00000004,
        ICON_MASK = 0x0000000F,
        NOSOUND = 0x00000010,
        LARGE_ICON = 0x00000020,
        RESPECT_QUIET_TIME = 0x00000080
    }
}
