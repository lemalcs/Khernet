using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
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
