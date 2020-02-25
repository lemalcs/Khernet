namespace Khernet.UI.Pages
{
    /// <summary>
    /// Read only profile of peer.
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
