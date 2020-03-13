using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
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

            if (state == ChatMessageState.Error|| state == ChatMessageState.UnCommited)
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

    /// <summary>
    /// Shows or hides a control based on <see cref="ChatMessageState"/>.
    /// </summary>
    public class ChatStateToVisibilityConverter : BaseValueConverter<ChatStateToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var chatState = (ChatMessageState)value;
            return chatState == ChatMessageState.Error||chatState==ChatMessageState.UnCommited ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Shows or hides a control based on <see cref="ChatMessageState"/> and if message was loaded successfully.
    /// </summary>
    public class ChatStateToVisibilityMultiConverter : BaseMultiValueConverter<ChatStateToVisibilityMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            bool isFileLoaded = (bool)values[0];
            ChatMessageState state = (ChatMessageState)values[1];

            return isFileLoaded&&(state==ChatMessageState.Processed||state==ChatMessageState.Pendding)?Visibility.Visible:Visibility.Collapsed;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="ChatMessageState"/> to a <see cref="Cursor"/>.
    /// </summary>
    public class ChatStateToCursorMultiConverter : BaseMultiValueConverter<ChatStateToCursorMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            bool isFileLoaded = (bool)values[0];
            ChatMessageState state = (ChatMessageState)values[1];

            return isFileLoaded && (state == ChatMessageState.Processed || state == ChatMessageState.Pendding) ? Cursors.Hand : Cursors.Arrow;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
