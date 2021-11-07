using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model to show information about this application.
    /// </summary>
    public class AboutViewModel : BaseModel
    {

        #region Commands

        /// <summary>
        /// Opens the issue page on browser.
        /// </summary>
        public ICommand OpenIssueCommand { get; private set; }

        /// <summary>
        /// Opens the home page on browser.
        /// </summary>
        public ICommand OpenHomePageCommand { get; private set; }

        /// <summary>
        /// Opens the directory where the main executable (Khernet.exe) is located.
        /// </summary>
        public ICommand OpenInstallationPathCommand { get; private set; }

        #endregion

        public AboutViewModel()
        {
            OpenIssueCommand = new RelayCommand(OpenIssue);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);
            OpenInstallationPathCommand = new RelayCommand(OpenInstallationPath);
        }

        private void OpenHomePage()
        {
            Process.Start("https://khernet.app");
        }

        private void OpenIssue()
        {
            Process.Start("https://github.com/lemalcs/Khernet/issues");
        }

        private async void OpenInstallationPath()
        {
            if (Directory.Exists(Configurations.HomeDirectory))
                Process.Start(Configurations.HomeDirectory);
            else
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = $"Error while opening path, please try again: {Configurations.HomeDirectory}",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
        }


    }
}
