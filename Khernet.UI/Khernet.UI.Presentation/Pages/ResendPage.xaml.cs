using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// List of peers to resend a message.
    /// </summary>
    public partial class ResendPage : BasePage<ResendViewModel>
    {
        public ResendPage()
        {
            InitializeComponent();
        }

        public ResendPage(ResendViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        private void UserListControl_SelectedUser(object sender, Controls.SelectedUserEventArgs e)
        {
            OnCommited();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnCommited();
        }
    }
}
