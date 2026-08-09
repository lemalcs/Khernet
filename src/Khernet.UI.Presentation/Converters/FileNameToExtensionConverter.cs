using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Gets the extension of file name.
    /// </summary>
    public class FileNameToExtensionConverter : BaseValueConverter<FileNameToExtensionConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return System.IO.Path.GetExtension(value as string).Replace(".", "").ToUpper();
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
