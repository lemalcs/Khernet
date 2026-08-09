using System;
using System.Globalization;
using System.Windows;

namespace Khernet.UI.Converters
{
    public class BoolToVisibilityConverter : BaseValueConverter<BoolToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    class InverseBoolToVisibilityConverter : BaseValueConverter<InverseBoolToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Hidden : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Shows an indicator when peer is writing or sending a file otherwise hides it.
    /// </summary>
    public class BoolToVisibilityMultiConverter : BaseMultiValueConverter<BoolToVisibilityMultiConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return Visibility.Collapsed;

            bool isWriting = (bool)values[0];
            bool isSendingFile = (bool)values[1];

            if (isWriting || isSendingFile)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
