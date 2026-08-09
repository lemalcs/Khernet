using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns True online updates are enabled otherwise false.
    /// </summary>
    public class UpdateOnlineConverter : BaseValueConverter<UpdateOnlineConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "Yes" : "No";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
