using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// Dilaog to be shown in a secondary window.
    /// </summary>
    public partial class DialogWindow
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
