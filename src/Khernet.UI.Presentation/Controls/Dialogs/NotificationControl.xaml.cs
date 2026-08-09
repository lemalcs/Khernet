using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Notification when a new message arrives.
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
