using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> time value to seconds and vice-versa.
    /// </summary>
    public class TimeSpanToSecondsConverter : BaseValueConverter<TimeSpanToSecondsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Duration)
            {
                Duration time = (Duration)value;
                if (!time.HasTimeSpan)
                    return 0;

                return time.TimeSpan.TotalSeconds;
            }
            else
                return ((TimeSpan)value).TotalSeconds;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = TimeSpan.FromTicks(System.Convert.ToInt64(TimeSpan.TicksPerSecond * (double)value));

            if (targetType == typeof(TimeSpan))
                return result;

            if (targetType == typeof(Duration))
                return new Duration(result);

            return Activator.CreateInstance(targetType);
        }
    }
}
