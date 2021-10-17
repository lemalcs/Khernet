using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            Task.Run(() => SpecificViewModel.LoadFiles(Media.MessageType.Binary));
        }

        private void Close()
        {
            OnCommited();
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!((Control)sender).IsLoaded)
                return;

            double scrollDifference = Math.Abs(e.ExtentHeight - e.VerticalOffset - e.ViewportHeight);

            if (scrollDifference >= 0 && scrollDifference <= 1)
            {
                SpecificViewModel.LoadFiles(Media.MessageType.Binary);
            }
        }
    }
}
