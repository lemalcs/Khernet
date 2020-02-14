using Khernet.UI.Media;
using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a <see cref="MessageType"/> enumaration to friendly name
    /// </summary>
    public class MessageTypeToFriendlyName : BaseValueConverter<MessageTypeToFriendlyName>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultValue = "Message";
            if (value == null)
                return defaultValue;

            switch ((MessageType)value)
            {
                case MessageType.Html:
                case MessageType.Markdown:
                    return "Text";
                case MessageType.Image:
                    return "Image";
                case MessageType.GIF:
                    return "GIF";
                case MessageType.Binary:
                    return "File";
                case MessageType.Audio:
                    return "Audio";
                case MessageType.Video:
                    return "Video";
                default:
                    return defaultValue;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
