using System;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class String2BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return !string.IsNullOrWhiteSpace(value?.ToString());
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return string.IsNullOrWhiteSpace(value?.ToString());
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
