using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            TaskEx.Run(() => SpecificViewModel.LoadFiles(Media.MessageType.Audio));
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!((Control)sender).IsLoaded)
                return;

            double scrollDifference = Math.Abs(e.ExtentHeight - e.VerticalOffset - e.ViewportHeight);

            if (scrollDifference >= 0 && scrollDifference <= 1)
            {
                SpecificViewModel.LoadFiles(Media.MessageType.Audio);
            }
        }
    }
}
