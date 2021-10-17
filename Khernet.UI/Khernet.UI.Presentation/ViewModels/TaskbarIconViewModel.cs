using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Khernet.UI
{
    public class TaskbarIconViewModel : BaseModel
    {
        /// <summary>
        /// Indicates whether application is running.
        /// </summary>
        public bool IsRunning { get; private set; } = true;

        public ICommand ShowWindowCommand
        {
            get
            {
                return new RelayCommand(ShowWindow, VerifyRunning);
            }
        }
        public ICommand HideWindowCommand
        {
            get
            {
                return new RelayCommand(HideWindow, VerifyRunning);
            }
        }
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(ExitApplication, VerifyRunning);
            }
        }


        /// <summary>
        /// Shutdowns the application.
        /// </summary>
        private async void ExitApplication()
        {
            IsRunning = false;
            Application.Current.MainWindow.Hide();

            await Task.Run(() =>
            {
                if (Engine.State == EngineState.Executing)
                {
                    IoCContainer.Media.Dispose();
                    IoCContainer.Text.Dispose();
                    IoCContainer.Get<ChatMessageStateManager>().StopProcessor();
                    IoCContainer.Get<UserManager>().StopProcessor();
                    IoCContainer.Get<MessageWritingChecker>().StopProcessor();
                    IoCContainer.Get<MessageProcessingEventManager>().StopProcessor();
                    Engine.Stop();
                }
            });
            Application.Current.Shutdown(0);
        }

        /// <summary>
        /// Verifies if application is running.
        /// </summary>
        /// <returns>True if application is running otherwise false.</returns>
        [DebuggerStepThrough]
        private bool VerifyRunning()
        {
            return IsRunning;
        }

        /// <summary>
        /// Shows the main window.
        /// </summary>
        private void ShowWindow()
        {
            IoCContainer.UI.ShowWindow();
        }

        /// <summary>
        /// Hides the main window.
        /// </summary>
        private void HideWindow()
        {
            Application.Current.MainWindow.Hide();
            IoCContainer.Get<ApplicationViewModel>().ClearChatPage();
        }
    }
}
