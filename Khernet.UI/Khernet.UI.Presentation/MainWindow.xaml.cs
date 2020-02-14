using Khernet.UI.IoC;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

            IoCContainer.Get<ApplicationViewModel>().ClearChatPage();
        }

        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Hide();
        }
    }
}
