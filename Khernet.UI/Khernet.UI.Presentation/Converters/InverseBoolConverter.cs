using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns inverse value bool variable.
    /// </summary>
    public class InverseBoolConverter : BaseValueConverter<InverseBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;

            return !val;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
