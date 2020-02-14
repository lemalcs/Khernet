namespace Khernet.UI.IoC
{
    public interface IApplicationWindow
    {
        /// <summary>
        /// Shows windows if it is not visible
        /// </summary>
        void ShowWindow();

        /// <summary>
        /// Indicates if main window is visible or active
        /// </summary>
        bool IsMainWindowActive();
    }

    public interface IApplicationNotification
    {
        /// <summary>
        /// Shows a notification ballon
        /// </summary>
        /// <param name="notificationModel"></param>
        void ShowNotification(NotificationViewModel notificationModel);
    }
}
