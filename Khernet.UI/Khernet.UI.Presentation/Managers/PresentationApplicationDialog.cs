using Khernet.UI.Controls;
using Khernet.UI.IoC;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace Khernet.UI.Managers
{
    public class PresentationApplicationDialog : IApplicationDialog
    {
        public async Task ShowDialog<T>(T viewModel) where T : BaseModel
        {
            if (viewModel is PagedDialogViewModel)
            {
                await new PagedDialogControl().ShowModalDialog(viewModel);
            }
            else if (viewModel is ImageChatMessageViewModel)
            {
                await new ImageViewerControl().ShowModalDialog(viewModel, true);
            }
            else if (viewModel is VideoChatMessageViewModel)
            {
                await new VideoPlayerControl().ShowModalDialog(viewModel, true);
            }
            else if (viewModel is SaveFileDialogViewModel)
            {
                await new SaveFileDialogControl().ShowModalDialog(viewModel);
            }
        }

        public async Task ShowMessageBox(MessageBoxViewModel dialogModel)
        {
            await new MessageBoxUserControl().ShowMessageBox(dialogModel);
        }

        public string[] ShowOpenFileDialog()
        {
            //Show open file dialog 
            OpenFileDialog op = new OpenFileDialog();
            bool? result = op.ShowDialog();

            //If user clicks OK ShowDialog() returns true
            //otherwise returns false
            if (!result.Value)
                return null;

            //Return all files selected by user
            return op.FileNames;
        }

        public void ShowPlayer<T>(T viewModel) where T : BaseModel
        {
            IoCContainer.Get<ApplicationViewModel>().ShowPlayer(viewModel);
        }

        /// <summary>
        /// Open save file dialog with the given file name.
        /// </summary>
        /// <param name="fileName">The name of file.</param>
        /// <returns>The full path of file.</returns>
        public string ShowSaveFileDialog(string fileName = null)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = fileName;
            saveDialog.Filter = "All files |*.*";

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Shows system dialog to save file with the given extension.
        /// </summary>
        /// <param name="filter">The filter to show.</param>
        /// <param name="defaultExtension">Optional default extension.</param>
        /// <returns>The full path of file.</returns>
        public string ShowSaveFileDialog(string filter, string defaultExtension)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = filter;
            saveDialog.DefaultExt = defaultExtension;
            saveDialog.AddExtension = false;

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }
    }

    public class ModalApplicationDialog : IApplicationDialog
    {
        public async Task ShowDialog<T>(T viewModel) where T : BaseModel
        {
            if (viewModel is PagedDialogViewModel)
            {
                await new PagedDialogControl().ShowChildModalDialog(viewModel);
            }
            else if (viewModel is ImageChatMessageViewModel)
            {
                await new ImageViewerControl().ShowChildModalDialog(viewModel, true);
            }
            else if (viewModel is VideoChatMessageViewModel)
            {
                await new VideoPlayerControl().ShowChildModalDialog(viewModel, true);
            }
            else if (viewModel is SaveFileDialogViewModel)
            {
                await new SaveFileDialogControl().ShowChildModalDialog(viewModel);
            }
        }

        public async Task ShowMessageBox(MessageBoxViewModel dialogModel)
        {
            await new MessageBoxUserControl().ShowMessageBox(dialogModel);
        }

        public string[] ShowOpenFileDialog()
        {
            //Show open file dialog 
            OpenFileDialog op = new OpenFileDialog();
            bool? result = op.ShowDialog();

            //If user clicks OK ShowDialog() returns true
            //otherwise returns false
            if (!result.Value)
                return null;

            //Return all files selected by user
            return op.FileNames;
        }

        public void ShowPlayer<T>(T viewModel) where T : BaseModel
        {
            IoCContainer.Get<ApplicationViewModel>().ShowPlayer(viewModel);
        }

        public string ShowSaveFileDialog(string fileName = null)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = fileName;
            saveDialog.Filter = "All files |*.*";

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        public string ShowSaveFileDialog(string filter, string defaultExtension)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = filter;
            saveDialog.DefaultExt = defaultExtension;
            saveDialog.AddExtension = false;

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }
    }
}
