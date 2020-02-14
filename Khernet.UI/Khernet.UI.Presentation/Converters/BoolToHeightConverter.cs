using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns <see cref="Visibility.Visible"/> if value is not null, otherwise returns <see cref="Visibility.Collapsed"/>
    /// </summary>
    public class BoolToHeightConverter : BaseValueConverter<BoolToHeightConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            bool isChecked = (bool)value;

            if (!isChecked)
                return new GridLength(0);

            if (parameter == null)
                return GridLength.Auto;

            if (parameter.ToString() == "*")
                return new GridLength(1, GridUnitType.Star);

            if (parameter.ToString() == "Auto")
                return new GridLength(1, GridUnitType.Auto);

            double height;
            if (double.TryParse(parameter.ToString(), out height))
                return new GridLength(height);
            else
                return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
