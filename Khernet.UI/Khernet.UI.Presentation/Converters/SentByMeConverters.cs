using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Changes background color of chat message based on message sender.
    /// </summary>
    public class SentByMeToChatBackgroudBrushConverter : BaseValueConverter<SentByMeToChatBackgroudBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Change background color for chat bubble anchor
            if ((bool)value)
            {
                return Application.Current.FindResource("AlmostWhiteGrayBrush");
            }
            return Application.Current.FindResource("LightBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Changes margin of message state indicator based on message sender.
    /// </summary>
    public class SentByMeToMarginConverter : BaseValueConverter<SentByMeToMarginConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new Thickness(20, 0, 0, 0);
            }
            return new Thickness(0, 0, 0, 0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
