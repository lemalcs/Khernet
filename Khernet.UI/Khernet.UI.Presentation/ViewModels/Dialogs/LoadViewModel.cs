namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile
    /// </summary>
    public class LoadViewModel : BaseModel
    {

        #region Properties

        private bool showProgress;

        private string messageText;

        public string MessageText
        {
            get => messageText;
            set
            {
                if (messageText != value)
                {
                    messageText = value;
                    OnPropertyChanged(nameof(MessageText));
                }
            }
        }

        public bool ShowProgress
        {
            get => showProgress;
            set
            {
                if (showProgress != value)
                {
                    showProgress = value;
                    OnPropertyChanged(nameof(ShowProgress));
                }
            }
        }

        #endregion
    }
}
