using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Helpers
{
    public class KeyHelper
    {
        private static readonly interop.LayoutMapManaged LayoutMap = new interop.LayoutMapManaged();

        public static string GetKeyName(uint key)
        {
            return LayoutMap.GetKeyName(key);
        }

        public const uint VirtualKeyWindows = 0x104;/*Fake key code to represent VK_WIN: VK_WIN_BOTH*/ //interop.Constants.VK_WIN_BOTH;
    }
}
