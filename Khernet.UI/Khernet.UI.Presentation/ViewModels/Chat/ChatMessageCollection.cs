using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Khernet.UI
{
    /// <summary>
    /// Double linked chat list.
    /// </summary>
    public class ChatMessageCollection:ObservableCollection<ChatMessageItemViewModel>
    {
        /// <summary>
        /// Adds a message to the bottom of list.
        /// </summary>
        /// <param name="chatMessage">The chat message to add to.</param>
        public new void Add(ChatMessageItemViewModel chatMessage)
        {
            if (chatMessage == null)
                return;

            if (Count > 0)
                chatMessage.PreviousChatMessage = this[Count - 1];
            
            base.Add(chatMessage);

            if (Count > 0)
                this[Count - 1].NextChatMessage = chatMessage;
        }

        /// <summary>
        /// Adds a message to a specific position in chat list.
        /// </summary>
        /// <param name="index">The index of chat list to add message.</param>
        /// <param name="chatMessage">The chat message to add to.</param>
        public new void Insert(int index,ChatMessageItemViewModel chatMessage)
        {
            if (chatMessage == null)
                return;

            if (index - 1 >= 0)
                chatMessage.PreviousChatMessage = this[index-1];

            if (index >= 0 && index < Count)
                chatMessage.NextChatMessage = this[index];

            InsertItem(index, chatMessage);

            if (index + 1 < Count)
                this[index + 1].PreviousChatMessage = chatMessage;
        }
    }
}
