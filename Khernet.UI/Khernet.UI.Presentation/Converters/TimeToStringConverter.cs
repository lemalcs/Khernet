using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a <see cref="DateTimeOffset"/> to a short string representation.
    /// </summary>
    public class TimeToStringConverter : BaseValueConverter<TimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset date = ((DateTimeOffset)value).ToLocalTime();

            //When message is sent at the same day, just show hour
            if (date.Date.ToShortDateString() == DateTimeOffset.Now.Date.ToShortDateString())
                return date.ToString("HH:mm");

            //Otherwise some date and hour
            return date.ToString("yyyy-MM-dd HH:mm");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a short string representation.
    /// </summary>
    public class TimeSpanToStringConverter : BaseValueConverter<TimeSpanToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (TimeSpan)value;

            if (Math.Truncate(time.TotalDays) > 0)//Verify if video lasts one day or more
                return time.ToString(@"dd\.hh\:mm\:ss");
            else if (Math.Truncate(time.TotalHours) > 0)//Verify if video lasts one hour or more
                return time.ToString(@"hh\:mm\:ss");
            else
                return time.ToString(@"mm\:ss");//Video lasts less than one hour
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="long"/> time representation to a short string representation.
    /// </summary>
    class NumberToTimeStringConverter : BaseValueConverter<NumberToTimeStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = TimeSpan.FromMilliseconds((long)value);

            if (Math.Truncate(time.TotalDays) > 0)//Verify if video lasts one day or more
                return time.ToString(@"dd\.hh\:mm\:ss");
            else if (Math.Truncate(time.TotalHours) > 0)//Verify if video lasts one hour or more
                return time.ToString(@"hh\:mm\:ss");
            else
                return time.ToString(@"mm\:ss");//Video lasts less than one hour
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
