using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns a different <see cref="Brush"/> whether user is selected or not.
    /// </summary>
    public class IsSelectedToBrushConverter : BaseValueConverter<IsSelectedToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Application.Current.FindResource("LightBrush") : Application.Current.FindResource("BlueBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
