namespace Khernet.UI
{
    /// <summary>
    /// Results of operations done asynchronously.
    /// </summary>
    public enum ProgressResultIcon
    {
        /// <summary>
        /// Successful operation.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Warnings raised after completing operation.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error encountered after completing operation.
        /// </summary>
        Error = 2,
    }

    /// <summary>
    /// View model for about information.
    /// </summary>
    public class ProgressDialogViewModel : BaseModel
    {
        /// <summary>
        /// Message to be show about current process.
        /// </summary>
        private string textProgress;


        /// <summary>
        /// Indicates if an operation is executing.
        /// </summary>
        private bool isExecuting;

        /// <summary>
        /// The result of operation.
        /// </summary>
        private ProgressResultIcon result;

        public string TextProgress
        {
            get => textProgress;
            set
            {
                if (textProgress != value)
                {
                    textProgress = value;
                    OnPropertyChanged(nameof(TextProgress));
                }

            }
        }
        public bool IsExecuting
        {
            get => isExecuting;
            set
            {
                if (isExecuting != value)
                {
                    isExecuting = value;
                    OnPropertyChanged(nameof(IsExecuting));
                }

            }
        }

        public ProgressResultIcon Result 
        { 
            get => result;
            set 
            { 
                if (result != value)
                {
                    result = value;
                    OnPropertyChanged(nameof(Result));
                }
            }
        }

        public ProgressDialogViewModel()
        {
        }
    }
}
