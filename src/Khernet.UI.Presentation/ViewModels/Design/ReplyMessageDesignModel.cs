namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class ReplyMessageDesignModel : ReplyMessageViewModel
    {
        public ReplyMessageDesignModel()
        {
            User = new UserItemViewModel
            {
            };
            IsReplying = true;
            IconName = "Play";
            FileName = "thisaudio.mp3";

            Operation = MessageDirection.Reply;
        }
    }
}