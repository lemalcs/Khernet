namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para ProfileViewerPage.xaml
    /// </summary>
    public partial class ProfileViewerPage : BasePage<ProfileViewModel>
    {
        public ProfileViewerPage()
        {
            InitializeComponent();
        }

        public ProfileViewerPage(ProfileViewModel model) : base(model)
        {
            InitializeComponent();
        }
    }
}
