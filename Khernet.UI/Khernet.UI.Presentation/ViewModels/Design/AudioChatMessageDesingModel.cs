namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class AudioChatMessageDesingModel : AudioChatMessageViewModel
    {
        public AudioChatMessageDesingModel() : base(new ChatMessageListViewModel())
        {
            FileName = "some_music.mp3";
            IsReadingFile = false;
            IsFileLoaded = true;
        }
    }
}