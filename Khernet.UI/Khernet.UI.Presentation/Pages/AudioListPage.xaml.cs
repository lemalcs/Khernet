using Khernet.UI.Files;
using Khernet.UI.IoC;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Khernet.UI.Pages
{
    /// <summary>
    /// Read only profile page.
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
