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
    /// Lógica de interacción para ProfileViewerPage.xaml
    /// </summary>
    public partial class VideoListPage : BasePage<FileListViewModel>
    {
        public VideoListPage()
        {
            InitializeComponent();
        }

        public VideoListPage(FileListViewModel model) : base(model)
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
            TaskEx.Delay(500).ContinueWith((t) => SpecificViewModel.LoadFiles(Media.MessageType.Video));
        }

        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var videoModel = (VideoChatMessageViewModel)e.AddedItems[0];

                //If file exists on local system
                if (File.Exists(videoModel.FilePath))
                {
                    FileOperations oper = new FileOperations();

                    //Verify If file on local system is the same as the file requested
                    if (!oper.VerifyFileIntegrity(videoModel.FilePath, videoModel.FileSize, videoModel.Id))
                        //Download the video from application storage
                        videoModel.ProcessVideo(videoModel.Id);
                }
                else //Otherwise download the requested file from application storage
                    videoModel.ProcessVideo(videoModel.Id);

                //Play the video file within the application
                await IoCContainer.UI.ShowChildDialog(videoModel);

                //Set selected index to -1 so other file can be selected
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void ListBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mark this event as handled to avoid fire SelectionChanged event of listbox
            //Left button will open or load the file
            //Right button will show a contextual menu
            e.Handled = true;
        }
    }
}
