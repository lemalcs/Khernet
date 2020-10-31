using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts circle radius to <see cref="Size"/> for right and left arcs.
    /// </summary>
    public class UnreadCountWidthConverter : BaseValueConverter<UnreadCountWidthConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minWidth = 20;

            //If message box is visible return 3 otherwise return 1
            return (double)value < minWidth ? minWidth : (double)value + 6;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
