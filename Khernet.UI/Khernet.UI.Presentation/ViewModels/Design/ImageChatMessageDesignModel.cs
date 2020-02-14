namespace Khernet.UI
{
    /// <summary>
    /// View model for text messages.
    /// </summary>
    public class ImageChatMessageDesignModel : ImageChatMessageViewModel
    {
        public ImageChatMessageDesignModel() : base(new ChatMessageListDesignModel())
        {
            IsSentByMe = false;
            User = new UserItemViewModel
            {
                Initials = "L",
            };
        }
    }
}