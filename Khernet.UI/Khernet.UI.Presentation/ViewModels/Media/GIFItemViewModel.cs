using System.Collections.ObjectModel;

namespace Khernet.UI
{
    public class GIFItemViewModel : BaseModel
    {
        /// <summary>
        /// The id of message that contains the actual GIF.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of GIF file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The path of GIF file.
        /// </summary>
        private string filePath;

        /// <summary>
        /// The width of GIF thumbnail.
        /// </summary>
        private int thumbNailWidth;

        /// <summary>
        /// The height of GIF thumbnail.
        /// </summary>
        private int thumbNailHeight;

        /// <summary>
        /// The with of GIF.
        /// </summary>
        private double width;

        /// <summary>
        /// The height of GIF.
        /// </summary>
        private double height;

        public ReadOnlyCollection<byte> Thumbnail { get; private set; }

        public int ThumbNailWidth
        {
            get => thumbNailWidth;
            set
            {
                if (thumbNailWidth != value)
                {
                    thumbNailWidth = value;
                    OnPropertyChanged(nameof(ThumbNailWidth));
                }
            }
        }
        public int ThumbNailHeight
        {
            get => thumbNailHeight;
            set
            {
                if (thumbNailHeight != value)
                {
                    thumbNailHeight = value;
                    OnPropertyChanged(nameof(ThumbNailHeight));
                }
            }
        }

        public double Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public double Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public string FilePath
        {
            get => filePath;
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        public void SetThumbnail(byte[] thumbnailBytes)
        {
            if (thumbnailBytes == null || thumbnailBytes.Length == 0)
                return;

            Thumbnail = new ReadOnlyCollection<byte>(thumbnailBytes);
        }
    }
}
