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
        /// Contains the chat list of users.
        /// </summary>
        private ConcurrentDictionary<UserChatContext, ChatMessageCollection> messageList;

        /// <summary>
        /// Adds a chat list for given token.
        /// </summary>
        /// <param name="token">The token of user.</param>
        /// <param name="chatList">The chat list.</param>
        public void AddChatList(UserItemViewModel user)
        {
            if (user == null)
                throw new ArgumentException($"Parameter {nameof(user)} cannot be null");

            if (messageList == null)
                messageList = new ConcurrentDictionary<UserChatContext, ChatMessageCollection>();

            UserChatContext chatContext = new UserChatContext(user);

            ChatMessageCollection chatList = new ChatMessageCollection();

            messageList.AddOrUpdate(chatContext, chatList, UpdateChatList);
        }

        /// <summary>
        /// Verify if list should be updated or added.
        /// </summary>
        /// <param name="token">The context of user.</param>
        /// <param name="chatList">The chat list.</param>
        /// <returns></returns>
        private ChatMessageCollection UpdateChatList(UserChatContext chatContext, ChatMessageCollection chatList)
        {
            return chatList;
        }

        /// <summary>
        /// Get a chat list for given token.
        /// </summary>
        /// <param name="token">The token of user.</param>
        /// <returns>The chat list.</returns>
        public ChatMessageCollection GetChat(UserItemViewModel user)
        {
            if (messageList == null)
                return null;

            var chatList = messageList.FirstOrDefault((chatContext) => { return chatContext.Key.User == user; });

            return chatList.Value;
        }

        public ChatMessageCollection GetChat(string token)
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
        /// Clears the chat cache.
        /// </summary>
        public void Clear()
        {
            if (messageList != null)
                messageList.Clear();
        }
    }
}
