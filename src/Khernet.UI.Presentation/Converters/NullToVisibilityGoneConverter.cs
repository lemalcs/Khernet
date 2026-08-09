using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> if value is not null, otherwise returns <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public class NullToVisibilityGoneConverter : BaseValueConverter<NullToVisibilityGoneConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> if value is null, otherwise returns <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public class InverseNullToVisibilityGoneConverter : BaseValueConverter<InverseNullToVisibilityGoneConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
