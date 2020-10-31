using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts <see cref="bool"/> value radius to <see cref="Thickness"/> for right and left arcs.
    /// </summary>
    public class BoolToThicknessConverter : BaseValueConverter<BoolToThicknessConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //True if message is waiting for a reply
            //False if message already was replied
            return (bool)value ? new Thickness(0) : new Thickness(4, 0, 0, 0);
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
