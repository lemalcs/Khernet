using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a string RGB hexadecimal value like A34F6D to an WPF Brush.
    /// </summary>
    public class StatusToIconConverter : BaseValueConverter<StatusToIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !System.Convert.ToBoolean(value))
                return "Exclamation";

            return "Check";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
