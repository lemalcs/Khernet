using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Page to show when application is started
    /// </summary>
    public partial class LoadPage : BasePage
    {
        public LoadPage()
        {
            InitializeComponent();
        }

        private async void Login_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await LoadEnvironment();

            OpenInitialPage();

            CreateFirewallRules();
        }

        /// <summary>
        /// Build a validate the environment before start the application
        /// </summary>
        /// <returns>A task to accomplish the operations</returns>
        private Task LoadEnvironment()
        {
            return Task.Factory.StartNew(() => 
            {
                RegionFactory regionFactory = new RegionFactory();
                regionFactory.Build();
            });
        }

        /// <summary>
        /// Open the first page to user
        /// </summary>
        private void OpenInitialPage()
        {
            RegionFactory regionFactory = new RegionFactory();
            if (!regionFactory.IsInitialized())
                IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.SignUp);
            else
                IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
        }

        /// <summary>
        /// Creates if firewall rules for this application exists, otherwise try to create them
        /// </summary>
        private async void CreateFirewallRules()
        {
            try
            {
                if (!FirewallHelper.ExistsFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 6))
                {
                    FirewallHelper.CreateFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 6);
                }

                if (!FirewallHelper.ExistsFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 17))
                {
                    FirewallHelper.CreateFirewallRule(Configurations.MainApplicationAssembly, Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly), 17);
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);

                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Some configurations could not be made. Please restart de application as administrator.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

    }
}
