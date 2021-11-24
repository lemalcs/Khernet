using System;
using System.Globalization;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Converts the result of operation to a icon name.
    /// </summary>
    public class ResultToIconConverter : BaseValueConverter<ResultToIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if ((ProgressResultIcon)value == ProgressResultIcon.Success)
                return "CheckCircleOutline";

            if ((ProgressResultIcon)value == ProgressResultIcon.Warning)
                return "AlertOutline";

            return "CloseCircleOutline";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
