using System.Collections.ObjectModel;

namespace Khernet.UI.IoC
{
    public interface IChatList
    {
        /// <summary>
        /// Adds a chat list for given token.
        /// </summary>
        /// <param name="token">The token of user.</param>
        /// <param name="chatList">The chat list.</param>
        void AddChatList(UserItemViewModel user);

        /// <summary>
        /// Gets a chat list for given <see cref="UserItemViewModel"/>.
        /// </summary>
        /// <param name="token">The token of user.</param>
        /// <returns>An object <see cref="ChatMessageCollection"/> containing chat list.</returns>
        /*ObservableCollection<ChatMessageItemViewModel>*/
        ChatMessageCollection GetChat(UserItemViewModel user);

        /// <summary>
        /// Gets a chat list for given token.
        /// </summary>
        /// <param name="token">The token of user.</param>
        /// <returns>An object <see cref="ObservableCollection{ChatMessageItemViewModel}"/> containing chat list.</returns>
        /*ObservableCollection<ChatMessageItemViewModel>*/
        ChatMessageCollection GetChat(string token);

        /// <summary>
        /// Gets user context of chat cache.
        /// </summary>
        /// <param name="user">The model of user.</param>
        /// <returns>The context of user.</returns>
        UserChatContext GetUserContext(UserItemViewModel user);

        /// <summary>
        /// Clears the chat cache.
        /// </summary>
        void Clear();
    }
}
