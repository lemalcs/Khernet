using System.Windows.Controls;

namespace Khernet.UI
{
    public class ModalDialogViewModel : BaseModel
    {
        /// <summary>
        /// Content hosted inside dialog.
        /// </summary>
        private Control content;

        /// <summary>
        /// The maximum height of dialog.
        /// </summary>
        private double maxheight;

        /// <summary>
        /// The minimum height of dialog.
        /// </summary>
        private double minheight;

        /// <summary>
        /// Indicates if dialog must be in full screen.
        /// </summary>
        private bool isFullScreen;

        /// <summary>
        /// The width of modal dialog.
        /// </summary>
        private double width;

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

        public double Maxheight
        {
            get => maxheight;
            set
            {
                if (maxheight != value)
                {
                    maxheight = value;
                    OnPropertyChanged(nameof(Maxheight));
                }
            }
        }

        public double MinHeight
        {
            get => minheight;
            set
            {
                if (minheight != value)
                {
                    minheight = value;
                    OnPropertyChanged(nameof(MinHeight));
                }
            }
        }

        public bool IsFullScreen
        {
            get => isFullScreen;
            set
            {
                if (isFullScreen != value)
                {
                    isFullScreen = value;
                    OnPropertyChanged(nameof(IsFullScreen));
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
    }
}
