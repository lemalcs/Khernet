using System.Threading.Tasks;
using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// List of images interchanged with a peer.
    /// </summary>
    public partial class ImageListPage : BasePage<FileListViewModel>
    {
        public ImageListPage()
        {
            InitializeComponent();
        }

        public ImageListPage(FileListViewModel model) : base(model)
        {
            model.Done = Close;
            InitializeComponent();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //Load files when this control finishes loading
            TaskEx.Delay(500).ContinueWith((t) => SpecificViewModel.LoadFiles(Media.MessageType.Image));
        }

        private void Close()
        {
            OnCommited();
        }
    }
}
