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
        /// Open save file dialog with the given file name.
        /// </summary>
        /// <param name="fileName">The name of file.</param>
        /// <returns>The full path of file.</returns>
        string ShowSaveFileDialog(string fileName = null);

        /// <summary>
        /// Shows system dialog to save file with the given extension.
        /// </summary>
        /// <param name="filter">The filter to show.</param>
        /// <param name="defaultExtension">Optional default extension</param>
        /// <returns>The full path of file.</returns>
        string ShowSaveFileDialog(string filter, string defaultExtension);


        /// <summary>
        /// Opens audio player into main window
        /// </summary>
        /// <typeparam name="T">The type view model</typeparam>
        /// <param name="viewModel">The view model for player</param>
        void ShowPlayer<T>(T viewModel) where T : BaseModel;
    }
}
