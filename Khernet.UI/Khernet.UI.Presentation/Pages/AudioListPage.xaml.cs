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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var audioModel = (AudioChatMessageViewModel)e.AddedItems[0];

                //If file exists on local system
                if (File.Exists(audioModel.FilePath))
                {
                    FileOperations oper = new FileOperations();

                    //Verify If file on local system is the same as the file requested
                    if (oper.VerifyFileIntegrity(audioModel.FilePath, audioModel.FileSize, audioModel.Id))
                    {
                        //Play the audio file within the application
                        IoCContainer.UI.ShowPlayer(audioModel);
                    }
                    else //Otherwise download the requested file from application storage
                        audioModel.ProcessAudio(audioModel.Id);
                }
                else //Otherwise download the requested file from application storage
                    audioModel.ProcessAudio(audioModel.Id);

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
