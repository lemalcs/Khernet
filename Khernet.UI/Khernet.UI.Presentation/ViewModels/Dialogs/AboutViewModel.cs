using System.Diagnostics;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile
    /// </summary>
    public class AboutViewModel : BaseModel
    {

        #region Commands

        /// <summary>
        /// Clear the files located in cache directory
        /// </summary>
        public ICommand OpenIssueCommand { get; private set; }

        #endregion

        public AboutViewModel()
        {
            OpenIssueCommand = new RelayCommand(OpenIssue);
        }

        private void OpenIssue(object obj)
        {
            Process.Start("https://github.com/lemalcs/Khernet-issues/issues");
        }

    }
}
