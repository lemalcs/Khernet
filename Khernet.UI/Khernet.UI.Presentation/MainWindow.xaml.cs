using Khernet.UI.IoC;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// The main window of application.
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Prevent to close the application
            e.Cancel = true;

            HideWindow();
        }

        /// <summary>
        /// Hide main window
        /// </summary>
        private void HideWindow()
        {
            this.Hide();

            //Show session page when this window is closed
            IoCContainer.Get<ApplicationViewModel>().ClearChatPage();
        }

        private void MainWin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                HideWindow();
        }
    }
}
