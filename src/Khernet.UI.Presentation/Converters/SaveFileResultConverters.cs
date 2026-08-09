using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a <see cref="SaveFileResult"/> value to a <see cref="Visibility"/> value.
    /// </summary>
    public class SaveFileResultToVisibilityConverter : BaseValueConverter<SaveFileResultToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SaveFileResult)value == SaveFileResult.Saved ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a a <see cref="SaveFileResult"/> value to a <see cref="Brush"/> value.
    /// </summary>
    public class SaveFileResultToBrushConverter : BaseValueConverter<SaveFileResultToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SaveFileResult)value)
            {
                case SaveFileResult.Saved:
                    return Application.Current.FindResource("GreenBrush");
                case SaveFileResult.Failed:
                    return Application.Current.FindResource("LightRedBrush");

                default:
                    return Application.Current.FindResource("OverTextboxBorderBrush");
            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
