namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class VideoChatMessageDesignModel : VideoChatMessageViewModel
    {
        public VideoChatMessageDesignModel() : base(new ChatMessageListDesignModel())
        {
            FileName = "avideo.mp4";
            FilePath = @"C:\MediaFiles\avideo.mp4";
            IsReadingFile = true;
        }
    }
}