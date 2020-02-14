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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var fileModel = (FileChatMessageViewModel)e.AddedItems[0];

                //If file exists on local system
                if (File.Exists(fileModel.FilePath))
                {
                    FileOperations oper = new FileOperations();

                    //Verify If file on local system is the same as the file requested
                    if (oper.VerifyFileIntegrity(fileModel.FilePath, fileModel.FileSize, fileModel.Id))
                    {
                        //Open file with configured application
                        IoCContainer.UI.OpenFile(fileModel.FilePath);
                    }
                    else //Otherwise download the requested file from application storage
                    {
                        fileModel.IsFileLoaded = false;
                        fileModel.ProcessFile(fileModel.Id);
                    }
                }
                else //Otherwise download the requested file from application storage
                    fileModel.ProcessFile(fileModel.Id);

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
