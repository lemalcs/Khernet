using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Hides anchor if previous message belongs the same user as current.
    /// </summary>
    public class ChatAnchorToVisibilityMultiConverter : BaseMultiValueConverter<ChatAnchorToVisibilityMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            if (values[0] == null || values[1] == null)
                return Visibility.Visible;

            bool currentChatSender = (bool)values[0];
            bool nextChatSender = (bool)values[1];

            return currentChatSender ^ nextChatSender ? Visibility.Visible:Visibility.Collapsed;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
