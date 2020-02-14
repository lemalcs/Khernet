namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class AnimationChatMessageDesignModel : AnimationChatMessageViewModel
    {
        public AnimationChatMessageDesignModel() : base(new ChatMessageListDesignModel())
        {
            FilePath = @"C:\MediaFiles\02 Pista 2.wma";
            IsFileLoaded = true;
        }
    }
}