using System.Security;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Login page when user starts application.
    /// </summary>
    public partial class LoginPage : BasePage<LoginViewModel>, IPasswordContainer
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public LoginPage(LoginViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public SecureString password
        {
            get
            {
                return pass1.SecurePassword;
            }
        }

        public SecureString secondPassword => null;

        public void Clear()
        {
            pass1.Clear();
        }

        private void Login_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((LoginViewModel)ViewModel).CanAutoLogin();
        }
    }
}
