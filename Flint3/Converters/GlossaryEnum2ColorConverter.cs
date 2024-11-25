using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flint3.Data.Models;
using Flint3.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Flint3.Converters
{
    internal class GlossaryEnum2ColorConverter : IValueConverter
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
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Transparent))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Transparent, new SolidColorBrush(Colors.Transparent));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Transparent];
                    case GlossaryColorsEnum.Red:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Red))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Red, new SolidColorBrush(Colors.Firebrick));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Red];
                    case GlossaryColorsEnum.Orange:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Orange))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Orange, new SolidColorBrush(Colors.Tomato));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Orange];
                    case GlossaryColorsEnum.Yellow:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Yellow))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Yellow, new SolidColorBrush(Colors.Gold));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Yellow];
                    case GlossaryColorsEnum.Green:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Green))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Green, new SolidColorBrush(Colors.ForestGreen));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Green];
                    case GlossaryColorsEnum.Blue:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Blue))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Blue, new SolidColorBrush(Colors.DodgerBlue));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Blue];
                    case GlossaryColorsEnum.Purple:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Purple))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Purple, new SolidColorBrush(Colors.Orchid));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Purple];
                    case GlossaryColorsEnum.Pink:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Pink))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Pink, new SolidColorBrush(Colors.DeepPink));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Pink];
                    case GlossaryColorsEnum.Brown:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Brown))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Brown, new SolidColorBrush(Colors.Sienna));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Brown];
                    case GlossaryColorsEnum.Gray:
                        if (!_glossaryColors.ContainsKey(GlossaryColorsEnum.Gray))
                        {
                            _glossaryColors.Add(GlossaryColorsEnum.Gray, new SolidColorBrush(Colors.DimGray));
                        }
                        return _glossaryColors[GlossaryColorsEnum.Gray];
                    default:
                        break;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}