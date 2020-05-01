using Khernet.UI.IoC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Khernet.UI
{
    public class UserListViewModel : BaseModel
    {
        /// <summary>
        /// The list of users
        /// </summary>
        private ObservableCollection<UserItemViewModel> items;

        /// <summary>
        /// Tyhe current selected user
        /// </summary>
        private UserItemViewModel selectedUser;

        public UserItemViewModel SelectedUser
        {
            get => selectedUser;
            set
            {
                if (selectedUser != value)
                {
                    if (selectedUser != null)
                        selectedUser.IsSelected ^= true;

                    selectedUser = value;

                    if (selectedUser != null)
                        selectedUser.IsSelected ^= true;

                    if (selectedUser != null && selectedUser.IsSelected)
                        selectedUser.OpenChatCommand.Execute(null);

                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }

        /// <summary>
        /// The list of users
        /// </summary>
        public ObservableCollection<UserItemViewModel> Items
        {
            get => items;
            private set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        /// <summary>
        /// The number of unread messages from all user in the list
        /// </summary>
        public int TotalUnreadMessages
        {
            get;
            private set;
        }

        /// <summary>
        /// Find and user based on token
        /// </summary>
        /// <param name="token">The token of user</param>
        /// <returns>Instance of <see cref="UserItemViewModel"/></returns>
        public UserItemViewModel FindUser(string token)
        {
            UserItemViewModel userItem = null;

            //Update user list into user interface thread context
            IoCContainer.UI.Execute(() =>
            {
                if (Items != null)
                {
                    userItem = Items.FirstOrDefault((user) => user.Token == token);
                }
            });

            return userItem;
        }

        /// <summary>
        /// Sets user as selected
        /// </summary>
        /// <param name="token">The token of user</param>
        public void SelectUser(string token)
        {
            if (Items == null)
                return;

            var user = Items.FirstOrDefault((us) => us.Token == token);
            if (user != null)
                SelectedUser = user;
        }

        public void ClearSelection()
        {
            SelectedUser = null;
            IoCContainer.Get<ChatMessageListViewModel>().SaveDraftMessage();
            IoCContainer.Get<ChatMessageListViewModel>().UserContext = null;
            IoCContainer.Get<ApplicationViewModel>().GoToPage(Converters.ApplicationPage.Session);
        }

        /// <summary>
        /// Clears list of users
        /// </summary>
        public void Clear()
        {
            if (Items != null)
                Items.Clear();
        }

        /// <summary>
        /// Gets a copy of user list
        /// </summary>
        /// <returns>A list of <see cref="UserItemViewModel"/> objects</returns>
        public List<UserItemViewModel> Clone()
        {
            List<UserItemViewModel> list = new List<UserItemViewModel>();

            var users = Items;

            for (int i = 0; i < users.Count; i++)
            {
                list.Add(users[i].Clone());
            }

            return list;
        }

        public void SetUserList(IEnumerable<UserItemViewModel> userList)
        {
            Items = (ObservableCollection<UserItemViewModel>)userList;
        }

        public void AddUser(UserItemViewModel userInfo)
        {
            //Update user list into user interface thread context
            IoCContainer.UI.Execute(() =>
            {
                if (Items == null)
                    Items = new ObservableCollection<UserItemViewModel>();

                if (Items.FirstOrDefault((u) => u.Token == userInfo.Token) != null)
                    return;

                Items.Add(userInfo);
            });
        }

        public void AddUnreadMessages(int unreadMessageNumber)
        {
            if (unreadMessageNumber <= 0)
                return;

            TotalUnreadMessages += unreadMessageNumber;
        }

        public void SubtractUnreadMessages(int unreadMessageNumber)
        {
            if (unreadMessageNumber <= 0)
                return;

            TotalUnreadMessages -= unreadMessageNumber;
            ClearNewMessageNotificationIcon();
        }

        public void ClearUnreadMessages()
        {
            TotalUnreadMessages = 0;
            ClearNewMessageNotificationIcon();
        }

        private void ClearNewMessageNotificationIcon()
        {
            if (TotalUnreadMessages == 0)
                IoCContainer.UI.ClearNotificationNewMessageIcon();
        }
    }
}
