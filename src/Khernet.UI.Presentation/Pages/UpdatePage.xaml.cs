namespace Khernet.UI.Pages
{
    /// <summary>
    /// Manage updates page.
    /// </summary>
    public partial class UpdatePage : BasePage<UpdateViewModel>
    {
        public UpdatePage()
        {
            InitializeComponent();
        }

        public UpdatePage(UpdateViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
