namespace Khernet.UI.Pages
{
    /// <summary>
    /// Page to show when application is started.
    /// </summary>
    public partial class LoadPage : BasePage
    {
        private readonly LoadViewModel loadModel;

        public LoadPage()
        {
            InitializeComponent();

            loadModel = new LoadViewModel();
            loadModel.ShowProgress = true;
            loadModel.MessageText = "Loading...";

            DataContext = loadModel;
        }

        public LoadPage(LoadViewModel loadViewModel)
        {
            InitializeComponent();

            loadModel = loadViewModel;
            DataContext = loadModel;
        }
    }
}
