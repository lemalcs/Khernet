using System;
using System.IO;

namespace Khernet.Installer.Launcher
{
    public class InstallerViewModel : BaseModel
    {
        private readonly IFileDownloader fileDownloader;
        private readonly IApplicationUpdater applicationUpdater;
        private string installerPath;

        /// <summary>
        /// The percentage of work done.
        /// </summary>
        private int percentageDone;

        // The current action that is being performed by installer.
        private string currentAction;
        public string CurrentAction
        {
            get => currentAction;
            set
            {
                if (currentAction != value)
                {
                    currentAction = value;
                    OnPropertyChanged(nameof(CurrentAction));
                }
            }
        }

        public int PercentageDone
        {
            get => percentageDone;
            set
            {
                if (percentageDone != value)
                {
                    percentageDone = value;
                    OnPropertyChanged(nameof(PercentageDone));
                }
            }
        }

        public string InstallerPath { get => installerPath; }

        public InstallerViewModel(IFileDownloader fileDownloader, IApplicationUpdater applicationUpdater, string installerPath)
        {
            if (fileDownloader == null)
                throw new ArgumentNullException($"Parameter {nameof(fileDownloader)} cannot be null");

            if (applicationUpdater == null)
                throw new ArgumentNullException($"Parameter {nameof(applicationUpdater)} cannot be null");

            this.fileDownloader = fileDownloader;
            this.applicationUpdater = applicationUpdater;
            this.installerPath = installerPath;

            CurrentAction = "Starting...";
        }

        /// <summary>
        /// Installs the application.
        /// </summary>
        /// <returns>The path of installed application.</returns>
        public string InstallApplication()
        {
            PercentageDone = 0;

            AppEnvironment appEnvironment = new AppEnvironment(installerPath);
            string appPath = appEnvironment.AppPath;
            if (!appEnvironment.ExistsAppDirectory())
            {
                appPath = appEnvironment.CreateAppDirectory();
            }

            if (!File.Exists(appEnvironment.AppPath))
            {
                CurrentAction = "Installing...";

                // Install main executable
                fileDownloader.Download(AppEnvironment.AppName, Environment.CurrentDirectory);
                File.Move(Path.Combine(Environment.CurrentDirectory, AppEnvironment.AppName),
                            Path.Combine(appPath, AppEnvironment.AppName));
                PercentageDone = 10;

                // Install database engine
                fileDownloader.Download(AppEnvironment.FirebirdPack, Environment.CurrentDirectory);
                using (FileStream fileStream = new FileStream(Path.Combine(Environment.CurrentDirectory, AppEnvironment.FirebirdPack), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Compressor c = new Compressor();
                    c.UnZipFile(fileStream, Path.Combine(appPath, "firebird"));
                }
                File.Delete(Path.Combine(Environment.CurrentDirectory, AppEnvironment.FirebirdPack));
                PercentageDone = 20;

                // Install media tools
                fileDownloader.Download(AppEnvironment.MediaPack, Environment.CurrentDirectory);
                using (FileStream fileStream = new FileStream(Path.Combine(Environment.CurrentDirectory, AppEnvironment.MediaPack), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Compressor c = new Compressor();
                    c.UnZipFile(fileStream, Path.Combine(appPath, "media"));
                }
                File.Delete(Path.Combine(Environment.CurrentDirectory, AppEnvironment.MediaPack));
                PercentageDone = 30;

                // Install VLC library
                fileDownloader.Download(AppEnvironment.VlcPack, Environment.CurrentDirectory);
                using (FileStream fileStream = new FileStream(Path.Combine(Environment.CurrentDirectory, AppEnvironment.VlcPack), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Compressor c = new Compressor();
                    c.UnZipFile(fileStream, Path.Combine(appPath, "libvlc"));
                }
                File.Delete(Path.Combine(Environment.CurrentDirectory, AppEnvironment.VlcPack));
                PercentageDone = 40;
            }

            return appEnvironment.AppPath;
        }

        /// <summary>
        /// Updates the applications.
        /// </summary>
        /// <returns>True if application was updated successfully otherwise false.</returns>
        public bool UpdateApplication()
        {
            CurrentAction = "Updating...";
            PercentageDone = 50;

            if (applicationUpdater.Update() == UpdateResult.Success)
            {
                return true;
            }
            PercentageDone = 100;
            return false;
        }
    }
}
