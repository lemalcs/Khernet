using System.Threading.Tasks;
using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// List of files interchanged with a peer.
    /// </summary>
    public partial class FileListPage : BasePage<FileListViewModel>
    {
        public FileListPage()
        {
            InitializeComponent();
        }

        public FileListPage(FileListViewModel model) : base(model)
        {
            model.Done = Close;
            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //Load files when this control finishes loading
            TaskEx.Delay(500).ContinueWith((t) => SpecificViewModel.LoadFiles(Media.MessageType.Binary));
        }

        private void Close()
        {
            OnCommited();
        }
    }
}
