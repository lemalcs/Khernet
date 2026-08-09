using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    public class ZIndexConverter : BaseValueConverter<ZIndexConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //If message box is visible return 3 otherwise return 1
            return (bool)value ? 3 : 1;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
