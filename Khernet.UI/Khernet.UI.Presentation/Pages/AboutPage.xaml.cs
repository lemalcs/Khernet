namespace Khernet.UI.Pages
{
    /// <summary>
    /// Listo of audio files interchanged with peer.
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
