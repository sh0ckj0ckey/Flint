using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Helpers
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X { get; set; }

        public int Y { get; set; }

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
