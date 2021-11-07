namespace Khernet.UI.IoC
{
    /// <summary>
    /// Interface to manage life cycle of the whole application.
    /// </summary>
    internal interface IApplicationManager
    {
        /// <summary>
        /// Shutdowns the application.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Hides the main window.
        /// </summary>
        void HideMainWindow();
    }
}
