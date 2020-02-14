using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para ReplyMessageControl.xaml
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
