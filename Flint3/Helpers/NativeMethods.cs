using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Helpers
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, NativeKeyboardHelper.INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern short GetAsyncKeyState(int vKey);
    }
}
