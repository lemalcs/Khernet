using Khernet.Core.Host;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile.
    /// </summary>
    public class GeneralSettingsViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The total size of files in bytes.
        /// </summary>
        private long size;

        /// <summary>
        /// Indicates if cleaning operation is executing.
        /// </summary>
        private bool isCleaning;

        /// <summary>
        /// Message to be show about cleaning process.
        /// </summary>
        private string textProgress;

        /// <summary>
        /// The dialog where settings are shown.
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        /// <summary>
        /// Indicates whether the auto-run is enabled or not.
        /// </summary>
        private bool enableAutoRun;

        /// <summary>
        /// Indicates whether to run the application in background.
        /// </summary>
        private bool runInBackground;

        public long Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }

        public bool IsCleaning
        {
            get => isCleaning;
            set
            {
                if (isCleaning != value)
                {
                    isCleaning = value;
                    OnPropertyChanged(nameof(IsCleaning));
                }
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

        public bool EnableAutoRun
        {
            get => enableAutoRun;
            set
            {
                if (enableAutoRun != value)
                {
                    enableAutoRun = value;
                    OnPropertyChanged(nameof(EnableAutoRun));
                }
            }
        }

        public bool RunInBackground
        {
            get => runInBackground;
            set
            {
                if (runInBackground != value)
                {
                    runInBackground = value;
                    OnPropertyChanged(nameof(RunInBackground));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Clear the files located in cache directory.
        /// </summary>
        public ICommand RunInBackgroundCommand { get; private set; }

        public ICommand EnableAutoRunCommand { get; private set; }

        #endregion

        public GeneralSettingsViewModel()
        {
            RunInBackgroundCommand = new UI.RelayCommand(ChangeRunInBackground);
            EnableAutoRunCommand = new RelayCommand(ChangeAutorun);

            GetAutoRunValue();
            GetRunInBackgroundValue();
        }

        private void OpenCache()
        {
            Process.Start(Configurations.CacheDirectory.FullName);
        }

        public GeneralSettingsViewModel(IPagedDialog pagedDialog) : this()
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
        }


        private void GetAutoRunValue()
        {
            ApplicationConfigurations applicationConfigurations = new ApplicationConfigurations();
            EnableAutoRun = applicationConfigurations.GetAutoRun();
        }

        private void GetRunInBackgroundValue()
        {
            ApplicationConfigurations applicationConfigurations = new ApplicationConfigurations();
            RunInBackground = applicationConfigurations.GetStartInBackGround();
        }

        private void ChangeRunInBackground()
        {
            ApplicationConfigurations applicationConfigurations = new ApplicationConfigurations();
            applicationConfigurations.SetStartInBackGround(RunInBackground);
        }

        private void ChangeAutorun()
        {
            RegionFactory regionFactory = new RegionFactory();
            if (EnableAutoRun)
            {
                regionFactory.CreateShortcut();
            }
            else
            {
                regionFactory.RemoveShortcut();
                RunInBackground = EnableAutoRun;
                ChangeRunInBackground();
            }
            ApplicationConfigurations applicationConfigurations = new ApplicationConfigurations();
            applicationConfigurations.SetAutoRun(EnableAutoRun);
        }
    }
}
