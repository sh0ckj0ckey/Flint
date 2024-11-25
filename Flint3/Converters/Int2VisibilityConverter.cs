using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class Int2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return int.Parse(value.ToString()) > 0 ? Visibility.Visible : Visibility.Collapsed;
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return int.Parse(value?.ToString() ?? "0") <= 0 ? Visibility.Visible : Visibility.Collapsed;
                }

                if (parameter != null && value != null)
                {
                    return parameter.ToString() == value.ToString() ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
