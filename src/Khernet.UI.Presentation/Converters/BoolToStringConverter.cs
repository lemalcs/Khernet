using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Returns a message which indicates whether a user is writing or sending a file.
    /// </summary>
    public class BoolToStringConverter : BaseMultiValueConverter<BoolToStringConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return string.Empty;

            bool isWriting = (bool)values[0];
            bool isSendingFile = (bool)values[1];

            if (isWriting)
                return "Typing message...";

            if (isSendingFile)
                return "Sending file...";

            return string.Empty;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
