using System.Security;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
    /// </summary>
    public partial class SignUpPage : BasePage<SignUpViewModel>, IPasswordContainer
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        public SignUpPage(SignUpViewModel viewModel) : base(viewModel)
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

        public SecureString secondPassword
        {
            get
            {
                return pass2.SecurePassword;
            }
        }
    }
}
