using Khernet.UI.Managers;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class VideoChatMessageDesignModel : VideoChatMessageViewModel
    {
        public VideoChatMessageDesignModel() : base(new ChatMessageListDesignModel(), new PresentationApplicationDialog())
        {
            FileName = "avideo.mp4";
            FilePath = @"C:\MediaFiles\avideo.mp4";
            IsReadingFile = true;
        }
    }
}