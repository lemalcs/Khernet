using Khernet.UI.Converters;

namespace Khernet.UI.IoC
{
    public interface IApplicationSurface
    {
        void ShowModalDialog(BaseModel viewModel);

        void ShowChildModalDialog(BaseModel viewModel);

        void ShowMessageBox(ModalDialogViewModel viewModel);

        void GoToPage(ApplicationPage page, BaseModel viewModel = null);

        void ShowPlayer(BaseModel viewModel);
    }
}
