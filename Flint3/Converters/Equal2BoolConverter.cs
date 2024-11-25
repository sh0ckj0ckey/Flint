using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class Equal2BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && parameter != null)
                {
                    return value.ToString().ToLower() == parameter.ToString().ToLower();
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
