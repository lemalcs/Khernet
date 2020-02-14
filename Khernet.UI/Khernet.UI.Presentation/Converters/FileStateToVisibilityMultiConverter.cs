using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Shows or hides download icon for files
    /// </summary>
    public class FileStateToVisibilityMultiConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue ||
                values[2] == DependencyProperty.UnsetValue)
                return null;

            bool reading = (bool)values[0];
            bool loading = (bool)values[1];
            bool loaded = (bool)values[2];

            //If file is not ready to open and it is not bing processed then make download icon visible
            return !reading && !loading && !loaded ? Visibility.Visible : Visibility.Hidden;
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
