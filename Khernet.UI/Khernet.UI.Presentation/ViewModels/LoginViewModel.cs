using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using Khernet.UI.Resources;
using System;
using System.Security;
using System.Threading.Tasks;

namespace Khernet.UI
{
    public class LoginViewModel : BaseModel
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

        /// <summary>
        /// Indicates whether to remember the credentials for next the login attempt.
        /// </summary>
        private bool rememberCredentials;

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

        public bool RememberCredentials
        {
            get => rememberCredentials;
            set
            {
                if (rememberCredentials != value)
                {
                    rememberCredentials = value;
                    OnPropertyChanged(nameof(RememberCredentials));
                }
            }
        }

        #endregion


        /// <summary>
        /// Command for login.
        /// </summary>
        public RelayCommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            IsAttemptingLogin = false;
            LoginCommand = new RelayCommand(Login, VerifyCredentials);
        }

        /// <summary>
        /// Performs login for application.
        /// </summary>
        /// <param name="passwordInfo">The password for user.</param>
        public async void Login(object passwordInfo)
        {
            //Show progress animation
            IsAttemptingLogin = true;

            //Disable button
            CanLogin = false;

            var t = passwordInfo as IPasswordContainer;

            try
            {

                PeerIdentity peer = await AuthenticateUser(t.password).ConfigureAwait(true);
                t.Clear();

                if (peer != null)
                {
                    await Engine.Start(peer);

                    //Go to sggession page to start chat
                    IoCContainer.Get<ApplicationViewModel>().GoToPage(Converters.ApplicationPage.Session);
                }
                else
                {
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "incorrect user name or password",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    });
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                Engine.Stop();
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = Constants.ErrorMessage,
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
        /// Validates whether user-name and password are not empty.
        /// </summary>
        /// <returns>True if credentials are right otherwise false.</returns>
        public bool VerifyCredentials()
        {
            return !string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username);
        }

        /// <summary>
        /// Authenticate the current users to access application.
        /// </summary>
        /// <param name="password">The password if user.</param>
        /// <returns>A <see cref="Task"/> for authentication.</returns>
        private Task<PeerIdentity> AuthenticateUser(SecureString password)
        {
            TaskCompletionSource<PeerIdentity> result = new TaskCompletionSource<PeerIdentity>();

            var t = Task.Run(() =>
            {
                try
                {
                    var peer = Engine.AuthenticateUser(Username, password, RememberCredentials);

                    result.TrySetResult(peer);
                }
                catch (Exception)
                {
                    result.TrySetResult(null);
                }
            });

            return result.Task;
        }

        public void CanAutoLogin()
        {
            Authenticator authenticator = new Authenticator();
            Tuple<string, SecureString> credentials = authenticator.RetrieveCredentials();
            if (credentials != null && credentials.Item1 != null && credentials.Item2 != null)
            {
                RememberCredentials = true;
                Username = credentials.Item1;
            }
        }
    }
}
