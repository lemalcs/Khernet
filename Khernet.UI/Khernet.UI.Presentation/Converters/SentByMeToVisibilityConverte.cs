using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    public class SentByMeToVisibilityConverter : BaseValueConverter<SentByMeToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sentByMe = (bool)value;

            //Hide profile picture if message was sent by current user
            //otherwise shows profile picture
            if (sentByMe)
            {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Makes visible or invisible the background profile color of chat message within chat list
    /// </summary>
    public class SentByMeToVisibilityMultiConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return null;

            bool isSentByMe = (bool)values[0];

            if (isSentByMe)
                return Visibility.Hidden;

            var avatar = (ReadOnlyCollection<byte>)values[1];

            return avatar != null ? Visibility.Hidden : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
