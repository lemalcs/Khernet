using System;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Holds the informations about selected emoji.
    /// </summary>
    public class SelectedEmojiEventArgs : EventArgs
    {
        public string EmojiCode { get; private set; }
        public SelectedEmojiEventArgs(string emojiCode)
        {
            EmojiCode = emojiCode;
        }
    }

    public class SelectedGIFEventArgs : EventArgs
    {
        public BaseModel AnimationModel { get; private set; }
        public SelectedGIFEventArgs(BaseModel animationModel)
        {
            AnimationModel = animationModel;
        }
    }
}
