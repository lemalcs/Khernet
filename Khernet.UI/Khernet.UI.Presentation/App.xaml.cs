using Hardcodet.Wpf.TaskbarNotification;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
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

            //Show main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
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
