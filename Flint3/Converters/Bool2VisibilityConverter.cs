using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return bool.Parse(value?.ToString() ?? "False") ? Visibility.Visible : Visibility.Collapsed;
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return !bool.Parse(value?.ToString() ?? "True") ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch { }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
