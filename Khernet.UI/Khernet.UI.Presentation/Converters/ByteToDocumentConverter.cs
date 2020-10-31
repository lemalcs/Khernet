using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Convert a markdown string to <see cref="FlowDocument"/> object.
    /// </summary>
    public class ByteToDocumentConverter : BaseValueConverter<ByteToDocumentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            try
            {
                FlowDocumentMarkdownConverter converter = new FlowDocumentMarkdownConverter();

                FlowDocument fw = converter.ConvertToFlowDocument(value as string);
                fw.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#roboto");
                fw.FontSize = 12.5;
                fw.LineHeight = 20;
                return fw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Convert a markdown string to <see cref="FlowDocument"/> object.
    /// </summary>
    public class HtmlByteToDocumentConverter : BaseValueConverter<HtmlByteToDocumentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            FlowDocumentHtmlConverter converter = new FlowDocumentHtmlConverter();

            FlowDocument fw = converter.ConvertFromHtml(value as string);
            return fw;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="FlowDocument"/> to <see cref="byte"/> array.
    /// </summary>
    public class DocumentToByteConverter : BaseValueConverter<DocumentToByteConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDocument fw = value as FlowDocument;
            TextRange range = new TextRange(fw.ContentStart, fw.ContentEnd);
            byte[] messageContent = new byte[0];
            using (MemoryStream mem = new MemoryStream())
            {
                range.Save(mem, DataFormats.XamlPackage);
                messageContent = mem.ToArray();
            }
            return messageContent;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
