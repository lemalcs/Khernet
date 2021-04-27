using Khernet.Installer.Launcher.Logger;
using Squirrel;
using System;

namespace Khernet.Installer.Launcher
{
    public enum UpdateResult
    {
        /// <summary>
        /// Update was downloaded successfully.
        /// </summary>
        Success,

        /// <summary>
        /// No updates available.
        /// </summary>
        NotUpdates,

        /// <summary>
        /// There was an error while updating.
        /// </summary>
        Fail,
    }

    /// <summary>
    /// Updates an application using Squirrel framework.
    /// </summary>
    public class SquirrelApplicationsUpdater : IApplicationUpdater
    {
        /// <summary>
        /// The list of sources to update from.
        /// </summary>
        readonly string[] updateSources;

        readonly ILogger logger;

        public SquirrelApplicationsUpdater(string[] updateSources, ILogger logger)
        {
            if (updateSources == null)
                throw new ArgumentNullException($"Parameter {nameof(updateSources)} cannot be null");

            if (logger == null)
                throw new ArgumentNullException($"Parameter {nameof(logger)} cannot be null");

            this.updateSources = updateSources;
            this.logger = logger;
        }

        /// <summary>
        /// Updates the application using the sources provided by this class.
        /// </summary>
        /// <returns> <see cref="UpdateResult"/> with the final result of update.</returns>
        public UpdateResult Update()
        {
            if (updateSources.Length == 0)
                return UpdateResult.NotUpdates;

            for (int i = 0; i < updateSources.Length; i++)
            {
                if (updateSources[i] == null)
                    continue;

                try
                {
                    using (var mgr = new UpdateManager(updateSources[i]))
                    {
                        var result = mgr.CheckForUpdate().Result;
                        if (result.ReleasesToApply.Count == 0)
                            return UpdateResult.NotUpdates;

                        mgr.UpdateApp().Wait();
                    }
                    return UpdateResult.Success;
                }
                catch (Exception error)
                {
                    logger.Log($"Update source: {updateSources[i]}", error);
                }
            }

            return UpdateResult.NotUpdates;
        }
    }
}
