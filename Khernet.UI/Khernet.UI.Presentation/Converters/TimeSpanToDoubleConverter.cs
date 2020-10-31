using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Get milliseconds from video duration.
    /// </summary>
    public class TimeSpanToDoubleConverter : BaseValueConverter<TimeSpanToDoubleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = (TimeSpan)value;
            return time.TotalMilliseconds;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
