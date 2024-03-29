﻿using System.Security;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Account creation page.
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

        public void Clear()
        {
            pass1.Clear();
            pass2.Clear();
        }
    }
}
