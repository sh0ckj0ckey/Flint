using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class Int2BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (parameter == null && value != null)
                {
                    return int.Parse(value.ToString()) > 0;
                }

                if (parameter != null && value != null && parameter.ToString() == "-")
                {
                    return int.Parse(value?.ToString() ?? "0") <= 0;
                }

                if (parameter != null && value != null)
                {
                    return parameter.ToString() == value.ToString();
                }
            }
            catch { }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
