using System.Threading.Tasks;

namespace Khernet.UI.IoC
{
    public interface IApplicationDialog
    {
        /// <summary>
        /// Shows a single message dialog
        /// </summary>
        /// <param name="dialogModel">View model for message dialog</param>
        /// <returns></returns>
        Task ShowMessageBox(MessageBoxViewModel dialogModel);

        /// <summary>
        /// Show an modal dialog
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowDialog<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Show system dialog to open files
        /// </summary>
        /// <returns></returns>
        string[] ShowOpenFileDialog();

        /// <summary>
        /// Showa system dialog to save file
        /// </summary>
        /// <returns></returns>
        string ShowSaveFileDialog(string fileName = null);

        /// <summary>
        /// Opens audio player into main window
        /// </summary>
        /// <typeparam name="T">The type view model</typeparam>
        /// <param name="viewModel">The view model for player</param>
        void ShowPlayer<T>(T viewModel) where T : BaseModel;
    }
}
