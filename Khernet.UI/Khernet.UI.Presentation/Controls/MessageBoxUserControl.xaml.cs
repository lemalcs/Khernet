using System.Windows;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para MessageBoxUserControl.xaml
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
