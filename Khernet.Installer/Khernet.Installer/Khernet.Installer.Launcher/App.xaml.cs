using Khernet.Installer.Launcher.Logger;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Khernet.Installer.Launcher
{
    /// <summary>
    /// Loads launcher application.
    /// </summary>
    public partial class App : Application
    {
        // The installation progress
        private Thread installerProcess;

        public ILogger logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            logger = new TextFileLogger("..\\KhernetUpdate.log");

            InstallerViewModel installerModel = new InstallerViewModel(
                new ResourceFileDownloader(Path.Combine("..\\..\\", AppEnvironment.AppName)),
                new SquirrelApplicationsUpdater(new string[]{
                "https://github.com/lemalcs/Khernet/releases/latest/download",
                    "..\\..\\khernetupgrade"},
                    logger),
                Assembly.GetExecutingAssembly().Location);

            Current.MainWindow = new MainWindow();
            Current.MainWindow.DataContext = installerModel;
            Current.MainWindow.Show();

            StartInstaller(installerModel);
        }

        /// <summary>
        /// Initialize the installation process.
        /// </summary>
        /// <param name="installerModel">The model of installer.</param>
        private void StartInstaller(BaseModel installerModel)
        {
            installerProcess = new Thread(new ParameterizedThreadStart(InstallApplication));
            installerProcess.Start(installerModel);
        }

        /// <summary>
        /// Installs and updates the main application.
        /// </summary>
        /// <param name="installerModel">The model of installer.</param>
        private void InstallApplication(object installerModel)
        {
            InstallerViewModel installer = (InstallerViewModel)installerModel;
            bool isInstalled = false;
            string appPath = null;
            try
            {
                appPath = installer.InstallApplication();
                isInstalled = true;
            }
            catch (Exception error)
            {
                logger.Log("Error while installing application.", error);
            }

            bool updateResult = false;
            if (isInstalled)
            {
                try
                {
                    updateResult = installer.UpdateApplication();
                }
                catch (Exception error)
                {
                    logger.Log("Error while installing application.", error);
                }

            }

            Current.Dispatcher.Invoke(() =>
            {

                if (Current.MainWindow != null)
                    Current.MainWindow.Close();

                if (!updateResult)
                    // Start main application, no updates were installed
                    Process.Start(appPath);
                else
                    // Start again launcher because application was updated
                    Process.Start(Path.Combine("..\\", Path.GetFileName(installer.InstallerPath)));

                Application.Current.Shutdown();
            });
        }
    }
}
