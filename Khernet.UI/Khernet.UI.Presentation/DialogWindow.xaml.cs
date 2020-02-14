using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class DialogWindow //: MetroWindow
    {
        /// <summary>
        /// View model for dialog window
        /// </summary>
        private DialogWindowViewModel dialogVM;

        public DialogWindow()
        {
            InitializeComponent();
        }

        public DialogWindowViewModel DialogVM
        {
            get
            {
                return dialogVM;
            }

            set
            {
                dialogVM = value;
                DataContext = dialogVM;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
