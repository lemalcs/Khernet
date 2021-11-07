using Khernet.Core.Host;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for update tasks of application.
    /// </summary>
    public class UpdateViewModel : BaseModel
    {
        /// <summary>
        /// The path of update file.
        /// </summary>
        private string updateFilePath;

        /// <summary>
        /// Indicates whether updates will be get online (true) or locally (false).
        /// </summary>
        private bool getUpdateOnline;

        /// <summary>
        /// The dialog where settings are shown.
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        /// <summary>
        /// Indicates a description of the current operation being performed.
        /// </summary>
        private string textProgress;

        /// <summary>
        /// Indicates whether an update is available or not.
        /// </summary>
        private bool availableUpdate;

        /// <summary>
        /// Indicates if update command is enabled to update application.
        /// </summary>
        private bool showUpdateCommand;

        #region Commands

        /// <summary>
        /// Updates the application manually.
        /// </summary>
        public ICommand UpdateCommand { get; private set; }

        /// <summary>
        /// Opens file dialog to browse files.
        /// </summary>
        public ICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// Opens the home page on browser.
        /// </summary>
        public ICommand OpenUpdatesPageCommand { get; private set; }

        /// <summary>
        /// Check if updates for application is available.
        /// </summary>
        public ICommand CheckUpdatesPageCommand { get; private set; }

        #endregion

        #region Properties

        public string UpdateFilePath
        {
            get => updateFilePath;
            set
            {
                if (updateFilePath != value)
                {
                    updateFilePath = value;
                    OnPropertyChanged(nameof(UpdateFilePath));
                }
            }
        }

        public bool GetUpdateOnline
        {
            get => getUpdateOnline;
            set
            {
                getUpdateOnline = value;
                SetUpdateSource(getUpdateOnline);

                // When manual mode is set then always show the update command
                if (!getUpdateOnline)
                    ShowUpdateCommand = true;
                else
                    // When automatic mode is set then only show update command if an update is available
                    ShowUpdateCommand = !(GetUpdateOnline ^ AvailableUpdate);

                OnPropertyChanged(nameof(GetUpdateOnline));
            }
        }

        public string TextProgress
        {
            get => textProgress;
            set
            {
                if (textProgress != value)
                {
                    textProgress = value;
                    OnPropertyChanged(nameof(TextProgress));
                }

            }
        }

        public bool AvailableUpdate
        {
            get => availableUpdate;
            set
            {
                if (availableUpdate != value)
                {
                    availableUpdate = value;
                    ShowUpdateCommand = AvailableUpdate;
                    OnPropertyChanged(nameof(AvailableUpdate));
                }
            }
        }

        public bool ShowUpdateCommand
        {
            get => showUpdateCommand;
            set
            {
                if (showUpdateCommand != value)
                {
                    showUpdateCommand = value;
                    OnPropertyChanged(nameof(ShowUpdateCommand));
                }
            }
        }

        #endregion

        public UpdateViewModel()
        {
            UpdateCommand = new RelayCommand(UpdateApplication, IsUpdateAvailable);
            OpenFileCommand = new RelayCommand(OpenFile);
            OpenUpdatesPageCommand = new RelayCommand(OpenUpdatesPage);
            CheckUpdatesPageCommand = new RelayCommand(CheckUpdates);
        }

        public UpdateViewModel(IPagedDialog pagedDialog) : this()
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
            GetUpdateInfo();

        }

        private void GetUpdateInfo()
        {
            ApplicationConfigurations appConfigurations = new ApplicationConfigurations();
            GetUpdateOnline = appConfigurations.GetUpdateSource();

            if (GetUpdateOnline)
            {
                string newVersion = IoCContainer.Get<IUpdater>().CurrentVersion;
                AvailableUpdate = !string.IsNullOrEmpty(newVersion);
                TextProgress = !string.IsNullOrEmpty(newVersion) ? $"New version: {newVersion}" : string.Empty;
            }
        }

        /// <summary>
        /// Sets update mode to automatic or manual.
        /// </summary>
        /// <param name="updateSource">True for automatic mode or false to manual mode.</param>
        private void SetUpdateSource(bool updateSource)
        {
            ApplicationConfigurations appConfigurations = new ApplicationConfigurations();
            appConfigurations.SetUpdateSource(updateSource);
        }

        private async void CheckUpdates()
        {
            string newVersion = null;
            try
            {
                TextProgress = "Searching for updates..";
                ProgressDialogViewModel progressDialogView = new ProgressDialogViewModel
                {
                    TextProgress = this.TextProgress,
                    IsExecuting = true,
                };
                pagedDialog.ShowChildDialog(progressDialogView);

                newVersion = await SearchUpdates().ConfigureAwait(false);

                if (string.IsNullOrEmpty(newVersion))
                    TextProgress = "The application is up to date";
                else
                    TextProgress = $"New version: {newVersion}";

                progressDialogView.IsExecuting = false;
                progressDialogView.TextProgress = TextProgress;

                AvailableUpdate = !string.IsNullOrEmpty(newVersion);
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }

        private Task<string> SearchUpdates()
        {
            return Task.Run(() =>
            {
                return IoCContainer.Get<IUpdater>().CheckUpdate();
            });
        }

        private void OpenFile()
        {
            string[] files = IoCContainer.Get<IUIManager>().ShowOpenFileDialog();

            if (files == null || files.Length == 0)
                return;

            UpdateFilePath = files[0];
        }

        private async void UpdateApplication()
        {
            ApplicationUpdater updater = new ApplicationUpdater();

            try
            {
                IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.UpdatesProgress);
                await IoCContainer.Get<ApplicationViewModel>().SignOut();
                if (GetUpdateOnline)
                    updater.Update();
                else
                    updater.Update(UpdateFilePath);
            }
            catch (Exception error)
            {
                MessageBoxViewModel messageModel = new MessageBoxViewModel
                {
                    Message = $"Error: {error.Message}",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                };
                await IoCContainer.UI.ShowMessageBox(messageModel, true);
                IoCContainer.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
            }
        }

        private void OpenUpdatesPage()
        {
            Process.Start("https://github.com/lemalcs/Khernet/releases/latest");
        }

        private bool IsUpdateAvailable()
        {
            return (!string.IsNullOrEmpty(updateFilePath) && !string.IsNullOrWhiteSpace(updateFilePath)) ||
                GetUpdateOnline & AvailableUpdate;
        }
    }
}
