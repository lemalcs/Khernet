using System.Collections.ObjectModel;

namespace Khernet.UI.IoC
{
    public interface IChatList
    {
        /// <summary>
        /// Adss a chat list for given token
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <param name="chatList">The chat list</param>
        void AddChatList(UserItemViewModel user);

        /// <summary>
        /// Get a chat list for given <see cref="UserItemViewModel"/>
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <returns>An object <see cref="ObservableCollection{ChatMessageItemViewModel}"/> containng chat list</returns>
        ObservableCollection<ChatMessageItemViewModel> GetChat(UserItemViewModel user);

        /// <summary>
        /// Get a chat list for given token
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <returns>An object <see cref="ObservableCollection{ChatMessageItemViewModel}"/> containng chat list</returns>
        ObservableCollection<ChatMessageItemViewModel> GetChat(string token);

        /// <summary>
        /// Get user context of chat cache
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        UserChatContext GetUserContext(UserItemViewModel user);

        /// <summary>
        /// Clears the chat cache
        /// </summary>
        void Clear();
    }
}
