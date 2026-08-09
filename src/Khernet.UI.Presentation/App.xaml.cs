using Hardcodet.Wpf.TaskbarNotification;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Unosquare.FFME;
using Unosquare.FFME.Shared;

namespace Khernet.UI
{
    /// <summary>
    /// Logic for whole application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Execute tasks when application starts.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Setup();

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            Current.MainWindow = new MainWindow();

            if (ExistsRunningInstances())
            {
                ShowClosingMessage();
            }
            else
            {
                IoCContainer.Get<ApplicationViewModel>().StartApplication();
            }
        }

        private bool ExistsRunningInstances()
        {
            Process[] processList = Process.GetProcesses();
            int instanceNumber = 0;

            foreach (Process process in processList)
            {
                try
                {
                    if (process.ProcessName == Path.GetFileNameWithoutExtension(Configurations.MainApplicationAssembly) &&
                        process.MainModule.FileName == Configurations.MainApplicationAssembly)
                    {
                        instanceNumber++;
                    }
                }
                catch (Exception)
                {
                    // Bypass 64-bits applications.
                }
            }
            return instanceNumber > 1;
        }

        private async void ShowClosingMessage()
        {
            Current.MainWindow.Show();
            await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
            {
                Message = $"Another instance of this very application is already running; close it and retry.\n" +
                $"The application is at:\n" +
                $"{Configurations.MainApplicationAssembly}",
                ShowAcceptOption = true,
                AcceptOptionLabel = "OK",
            });
            App.Current.Shutdown(1);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogDumper.WriteLog(e.Exception);
            IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
            {
                Message = Khernet.UI.Resources.Constants.ErrorMessage,
                ShowAcceptOption = true,
                AcceptOptionLabel = "OK",
            }
            );
        }

        /// <summary>
        /// Configure dependency injection before any window is showed.
        /// </summary>
        private void Setup()
        {
            IoCContainer.Configure<IUIManager, UIManager>(new UIManager());
            IoCContainer.Configure<IUpdater, ApplicationUpdater>(new ApplicationUpdater());

            IoCContainer.Configure<IApplicationManager, TaskbarIconViewModel>(new TaskbarIconViewModel());
            TaskbarIcon notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;
            notificationIcon.DataContext = IoCContainer.Get<IApplicationManager>();

            IoCContainer.Configure<ApplicationViewModel>();

            // Directory of ffmpeg needed to play audio and video files.
            MediaElement.FFmpegDirectory = Configurations.FFMEGDirectory.FullName;

            // Full Features is already the default.
            MediaElement.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
