using AutoUpdaterDotNET;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Khernet.UI
{
    // Manages updates for this application.
    internal class ApplicationUpdater : IUpdater
    {
        /// <summary>
        /// The current version of application with format {MAJOR}.{MINOR}.{BUILD}.{REVISION}.
        /// </summary>
        private string currentVersion;

        /// <summary>
        /// Contains the exception raised when trying to update.
        /// </summary>
        private Exception error;

        /// <summary>
        /// The url to get updates files from.
        /// </summary>
#if DEBUG
        private const string UPDATE_URL = @"C:\UpdateServer\KhernetUpdater.xml";
#else
        private const string UPDATE_URL = "http://khernet.app/KhernetUpdater.xml";
#endif


        public string CurrentVersion
        {
            get => currentVersion;
            private set
            {
                currentVersion = value;
            }
        }

        /// <summary>
        /// Checks if there is a new version of this applications.
        /// </summary>
        /// <returns>The new version.</returns>
        public string CheckUpdate()
        {
            AutoUpdater.Synchronous = true;
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
            AutoUpdater.Start(UPDATE_URL);
            AutoUpdater.CheckForUpdateEvent -= AutoUpdater_CheckForUpdateEvent;
            return CurrentVersion;
        }

        /// <summary>
        /// Updates the application from online source. Updates are downloaded from application site.
        /// </summary>
        /// <returns>The code that indicates the result of operation</returns>
        public void Update()
        {
            UpdateApp(true, null);
        }

        /// <summary>
        /// Event handler to check details of new version if available.
        /// </summary>
        /// <param name="args">The details of version.</param>
        private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error != null)
            {
                error = args.Error;
                LogDumper.WriteLog(args.Error);
                return;
            }

            if (args.IsUpdateAvailable)
            {
                CurrentVersion = args.CurrentVersion;
            }
        }

        /// <summary>
        /// Manages the update process when manual mode is enabled.
        /// </summary>
        /// <param name="args">The information about the update</param>
        private void AutoUpdater_CheckForUpdateEvent_Manual(UpdateInfoEventArgs args)
        {
            if (args.Error != null)
            {
                error = args.Error;
                LogDumper.WriteLog(args.Error);
                return;
            }

            if (args.IsUpdateAvailable)
            {
                CurrentVersion = args.CurrentVersion;
                try
                {
                    if (AutoUpdater.DownloadUpdate(args))
                    {
                        IoCContainer.Get<IApplicationManager>().Shutdown();
                    }
                    else
                    {
                        error = new Exception("Updates couldn't be downloaded");
                    }
                }
                catch (System.Exception error)
                {
                    LogDumper.WriteLog(error);
                }
            }
        }

        /// <summary>
        /// Updates the application in manual mode. Updates are provided by user.
        /// </summary>
        /// <param name="updateFilePath">The path of update file</param>
        public void Update(string updateFilePath)
        {
            UpdateApp(false, updateFilePath);
        }

        private void UpdateApp(bool getUpdatesOnline, string updateFilePath)
        {
            error = null;

            if (!getUpdatesOnline && !File.Exists(updateFilePath))
                throw new FileNotFoundException($"File not exists: {updateFilePath}");

            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.Synchronous = true;

            AutoUpdater.AppTitle = "Khernet";

            if (!Directory.Exists(Configurations.CacheDirectory.FullName))
            {
                Directory.CreateDirectory(Configurations.CacheDirectory.FullName);
            }

            AutoUpdater.DownloadPath = Configurations.CacheDirectory.FullName;
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent_Manual;

            if (getUpdatesOnline)
            {
                AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
                AutoUpdater.Start(UPDATE_URL);
                AutoUpdater.ApplicationExitEvent -= AutoUpdater_ApplicationExitEvent;
            }
            else
            {
                if (!getUpdatesOnline && File.Exists(updateFilePath))
                {
                    AutoUpdater.Start(CreateUpdateFile(updateFilePath));
                }
            }
            AutoUpdater.CheckForUpdateEvent -= AutoUpdater_CheckForUpdateEvent_Manual;

            if (error != null)
            {
                throw error;
            }
        }

        /// <summary>
        /// Shutdowns the application to update it. This event is fired when CheckForUpdateEvent was handled by code.
        /// </summary>
        private void AutoUpdater_ApplicationExitEvent()
        {
            IoCContainer.Get<IApplicationManager>().Shutdown();
        }

        /// <summary>
        /// Creates a XML file containing the update information. Used for manual update.
        /// </summary>
        /// <param name="updateFilePath">The path of file that contains the actual update files.</param>
        /// <returns>The path of XML file.</returns>
        private string CreateUpdateFile(string updateFilePath)
        {
            string xmlFile = Path.Combine(Configurations.AppDirectory, "Update.xml");
            XmlWriter xmlWriter = new XmlTextWriter(xmlFile, Encoding.UTF8);
            xmlWriter.WriteStartElement("item");

            // Create a artificial new version to enable updater to fire a new installation
            Version version = new Version(Configurations.AppVerion);
            xmlWriter.WriteElementString("version", new Version(version.Major, version.Minor, version.Build, version.Revision + 1).ToString());

            // Set the path of the installer
            xmlWriter.WriteElementString("url", updateFilePath);

            // Send to installer the path of the current application with variable %path%
            xmlWriter.WriteElementString("args", "/SP- /silent /noicons /NOCANCEL /dir=\"%path%\"");

            xmlWriter.WriteElementString("changelog", "https://github.com/lemalcs/Khernet/releases/latest");
            xmlWriter.WriteElementString("mandatory", "true");
            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            return xmlFile;
        }
    }
}
