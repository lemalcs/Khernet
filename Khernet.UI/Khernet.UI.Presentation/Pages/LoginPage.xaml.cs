﻿using System.Security;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
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
    }
}