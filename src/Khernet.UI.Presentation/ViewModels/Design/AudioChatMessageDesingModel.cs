using Khernet.UI.Managers;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class AudioChatMessageDesingModel : AudioChatMessageViewModel
    {
        public AudioChatMessageDesingModel() : base(new ChatMessageListViewModel(), new PresentationApplicationDialog())
        {
            FileName = "some_music.mp3";
            IsReadingFile = false;
            IsLoading = false;
            IsFileLoaded = true;
        }
    }
}