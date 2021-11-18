namespace Khernet.UI.Pages
{
    /// <summary>
    /// Cache information page.
    /// </summary>
    public partial class ConnectionPage : BasePage<ConnectionViewModel>
    {
        public ConnectionPage()
        {
            InitializeComponent();
        }

        public ConnectionPage(ConnectionViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
