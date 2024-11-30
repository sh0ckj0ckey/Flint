using System;
using Microsoft.UI.Xaml.Data;

namespace Flint3.Converters
{
    internal class LineBreaksRemover : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is string)
                {
                    string text = value.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        text = text.Trim();
                        text.Replace("\r\n", "   ");
                        text = text.Replace("\n", "   ");
                        return text;
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
