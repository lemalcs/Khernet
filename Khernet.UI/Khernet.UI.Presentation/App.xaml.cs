using Hardcodet.Wpf.TaskbarNotification;
using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using Khernet.UI.Resources;
using System.Windows;

namespace Khernet.UI
{
    /// <summary>
    /// Logic for whole application
    /// </summary>
    public partial class App : Application
    {
        TaskbarIcon notificationIcon;

        /// <summary>
        /// Execute tasks when application starts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            base.OnStartup(e);

            BuildEnvironment();

            Setup();

            //Show main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            SetNotificationIcon();
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogDumper.WriteLog(e.Exception);
            IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
            {
                Message = Constants.ErrorMessage,
                ShowAcceptOption = true,
                AcceptOptionLabel = "OK",
            }
            );
        }

        private void BuildEnvironment()
        {
            RegionFactory regionFactory = new RegionFactory();
            regionFactory.Build();
        }

        /// <summary>
        /// Configure dependency injection before any window is showed
        /// </summary>
        private void Setup()
        {
            IoCContainer.Configure<IUIManager, UIManager>(new UIManager());

            IoCContainer.Configure<ApplicationViewModel>();
        }

        private void SetNotificationIcon()
        {
            notificationIcon = Resources["notificationIcon"] as TaskbarIcon;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //Dispose notification icon
            notificationIcon.Dispose();

            base.OnExit(e);
        }
    }
}
