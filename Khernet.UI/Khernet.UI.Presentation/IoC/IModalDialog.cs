using Khernet.UI.Converters;

namespace Khernet.UI.IoC
{
    public interface IModalDialog
    {
        /// <summary>
        /// Close the dialog
        /// </summary>
        void Close();
    }

    public interface IPagedDialog
    {
        /// <summary>
        /// Close the dialog
        /// </summary>
        void GoToPage(ApplicationPage page, BaseModel viewModel, string title);

        void ShowChildDialog(BaseModel model);

        void CloseChildDialog();
    }
}
