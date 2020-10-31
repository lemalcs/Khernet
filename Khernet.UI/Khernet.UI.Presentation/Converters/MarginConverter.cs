using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a boolean value of full screen to <see cref="Thickness"/> object.
    /// </summary>
    public class FullScreenToMarginConverter : BaseValueConverter<FullScreenToMarginConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new Thickness(0) : new Thickness(0, 10, 0, 10);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts full screen mode to <see cref="VerticalAlignment.Stretch"/> when true or <see cref="VerticalAlignment.Center"/> when false.
    /// </summary>
    public class FullScreenToAligmentConverter : BaseValueConverter<FullScreenToAligmentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? VerticalAlignment.Stretch : VerticalAlignment.Center;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
