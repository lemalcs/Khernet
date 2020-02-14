using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Khernet.UI.Converters
{
    /// <summary>
    /// COnvert an image stored in a <see cref="ReadOnlyCollection{T}"/> into a <see cref="BitmapImage"/> object.
    /// </summary>
    public class BytesToImageConverter : BaseValueConverter<BytesToImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Get image from cache
            BitmapImage img = null;

            if (value != null && value is ReadOnlyCollection<byte> thumb)
            {
                if (thumb.Count == 0)
                    return null;

                MemoryStream mem = new MemoryStream(((ReadOnlyCollection<byte>)value).ToArray());

                img = new BitmapImage();

                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = mem;
                img.EndInit();
            }

            return img;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts emoji code to image path
    /// </summary>
    public class NameToImageConverter : BaseValueConverter<NameToImageConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return $"pack://application:,,,/Khernet.UI.Container;component/{value.ToString()}.png";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
