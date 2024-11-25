using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flint3.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Flint3.Data.Models;

namespace Flint3.Converters
{
    internal class GlossaryEnum2NameConverter : IValueConverter
    {
        private static Dictionary<GlossaryColorsEnum, SolidColorBrush> _glossaryColors = new();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                GlossaryColorsEnum color = (GlossaryColorsEnum)value;
                switch (color)
                {
                    case GlossaryColorsEnum.Transparent:
                        return "全部";
                    case GlossaryColorsEnum.Red:
                        return "红色";
                    case GlossaryColorsEnum.Orange:
                        return "橙色";
                    case GlossaryColorsEnum.Yellow:
                        return "黄色";
                    case GlossaryColorsEnum.Green:
                        return "绿色";
                    case GlossaryColorsEnum.Blue:
                        return "蓝色";
                    case GlossaryColorsEnum.Purple:
                        return "紫色";
                    case GlossaryColorsEnum.Pink:
                        return "粉色";
                    case GlossaryColorsEnum.Brown:
                        return "棕色";
                    case GlossaryColorsEnum.Gray:
                        return "灰色";
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
