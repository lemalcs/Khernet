namespace Khernet.UI
{
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

        public ProgressDialogViewModel()
        {
        }
    }
}
