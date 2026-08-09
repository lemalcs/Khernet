using Khernet.UI.Converters;

namespace Khernet.UI.IoC
{
    public interface IModalDialog
    {
        /// <summary>
        /// Closes the dialog.
        /// </summary>
        void Close();
    }

    public interface IPagedDialog
    {
        /// <summary>
        /// Closes the dialog.
        /// </summary>
        void GoToPage(ApplicationPage page, BaseModel viewModel, string title);

        void ShowChildDialog(BaseModel model);

        void CloseChildDialog();
    }
}
