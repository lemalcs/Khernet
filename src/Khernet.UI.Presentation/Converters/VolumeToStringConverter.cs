using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Change a volume value to a icon name.
    /// </summary>
    public class VolumeToStringConverter : BaseValueConverter<VolumeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMuted = (bool)value;
            return isMuted ? "VolumeOff" : "VolumeHigh";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
