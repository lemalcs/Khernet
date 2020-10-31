using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a string RGB hexadecimal value like A34F6D to an WPF Brush.
    /// </summary>
    public class StringRGBToColorConverter : BaseValueConverter<StringRGBToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (string.IsNullOrEmpty(value.ToString()) || value.ToString().Length != 6))
                return (SolidColorBrush)Application.Current.Resources["DefaultUserItemBrush"];

            return (SolidColorBrush)(new BrushConverter()).ConvertFromString($"#{value}");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
