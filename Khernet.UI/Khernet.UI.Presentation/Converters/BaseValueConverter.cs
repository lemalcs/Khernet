using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Value Converter for direct access to XAML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        /// <summary>
        /// Instance for specific value converter
        /// </summary>
        private static T converter = null;

        #region IValueConverter methods
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
        #endregion

        #region MarkupExtension methods
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? new T();
        }
        #endregion
    }
}
