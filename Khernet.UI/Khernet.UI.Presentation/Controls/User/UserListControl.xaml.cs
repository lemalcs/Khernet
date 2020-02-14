using System;
using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para UserListControl.xaml
    /// </summary>
    public partial class UserListControl : UserControl
    {
        /// <summary>
        /// Fired when a user is selected from list
        /// </summary>
        public event EventHandler<SelectedUserEventArgs> SelectedUser;
        public UserListControl()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //Fire event if a user is selected
                UserItemViewModel item = e.AddedItems[0] as UserItemViewModel;
                OnSelecteduser(item.Token);
            }

            IoC.IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }

        /// <summary>
        /// Executes <see cref="SelectedUser"/> event
        /// </summary>
        /// <param name="userToken"></param>
        protected void OnSelecteduser(string userToken)
        {
            SelectedUser?.Invoke(this, new SelectedUserEventArgs(userToken));
        }
    }
}
