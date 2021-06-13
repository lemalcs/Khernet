namespace Khernet.Installer.Launcher
{
    public interface IApplicationUpdater
    {
        /// <summary>
        /// Updates the application.
        /// </summary>
        /// <returns>The result of update.</returns>
        UpdateResult Update();
    }
}
