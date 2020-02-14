using Khernet.UI.IoC;
using System.Collections.Generic;

namespace Khernet.UI
{
    public class ResendViewModel : BaseModel
    {
        /// <summary>
        /// The current selected user
        /// </summary>
        private UserItemViewModel selectedUser;

        /// <summary>
        /// The list of users
        /// </summary>
        private List<UserItemViewModel> userList;

        /// <summary>
        /// The chat message to resend
        /// </summary>
        private ChatMessageItemViewModel message;

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

                    selectedUser = value;
                    selectedUser.IsSelected ^= true;

                    if (selectedUser.IsSelected)
                    {
                        IoCContainer.Get<UserListViewModel>().SelectUser(selectedUser.Token);
                    }

                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }

        public List<UserItemViewModel> Items
        {
            get => userList;
            set
            {
                if (userList != value)
                {
                    userList = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        public ChatMessageItemViewModel Message
        {
            get => message;
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }
    }
}
