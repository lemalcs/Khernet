namespace Khernet.UI
{
    /// <summary>
    /// Interface to implement update functionality.
    /// </summary>
    internal interface IUpdater
    {
        string CurrentVersion { get; }

        /// <summary>
        /// Checks if there is a new version of this applications.
        /// </summary>
        /// <returns>The new version.</returns>
        string CheckUpdate();

        /// <summary>
        /// Updates the application in automatic mode. Updates are downloaded from application site.
        /// </summary>
        /// <returns>The code that indicates the result of operation</returns>
        void Update();

        /// <summary>
        /// Updates the application in manual mode. Updates are provided by user.
        /// </summary>
        /// <param name="updateFilePath">The path of update file</param>
        void Update(string updateFilePath);
    }
}
