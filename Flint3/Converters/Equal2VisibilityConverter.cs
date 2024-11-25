using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class Equal2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null && parameter != null)
                {
                    return value.ToString().ToLower() == parameter.ToString().ToLower() ? Visibility.Visible : Visibility.Collapsed;
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
