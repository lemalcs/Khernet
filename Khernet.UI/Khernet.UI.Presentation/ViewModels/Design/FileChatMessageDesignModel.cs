using Khernet.UI.Managers;

namespace Khernet.UI.ViewModels
{
    public class FileChatMessageDesignModel : FileChatMessageViewModel
    {
        public FileChatMessageDesignModel() : base(new ChatMessageListDesignModel(),new PresentationApplicationDialog())
        {
            FileName = "AdobeDreamweaver10en_USLanguagePack.msi";
            IsFileLoaded = true;
            IsReadingFile = false;
            IsLoading = false;
            CurrentReadBytes = 120;
            FileSize = 125;
            IsSentByMe = true;

            SetChatState(ChatMessageState.Processed);
        }
    }
}
