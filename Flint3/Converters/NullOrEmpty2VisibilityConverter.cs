using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class NullOrEmpty2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null)
                {
                    return (value == null || string.IsNullOrWhiteSpace(value?.ToString())) ? Visibility.Collapsed : Visibility.Visible;
                }

                if (parameter != null && parameter.ToString() == "-")
                {
                    return (value == null || string.IsNullOrWhiteSpace(value?.ToString())) ? Visibility.Visible : Visibility.Collapsed;
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
