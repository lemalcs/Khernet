namespace Khernet.UI.Pages
{
    /// <summary>
    /// Cache information page.
    /// </summary>
    public partial class GeneralSettingsPage : BasePage<GeneralSettingsViewModel>
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        public GeneralSettingsPage(GeneralSettingsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
