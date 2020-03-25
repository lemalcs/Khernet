using System.Threading.Tasks;
using System.Windows;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// List of audio files interchanged with a peer.
    /// </summary>
    public partial class AudioListPage : BasePage<FileListViewModel>
    {
        public AudioListPage()
        {
            InitializeComponent();
        }

        public AudioListPage(FileListViewModel model) : base(model)
        {
            model.Done = Close;
            InitializeComponent();
        }

        private void Close()
        {
            OnCommited();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            //Load files when this control finishes loading
            TaskEx.Delay(500).ContinueWith((t) => SpecificViewModel.LoadFiles(Media.MessageType.Audio));
        }
    }
}
