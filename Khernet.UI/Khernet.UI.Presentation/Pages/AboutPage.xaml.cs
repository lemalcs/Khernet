namespace Khernet.UI.Pages
{
    /// <summary>
    /// About information page.
    /// </summary>
    public partial class AboutPage : BasePage<AboutViewModel>
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        public AboutPage(AboutViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
