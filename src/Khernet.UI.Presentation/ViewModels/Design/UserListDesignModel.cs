using System.Collections.ObjectModel;

namespace Khernet.UI.ViewModels
{
    public class UserListDesignModel : UserListViewModel
    {
        public static UserListDesignModel Instance => new UserListDesignModel();

        public UserListDesignModel()
        {
            var items = new ObservableCollection<UserItemViewModel>
            {
                new UserItemViewModel
                {
                    Initials="C",
                    ColorHex="ba007c"
                },
                new UserItemViewModel
                {
                    Initials="M",
                    ColorHex="db9600"
                },
                new UserItemViewModel
                {
                    Initials="S",
                    ColorHex="8165a2"
                },
                 new UserItemViewModel
                {
                    Initials="C",
                    ColorHex="ba007c"
                },
                new UserItemViewModel
                {
                    Initials="M",
                    ColorHex="db9600"
                },
                new UserItemViewModel
                {
                    Initials="S",
                    ColorHex="8165a2"
                },
                 new UserItemViewModel
                {
                    Initials="C",
                    ColorHex="ba007c"
                },
                new UserItemViewModel
                {
                    Initials="M",
                    ColorHex="db9600"
                },
                new UserItemViewModel
                {
                    Initials="S",
                    ColorHex="8165a2"
                }
            };

            SetUserList(items);
        }
    }
}
