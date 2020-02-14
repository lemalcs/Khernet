using Khernet.UI.Converters;
using Khernet.UI.IoC;
using System;

namespace Khernet.UI.Managers
{
    public class PresentationApplicationSurface : IApplicationSurface
    {
        public void GoToPage(ApplicationPage page, BaseModel viewModel = null)
        {
            throw new NotImplementedException();
        }

        public void ShowChildModalDialog(BaseModel viewModel)
        {
            throw new NotImplementedException();
        }

        public void ShowMessageBox(ModalDialogViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public void ShowModalDialog(BaseModel viewModel)
        {
            throw new NotImplementedException();
        }

        public void ShowPlayer(BaseModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
