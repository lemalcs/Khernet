using Khernet.UI.Cache;
using System.Diagnostics;
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
        /// Opens the source code page on browser.
        /// </summary>
        public ICommand OpenSourceCodeCommand { get; private set; }

        /// <summary>
        /// Opens the directory where this application is installed. 
        /// </summary>
        public ICommand OpenInstallationPathCommand { get; private set; }

        #endregion

        public AboutViewModel()
        {
            OpenIssueCommand = new RelayCommand(OpenIssue);
            OpenSourceCodeCommand = new RelayCommand(OpenSourceCode);
            OpenInstallationPathCommand = new RelayCommand(OpenInstallationPath);
        }

        private void OpenSourceCode()
        {
            Process.Start("https://github.com/lemalcs/Khernet");
        }

        private void OpenIssue()
        {
            Process.Start("https://github.com/lemalcs/Khernet-issues/issues");
        }

        private void OpenInstallationPath()
        {
            Process.Start(Configurations.HomeDirectory);

        }

    }
}
