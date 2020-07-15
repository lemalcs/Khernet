using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns a <see cref="double"/> height reduced in a amount or a default value if zero.
    /// </summary>
    public class ImageHeightConverter : BaseValueConverter<ImageHeightConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)value;

            return height / 2.7;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the first height value that is not zero.
    /// </summary>
    public class ImageHeightMultiConverter : BaseMultiValueConverter<ImageHeightMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return 0;

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return 0;

            double height = (double)values[0];

            if (height > 0)
                return height / 2.7;

            double alternateValue;

            if (double.TryParse(values[1].ToString(), out alternateValue))
                return alternateValue;

            return 0;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
