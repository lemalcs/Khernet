namespace Khernet.UI.Pages
{
    /// <summary>
    /// Cache information page.
    /// </summary>
    public partial class AddContactPage : BasePage<AddContactViewModel>
    {
        public AddContactPage()
        {
            InitializeComponent();
        }

        public AddContactPage(AddContactViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnCommited();
        }
    }
}
