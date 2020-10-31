using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> if value is not null, otherwise returns <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public class NullToStretchImageConverter : BaseValueConverter<NullToStretchImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Stretch.None;
            return Stretch.UniformToFill;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
