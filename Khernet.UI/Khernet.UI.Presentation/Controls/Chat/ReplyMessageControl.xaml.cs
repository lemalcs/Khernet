using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// A summary of message that was replied.
    /// </summary>
    public partial class ReplyMessageControl : UserControl
    {
        public ReplyMessageControl()
        {
            InitializeComponent();
        }

        public ReplyMessageControl(BaseModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

        }
    }
}
