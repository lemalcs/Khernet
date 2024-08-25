using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Page to show when application is started.
    /// </summary>
    public partial class LoadPage : BasePage
    {
        private readonly LoadViewModel loadModel;

        public LoadPage()
        {
            InitializeComponent();

            loadModel = new LoadViewModel();
            loadModel.ShowProgress = true;
            loadModel.MessageText = "Loading...";

            DataContext = loadModel;
        }

        private async void Login_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                await LoadEnvironment();

                sbyte processResult = await CreateFirewallRules();

                if (processResult != 0)
                {
                    await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                    {
                        Message = "Some configurations could not be made. Please restart the application as administrator once.",
                        Title = "Khernet",
                        ShowAcceptOption = true,
                        AcceptOptionLabel = "OK",
                        ShowCancelOption = false,
                    }, true);
                }

                IoCContainer.UI.ShowNotificationIcon();

                OpenInitialPage();
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);

                loadModel.ShowProgress = false;
                loadModel.MessageText = string.Empty;

                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "There was an error while loading. There must be just one instance of application per user running, please close other instances and restart application.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                }, true);

                Application.Current.Shutdown(1);
            }
        }

        /// <summary>
        /// Build a validate the environment before start the application.
        /// </summary>
        /// <returns>A <see cref="Task"/> to accomplish the operations.</returns>
        private Task LoadEnvironment()
        {
            return Task.Factory.StartNew(() =>
            {
                RegionFactory regionFactory = new RegionFactory();
                regionFactory.Build();

                string ffmpegPath = Path.Combine(Configurations.AppDirectory, "media");
                if (!Directory.Exists(ffmpegPath))
                {
                    Directory.CreateDirectory(ffmpegPath);
                }

                if (Directory.GetFiles(ffmpegPath).Length == 0)
                    ExtractFile("ffmpeg.zip", ffmpegPath);
            });
        }

        private void ExtractFile(string file, string destinationPath)
        {

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Khernet.g.resources"))
            {
                using (System.Resources.ResourceReader r = new System.Resources.ResourceReader(stream))
                {
                    r.GetResourceData(file, out string resourceType, out byte[] resourceBytes);

                    using (MemoryStream mem = new MemoryStream(resourceBytes))
                    {
                        // File is located from 5 position in array `resourceBytes`
                        mem.Position = 4;

                        Compressor c = new Compressor();
                        c.UnZipFile(mem,
                            destinationPath
                            );

                    }
                }
            }
        }

        private class Credential : IPasswordContainer
        {
            public SecureString password { get; }
            public SecureString secondPassword { get; }
            public Credential(SecureString password)
            {
                this.password = password;
            }

            public void Clear()
            {
                password.Clear();
            }
        }

        /// <summary>
        /// Open the first page to user.
        /// </summary>
        private void OpenInitialPage()
        {
            RegionFactory regionFactory = new RegionFactory();
            if (!regionFactory.IsInitialized())
                IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.SignUp);
            else
            {
                Authenticator authenticator = new Authenticator();
                Tuple<string, SecureString> credentials = authenticator.RetrieveCredentials();

                if (credentials != null && credentials.Item1 != null && credentials.Item2 != null)
                {
                    LoginViewModel loginViewModel = new LoginViewModel();
                    loginViewModel.RememberCredentials = true;
                    loginViewModel.Username = credentials.Item1;
                    loginViewModel.Login(new Credential(credentials.Item2));
                }
                else
                {
                    IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
                }
            }
        }

        /// <summary>
        /// Check if firewall rules for this application exist, otherwise try to create them.
        /// </summary>
        private Task<sbyte> CreateFirewallRules()
        {
            TaskCompletionSource<sbyte> result = new TaskCompletionSource<sbyte>();

            Task.Run(() =>
            {
                try
                {
                    //Create rule for TCP protocol
                    if (!FirewallHelper.ExistsFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 6))
                    {
                        FirewallHelper.CreateFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 6);
                    }

                    //Create rule for UDP protocol
                    if (!FirewallHelper.ExistsFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 17))
                    {
                        FirewallHelper.CreateFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 17);
                    }

                    result.TrySetResult(0);
                }
                catch (Exception error)
                {
                    LogDumper.WriteLog(error, $"Could not create firewall rules due to: {error.Message}");
                    result.TrySetResult(-1);
                }
            });

            return result.Task;
        }

    }
}
