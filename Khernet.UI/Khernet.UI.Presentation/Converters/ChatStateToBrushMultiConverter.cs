using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    public abstract class BaseMultiValueConverter<T> : MarkupExtension, IMultiValueConverter
        where T : new()
    {
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new T();
        }
    }

    /// <summary>
    /// Sets color of anchor for chat messages based on the current state <see cref="ChatMessageState"/>
    /// </summary>
    public class ChatStateToBrushMultiConverter : BaseMultiValueConverter<ChatStateToBrushMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;


            bool isSentByMe = (bool)values[0];
            ChatMessageState state = (ChatMessageState)values[1];

            if (state == ChatMessageState.Error)
                return Application.Current.FindResource("DarkRedBrush");

            if (!isSentByMe || state == ChatMessageState.Pendding)
                return Application.Current.FindResource("LightGrayAnchorBrush");

            return Application.Current.FindResource("LightBlueAnchorBrush");
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
