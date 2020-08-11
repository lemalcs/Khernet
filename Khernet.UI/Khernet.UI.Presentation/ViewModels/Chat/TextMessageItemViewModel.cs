using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;

namespace Khernet.UI
{
    /// <summary>
    /// Base class for text messages such as include HTML, markdown and XAML formats.
    /// </summary>
    public abstract class TextMessageItemViewModel : ChatMessageItemViewModel
    {
        /// <summary>
        /// Gets a copy of this message
        /// </summary>
        /// <returns></returns>
        public abstract ChatMessageItemViewModel GetInstanceCopy();

        public abstract void Send(byte[] message);

        public override void Load(MessageItem messageItem)
        {
            if (messageItem == null)
                throw new NullReferenceException($"Parameter {nameof(messageItem)} cannot be null.");

            Id = messageItem.Id;
            UID = messageItem.UID;
            TimeId = messageItem.TimeId;
            SendDate = messageItem.RegisterDate;
            IsSentByMe = messageItem.IdSenderPeer == 0;
            IsRead = messageItem.IsRead;
            State = (ChatMessageState)(int)messageItem.State;
        }
    }
}
