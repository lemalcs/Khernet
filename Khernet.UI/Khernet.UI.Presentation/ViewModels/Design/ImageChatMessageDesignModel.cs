﻿using Khernet.UI.Managers;

namespace Khernet.UI
{
    /// <summary>
    /// View model for text messages.
    /// </summary>
    public class ImageChatMessageDesignModel : ImageChatMessageViewModel
    {
        public ImageChatMessageDesignModel() : base(new ChatMessageListDesignModel(), new PresentationApplicationDialog())
        {
            IsSentByMe = false;
            User = new UserItemViewModel
            {
                Initials = "L",
            };
        }
    }
}