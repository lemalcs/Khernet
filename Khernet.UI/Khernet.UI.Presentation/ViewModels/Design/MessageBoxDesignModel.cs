namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class MessageBoxDesignModel : MessageBoxViewModel
    {
        public MessageBoxDesignModel()
        {
            ShowAcceptOption = true;
            AcceptOptionLabel = "Yes";
        }
    }
}