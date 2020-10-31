using Khernet.UI.Media;
using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts <see cref="MessageDirection"/> value to string that represents an icon name.
    /// </summary>
    public class MessageFormatToStringConverter : BaseValueConverter<MessageFormatToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (MessageType)value == MessageType.Markdown;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? MessageType.Markdown : MessageType.Html;
        }
    }

    /// <summary>
    /// If message type is <see cref="MessageType.Markdown"/> returns <see cref="Thickness"/> with a value of 1 otherwise 0.
    /// </summary>
    public class MessageFormatToThicknessConverter : BaseValueConverter<MessageFormatToThicknessConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (MessageType)value == MessageType.Markdown ? new Thickness(1) : new Thickness(0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? MessageType.Markdown : MessageType.Html;
        }
    }
}
