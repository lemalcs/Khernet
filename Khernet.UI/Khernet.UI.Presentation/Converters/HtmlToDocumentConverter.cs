using System;
using System.Globalization;
using System.Windows.Documents;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts a html document to <see cref="FlowDocument"/> object
    /// </summary>
    public class HtmlToDocumentConverter : BaseValueConverter<HtmlToDocumentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            FlowDocumentHtmlConverter converter = new FlowDocumentHtmlConverter();
            converter.ConvertFromHtml(value as string);

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
