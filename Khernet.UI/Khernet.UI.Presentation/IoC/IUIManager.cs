using System;
using System.Threading.Tasks;

namespace Khernet.UI.IoC
{
    public interface IUIManager
    {
        /// <summary>
        /// Shows a single message dialog.
        /// </summary>
        /// <param name="dialogModel">View model for message dialog.</param>
        /// <returns>A <see cref="Task"/> for dialog.</returns>
        Task ShowMessageBox(MessageBoxViewModel dialogModel);

        /// <summary>
        /// Shows a single message dialog into a specific parent window.
        /// </summary>
        /// <param name="dialogModel">View model for message dialog.</param>
        /// <param name="newWindow">True to show message in a new windows, false to show message into main window.</param>
        /// <returns>A <see cref="Task"/> for dialog.</returns>
        Task ShowMessageBox(MessageBoxViewModel dialogModel, bool newWindow);

        /// <summary>
        /// Show an modal dialog.
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A <see cref="Task"/> for dialog.</returns>
        Task ShowDialog<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Show an child modal dialog owned by parent modal dialog.
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A <see cref="Task"/> for dialog.</returns>
        Task ShowChildDialog<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Show system dialog to open files.
        /// </summary>
        /// <returns>The list of file paths.</returns>
        string[] ShowOpenFileDialog();

        /// <summary>
        /// Opens audio player into main window.
        /// </summary>
        /// <typeparam name="T">The type view model.</typeparam>
        /// <param name="viewModel">The view model for player.</param>
        void ShowPlayer<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Open a file with the default application.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        void OpenFile(string fileName);

        /// <summary>
        /// Open folder in file explorer with selected file.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        void OpenFolderForFile(string fileName);

        /// <summary>
        /// Indicates if it is design time.
        /// </summary>
        /// <returns>True if it is design time otherwise false.</returns>
        bool IsInDesignTime();

        /// <summary>
        /// Converts a <see cref="string"/> HTML document to XAML document byte array.
        /// </summary>
        /// <param name="value">The HTML document.</param>
        /// <returns>A <see cref="byte[]"/> array.</returns>
        byte[] ConvertHtmlToDocument(string html);

        /// <summary>
        /// Converts markdown text to XAML document byte array.
        /// </summary>
        /// <param name="markdownText">The text in markdown syntax.</param>
        /// <returns>A <see cref="byte"/> array containing a document for view.</returns>
        byte[] ConvertMarkdownToDocument(string markdownText);

        /// <summary>
        /// Shows a notification balloon.
        /// </summary>
        /// <param name="notificationModel">The model of notification.</param>
        void ShowNotification(NotificationViewModel notificationModel);

        /// <summary>
        /// Display unread chat messages.
        /// </summary>
        void ShowUnReadMessage(int idMessage);

        /// <summary>
        /// Indicates if main window is active.
        /// </summary>
        /// <returns>True if window has physical focused otherwise false.</returns>
        bool IsMainWindowActive();

        /// <summary>
        /// Shows windows if it is not visible.
        /// </summary>
        void ShowWindow();

        /// <summary>
        /// Execute a task asynchronously on user interface thread.
        /// </summary>
        void ExecuteAsync(Action action);

        /// <summary>
        /// Execute a task synchronously on user interface thread.
        /// </summary>
        void Execute(Action action);

        /// <summary>
        /// Shows the notification icon for application.
        /// </summary>
        void ShowNotificationIcon();

        /// <summary>
        /// Hide the new message indicator of icon located in notification area.
        /// </summary>
        void ClearNotificationNewMessageIcon();

        /// <summary>
        /// Shows the number of unread message on taskbar icon.
        /// </summary>
        /// <param name="unreadMessages">The number of unreadMessages.</param>
        void ShowUnreadMessagesNumber(int unreadMessages);

        /// <summary>
        /// Show a message indicating that application is minimized to tray area.
        /// </summary>
        void ShowTrayMessage();

        void ShowMainWindow();

        /// <summary>
        /// Exists the whole application.
        /// </summary>
        /// <param name="exitCode">The exit code that the application will return to caller.</param>
        void ShutDownApplication(int exitCode);
    }
}
