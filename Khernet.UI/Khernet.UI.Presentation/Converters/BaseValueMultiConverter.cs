using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Multi value converter for direct access to XAML
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
}
