using Khernet.UI.Media;
using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a long value to a double value according to bytes units.
    /// </summary>
    public class SizeUnitConverter : BaseValueConverter<SizeUnitConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(FileHelper.ConvertToSmallestSize((long)value), 2);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a long value to a double value according to bytes units.
    /// </summary>
    public class SizeToUnitNameConverter : BaseValueConverter<SizeToUnitNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName(typeof(ByteUnit), FileHelper.GetSmallestUnit((long)value));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
