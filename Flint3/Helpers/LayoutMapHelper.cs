using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flint3.Helpers
{
    public class LayoutMapHelper
    {
        private static readonly interop.LayoutMapManaged LayoutMap = new();

        public static string GetKeyName(uint key)
        {
            return LayoutMap.GetKeyName(key);
        }
    }
}
