using System;
using System.Collections.ObjectModel;

namespace Khernet.UI
{
    public class ChatMessageListDesignModel : ChatMessageListViewModel
    {
        public ChatMessageListDesignModel()
        {
            var items = new ObservableCollection<ChatMessageItemViewModel>
            {
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = false
                },
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = true
                },
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = true
                },
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = false
                },
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = true
                },
                new TextChatMessageViewModel(this)
                {
                    DisplayUser=new UserItemViewModel{ Initials = "L",ColorHex = "009bdb", },
                    SendDate = DateTimeOffset.Now,
                    IsSentByMe = false
                }
            };

            SetChatList(items);

            UserContext.ReplyMessage = new ReplyMessageViewModel
            {
                IsReplying = true,
            };

            UserContext = new UserChatContext(new UserItemViewModel());

            CanShowUnreadPopup = true;
            UserContext.User.SetUnreadMessages(7);
        }
    }
}
