using System.Windows;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Dialog to show short messages
    /// </summary>
    public partial class MessageBoxUserControl : BaseDialogUserControl
    {
        public MessageBoxUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            base.CloseCommand.Execute(null);
        }
    }
}
