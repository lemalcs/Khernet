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

        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var imageModel = (ImageChatMessageViewModel)e.AddedItems[0];

                //If file exists on local system
                if (File.Exists(imageModel.FilePath))
                {
                    FileOperations oper = new FileOperations();

                    //Verify If file on local system is the same as the file requested
                    if (!oper.VerifyFileIntegrity(imageModel.FilePath, imageModel.FileSize, imageModel.Id))
                        //Download the requested file from application storage
                        imageModel.ProcessImage(imageModel.Id);
                }
                else //Otherwise download the requested file from application storage
                    imageModel.ProcessImage(imageModel.Id);

                //Open the image file within the application
                await IoCContainer.UI.ShowChildDialog(imageModel);

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

        private void Close()
        {
            OnCommited();
        }
    }
}
