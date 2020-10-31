using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Vlc.DotNet.Core.Interops.Signatures;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// Change a string state based on <see cref="MediaStates"/>.
    /// </summary>
    public class MediaStatesToStringConverter : BaseValueConverter<MediaStatesToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Get milliseconds from video duration
            MediaStates state = (MediaStates)value;
            return state == MediaStates.Playing ? "Pause" : "Play";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts and boolean value to <see cref="Cursor"/>.
    /// </summary>
    public class BoolToCursorConverter : BaseValueConverter<BoolToCursorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Returns arrow when reading a file and hand when file is reading to open
            return (bool)value ? Cursors.Hand : Cursors.Arrow;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Shows or hides current time of playing audio.
    /// </summary>
    public class MediaStatesToVisibilityConverter : BaseValueConverter<MediaStatesToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MediaStates state = (MediaStates)value;

            //Returns arrow when reading a file and hand when file is reading to open
            return state == MediaStates.Playing || state == MediaStates.Paused ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
