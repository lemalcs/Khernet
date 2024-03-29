﻿namespace Khernet.UI
{
    /// <summary>
    /// View model for text messages.
    /// </summary>
    public class TextChatMessageDesignModel : TextChatMessageViewModel
    {
        public TextChatMessageDesignModel() : base(new ChatMessageListDesignModel())
        {
            IsSentByMe = true;
            DisplayUser = new UserItemViewModel
            {
                Initials = "L",
            };
        }
    }
}