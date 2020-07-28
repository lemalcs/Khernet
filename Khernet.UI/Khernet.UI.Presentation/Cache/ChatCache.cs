using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

namespace Khernet.UI.Cache
{
    public class ChatCache : IChatList
    {
        /// <summary>
        /// Contains the chat list of users
        /// </summary>
        private ConcurrentDictionary<UserChatContext, ObservableCollection<ChatMessageItemViewModel>> messageList;

        /// <summary>
        /// Adss a chat list for given token
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <param name="chatList">The chat list</param>
        public void AddChatList(UserItemViewModel user)
        {
            if (user == null)
                throw new ArgumentException($"Parameter {nameof(user)} cannot be null");

            if (messageList == null)
                messageList = new ConcurrentDictionary<UserChatContext, ObservableCollection<ChatMessageItemViewModel>>();

            UserChatContext chatContext = new UserChatContext(user);

            ObservableCollection<ChatMessageItemViewModel> chatList = new ObservableCollection<ChatMessageItemViewModel>();

            messageList.AddOrUpdate(chatContext, chatList, UpdateChatList);
        }

        /// <summary>
        /// Verify if list should be updated or added
        /// </summary>
        /// <param name="token"></param>
        /// <param name="chatList"></param>
        /// <returns></returns>
        private ObservableCollection<ChatMessageItemViewModel> UpdateChatList(UserChatContext chatContext, ObservableCollection<ChatMessageItemViewModel> chatList)
        {
            return chatList;
        }

        /// <summary>
        /// Get a chat list for given token
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <returns>The chat list </returns>
        public ObservableCollection<ChatMessageItemViewModel> GetChat(UserItemViewModel user)
        {
            if (messageList == null)
                return null;

            var chatList = messageList.FirstOrDefault((chatContext) => { return chatContext.Key.User == user; });

            return chatList.Value;
        }

        public ObservableCollection<ChatMessageItemViewModel> GetChat(string token)
        {
            if (messageList == null)
                return null;

            var chatList = messageList.FirstOrDefault((chatContext) => { return chatContext.Key.User.Token == token; });

            return chatList.Value;
        }

        public UserChatContext GetUserContext(UserItemViewModel user)
        {
            if (messageList == null)
                return null;

            var userContext = messageList.FirstOrDefault((chatContext) => { return chatContext.Key.User == user; });

            return userContext.Key;
        }

        /// <summary>
        /// Clears the chat cache
        /// </summary>
        public void Clear()
        {
            if (messageList != null)
                messageList.Clear();
        }
    }
}
