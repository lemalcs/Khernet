namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
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
