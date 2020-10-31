using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;

namespace Khernet.UI
{
    public class SignUpViewModel : BaseModel, IDataErrorInfo
    {
        #region Properties

        /// <summary>
        /// Indicates if there is an attempt to login.
        /// </summary>
        private bool isAttemptingLogin;

        /// <summary>
        /// The User for login.
        /// </summary>
        private string username;

        /// <summary>
        /// The password for login.
        /// </summary>
        private SecureString password;

        /// <summary>
        /// Indicates if login command can be executed.
        /// </summary>
        private bool canLogin;

        public bool IsAttemptingLogin
        {
            get { return isAttemptingLogin; }
            set
            {
                if (isAttemptingLogin != value)
                {
                    isAttemptingLogin = value;
                    OnPropertyChanged(nameof(IsAttemptingLogin));
                }
            }
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                if (username != value)
                {
                    username = value;
                    CanLogin = true;
                    OnPropertyChanged(nameof(Username));
                }

            }
        }

        public SecureString Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool CanLogin
        {
            get
            {
                return canLogin;
            }

            set
            {
                if (canLogin != value)
                {
                    canLogin = value;
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }

        #endregion

        /// <summary>
        /// Command for create a new user.
        /// </summary>
        public RelayCommand SignUpCommand { get; private set; }

        public SignUpViewModel()
        {
            IsAttemptingLogin = false;
            SignUpCommand = new RelayCommand(SignUp, VerifyCredentials);
        }

        /// <summary>
        /// Performs login for application.
        /// </summary>
        /// <param name="parameter">The password for user.</param>
        public async void SignUp(object parameter)
        {
            //Show progress animation
            IsAttemptingLogin = true;

            //Disable button
            CanLogin = false;

            var t = parameter as IPasswordContainer;

            try
            {

                bool isValid = await ValidatePasswords(t.password, t.secondPassword).ConfigureAwait(true);

                if (!isValid)
                    return;

                PeerIdentity peer = await CreateUser(t.password).ConfigureAwait(true);
                t.Clear();

                if (peer != null)
                {
                    await Engine.Start(peer);

                    //Go to session page to start chat
                    IoCContainer.Get<ApplicationViewModel>().GoToPage(Converters.ApplicationPage.Session);
                }

                else
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "User name or password incorrect",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    });
            }
            catch (Exception)
            {
                Engine.Stop();
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while trying to create account",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
            finally
            {
                //Hide progress animation 
                IsAttemptingLogin = false;

                //Enable button
                CanLogin = true;

                if (t != null)
                    t.Clear();
            }

        }

        /// <summary>
        /// Verifies if password match the policies.
        /// </summary>
        /// <param name="firstPassword">The password.</param>
        /// <param name="secondPassword">The confirm password.</param>
        private async Task<bool> ValidatePasswords(SecureString firstPassword, SecureString secondPassword)
        {

            if (!FieldValidator.ValidatePassword(firstPassword) || !FieldValidator.ValidatePassword(secondPassword))
            {
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Passwords must be at least 10 characters and contain letters, numbers, special characters.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
                return false;
            }
            if (!FieldValidator.ComparePasswords(firstPassword, secondPassword))
            {
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Passwords must be the same.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates whether user-name and password are not empty.
        /// </summary>
        /// <param name="parameter">The parameter for command.</param>
        /// <returns>True if user-name is valid otherwise false.</returns>
        public bool VerifyCredentials(object parameter)
        {
            return FieldValidator.ValidateUserName(Username);
        }

        /// <summary>
        /// Creates an new user for application.
        /// </summary>
        /// <param name="password">The password of user.</param>
        /// <returns>The identity of user <see cref="PeerIdentity"/>.</returns>
        private Task<PeerIdentity> CreateUser(SecureString password)
        {
            TaskCompletionSource<PeerIdentity> result = new TaskCompletionSource<PeerIdentity>();

            var t = TaskEx.Run(() =>
             {
                 //Create user
                 Authenticator auth = new Authenticator();
                 auth.CreateUser(Username, password);

                 //Authenticate created user
                 PeerIdentity peer = auth.Login(Username, password);

                 //return user identity
                 result.TrySetResult(peer);
             });

            return result.Task;
        }

        public string Error { get { return string.Empty; } }

        public string this[string columnName]
        {
            get
            {
                //Validate user-name
                if (columnName == nameof(Username))
                {
                    if (string.IsNullOrEmpty(Username))
                        return string.Empty;

                    if (!FieldValidator.ValidateUserName(Username))
                    {
                        return "Username must be less or equal to 20 characters, contain alphanumeric characters and underscore only";
                    }
                }
                return string.Empty;
            }
        }
    }
}
