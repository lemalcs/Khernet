using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for about information.
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
        /// Opens the directory where this installer (the executable that installs the whole application) is located. 
        /// </summary>
        public ICommand OpenInstallationPathCommand { get; private set; }

        /// <summary>
        /// Opens the working directory where this application is located. 
        /// </summary>
        public ICommand OpenWorkingDirectoryCommand { get; private set; }

        #endregion

        public AboutViewModel()
        {
            OpenIssueCommand = new RelayCommand(OpenIssue);
            OpenHomePageCommand = new RelayCommand(OpenHomePage);
            OpenInstallationPathCommand = new RelayCommand(OpenInstallationPath);
            OpenWorkingDirectoryCommand = new RelayCommand(OpenWorkingDirectory);
        }

        private void OpenWorkingDirectory()
        {
            Process.Start(Configurations.HomeDirectory);
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
            DirectoryInfo dirInfo = new DirectoryInfo(Configurations.HomeDirectory);
            string installerPath = dirInfo.Parent.Parent.FullName;
            if (Directory.Exists(installerPath))
                Process.Start(installerPath);
            else
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = $"Installer not found, place the installer at the following path and try again: {installerPath}",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
        }


    }
}
