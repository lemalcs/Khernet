using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {

        public NotificationControl()
        {
            InitializeComponent();
        }

        public NotificationControl(NotificationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
