using System.Windows.Controls;

namespace Khernet.UI
{
    public class DialogWindowViewModel : BaseModel
    {
        /// <summary>
        /// Title of dialog window.
        /// </summary>
        private string title;

        /// <summary>
        /// The color of background dialog.
        /// </summary>
        private string backgroundColorHex;

        /// <summary>
        /// Indicates if windows has full screen size.
        /// </summary>
        private bool isFullScreen;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Content hosted inside dialog.
        /// </summary>
        private Control content;

        public Control Content
        {
            get
            {
                return content;
            }

            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public string BackgroudColorHex
        {
            get
            {
                return backgroundColorHex;
            }

            set
            {
                if (backgroundColorHex != value)
                {
                    backgroundColorHex = value;
                    OnPropertyChanged(nameof(BackgroudColorHex));
                }
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return isFullScreen;
            }

            set
            {
                if (isFullScreen != value)
                {
                    isFullScreen = value;
                    OnPropertyChanged(nameof(IsFullScreen));
                }
            }
        }
    }
}
