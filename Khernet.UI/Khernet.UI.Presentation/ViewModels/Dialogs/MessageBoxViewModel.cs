using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// The result of message box when it is closed.
    /// </summary>
    public enum MessageBoxResponse
    {
        Accept = 0,
        Cancel = 1
    }


    public class MessageBoxViewModel : BaseModel
    {
        /// <summary>
        /// The title for dialog windows.
        /// </summary>
        private string title;

        /// <summary>
        /// The label for accept option.
        /// </summary>
        private string acceptOptionLabel;

        /// <summary>
        /// The label for cancel option.
        /// </summary>
        private string cancelOptionLabel;

        /// <summary>
        /// Indicates whether to show accept option.
        /// </summary>
        private bool showAcceptOption;

        /// <summary>
        /// Indicates whether to show the cancel option.
        /// </summary>
        private bool showCancelOption;

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
        /// The message for dialog window.
        /// </summary>
        private string message;

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public string AcceptOptionLabel
        {
            get => acceptOptionLabel;
            set
            {
                if (acceptOptionLabel != value)
                {
                    acceptOptionLabel = value;
                    OnPropertyChanged(nameof(AcceptOptionLabel));
                }
            }
        }
        public string CancelOptionLabel
        {
            get => cancelOptionLabel;
            set
            {
                if (cancelOptionLabel != value)
                {
                    cancelOptionLabel = value;
                    OnPropertyChanged(nameof(CancelOptionLabel));
                }
            }
        }

        public bool ShowAcceptOption
        {
            get => showAcceptOption;
            set
            {
                if (showAcceptOption != value)
                {
                    showAcceptOption = value;
                    OnPropertyChanged(nameof(ShowAcceptOption));
                }
            }
        }
        public bool ShowCancelOption
        {
            get => showCancelOption;
            set
            {
                if (showCancelOption != value)
                {
                    showCancelOption = value;
                    OnPropertyChanged(nameof(ShowCancelOption));
                }
            }
        }

        public MessageBoxResponse Result { get; private set; }


        public ICommand AcceptCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }


        public MessageBoxViewModel()
        {
            AcceptCommand = new RelayCommand(Accept);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Cancel()
        {
            Result = MessageBoxResponse.Cancel;
        }

        private void Accept()
        {
            Result = MessageBoxResponse.Accept;
        }
    }
}
