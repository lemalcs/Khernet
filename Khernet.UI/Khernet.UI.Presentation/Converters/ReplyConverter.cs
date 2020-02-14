using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts <see cref="MessageDirection"/> value to string that represents an icon name
    /// </summary>
    public class MessageOperationToStringConverter : BaseValueConverter<MessageOperationToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MessageDirection)value) == MessageDirection.Reply ? "Reply" : "ArrowRightBold";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
