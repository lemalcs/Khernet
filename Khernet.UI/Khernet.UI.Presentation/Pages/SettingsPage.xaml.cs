namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para SettingsListPage.xaml
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
