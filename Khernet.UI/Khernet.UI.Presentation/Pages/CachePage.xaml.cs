namespace Khernet.UI.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
    /// </summary>
    public partial class CachePage : BasePage<CacheViewModel>
    {
        public CachePage()
        {
            InitializeComponent();
        }

        public CachePage(CacheViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
