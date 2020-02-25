namespace Khernet.UI.Pages
{
    /// <summary>
    /// List of settings for application.
    /// </summary>
    public partial class SettingsPage : BasePage<SettingControllerViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        public SettingsPage(SettingControllerViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
