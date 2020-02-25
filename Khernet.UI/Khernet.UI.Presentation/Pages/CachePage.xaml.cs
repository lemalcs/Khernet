namespace Khernet.UI.Pages
{
    /// <summary>
    /// Cache information page.
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
