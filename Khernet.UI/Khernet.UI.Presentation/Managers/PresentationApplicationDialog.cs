﻿using Khernet.UI.Controls;
using Khernet.UI.IoC;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace Khernet.UI.Managers
{
    public class PresentationApplicationDialog : IApplicationDialog
    {
        public async Task ShowChildDialog<T>(T viewModel) where T : BaseModel
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
                await new VideoViewerControl().ShowChildModalDialog(viewModel, true);
            }
        }

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
                await new VideoViewerControl().ShowModalDialog(viewModel, true);
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
            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }
    }
}