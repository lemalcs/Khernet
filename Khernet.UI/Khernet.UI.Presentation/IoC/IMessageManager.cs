namespace Khernet.UI.IoC
{
    public interface IMessageManager
    {
        /// <summary>
        /// Send a message to specific user
        /// </summary>
        /// <param name="messageModel">The message that is being sended</param>
        /// <param name="receiver">The receipt user of message</param>
        void ResendMessage(ChatMessageItemViewModel messageModel, UserItemViewModel receiver = null);

        /// <summary>
        /// Send a reply to the given message
        /// </summary>
        /// <param name="messageModel">The message that is being replied</param>
        void SendReplyMessage(ChatMessageItemViewModel messageModel);
    }
}
