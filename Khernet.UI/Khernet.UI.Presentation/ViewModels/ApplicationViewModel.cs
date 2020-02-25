using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.Cache;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public class ApplicationViewModel : BaseModel
    {
        #region Properties
        /// <summary>
        /// The current page displayed
        /// </summary>
        private ApplicationPage currentPage;

        /// <summary>
        /// Indicates if user list is visible
        /// </summary>
        private bool isSidePanelVisible;

        /// <summary>
        /// Indicates if audio player is visible
        /// </summary>
        private bool isPlayerVisible;

        /// <summary>
        /// Indicates if a modal dialog is shown
        /// </summary>
        private bool isModalDialogVisible;

        /// <summary>
        /// Indicates if a child dialog owned by <see cref="ModalDialogViewModel"/> is shown
        /// </summary>
        private bool isChildDialogVisible;

        /// <summary>
        /// Indicates if a message box is shown
        /// </summary>
        private bool isMessageBoxVisible;

        /// <summary>
        /// The view model for current page
        /// </summary>
        private BaseModel currentViewModel;

        /// <summary>
        /// The view model for audio player
        /// </summary>
        private BaseModel playerViewModel;

        /// <summary>
        /// The view model of modal dialog
        /// </summary>
        private BaseModel modalDialogViewModel;

        /// <summary>
        /// The view model of child dialog owned by <see cref="ModalDialogViewModel"/>
        /// </summary>
        private BaseModel childDialogViewModel;

        /// <summary>
        /// The view model for modal dialogs
        /// </summary>
        private ModalDialogViewModel messageViewModel;

        /// <summary>
        /// The view model for user list
        /// </summary>
        private UserListViewModel userViewModel;

        /// <summary>
        /// The Z index of overlay panel when a modai dialog is shown
        /// </summary>
        private int overlayLevel;

        /// <summary>
        /// The previous value of Z index of overlay panel
        /// </summary>
        private int previousOverlayLevel;

        /// <summary>
        /// The current Page application shows
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get
            {
                return currentPage;
            }

            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }



        /// <summary>
        /// Indicates if side panel is visible
        /// </summary>
        public bool IsSidePanelVisible
        {
            get
            {
                return isSidePanelVisible;
            }

            set
            {
                if (isSidePanelVisible != value)
                {
                    isSidePanelVisible = value;
                    OnPropertyChanged(nameof(IsSidePanelVisible));
                }
            }
        }

        /// <summary>
        /// Indicates whether to show or hide dimmed overlay
        /// </summary>
        private bool isOverlayVisible;

        public bool IsOverlayVisible
        {
            get
            {
                return isOverlayVisible;
            }

            set
            {
                if (isOverlayVisible != value)
                {
                    isOverlayVisible = value;
                    OnPropertyChanged(nameof(IsOverlayVisible));
                }
            }
        }

        public BaseModel CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }

            set
            {
                if (currentViewModel != value)
                {
                    currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        public bool IsPlayerVisible
        {
            get => isPlayerVisible;
            set
            {
                if (isPlayerVisible != value)
                {
                    isPlayerVisible = value;

                    //Remove previuos view model when hidding audio player
                    if (!value)
                        PlayerViewModel = null;

                    OnPropertyChanged(nameof(IsPlayerVisible));
                }
            }
        }

        public BaseModel PlayerViewModel
        {
            get => playerViewModel;
            set
            {
                if (playerViewModel != value)
                {
                    playerViewModel = value;
                    OnPropertyChanged(nameof(PlayerViewModel));
                }
            }
        }

        public bool IsModalDialogVisible
        {
            get => isModalDialogVisible;
            set
            {
                if (isModalDialogVisible != value)
                {
                    isModalDialogVisible = value;
                    if (!value)
                    {
                        ModalDialogViewModel = null;
                        IsOverlayVisible = false;
                    }

                    OnPropertyChanged(nameof(IsModalDialogVisible));
                }
            }
        }

        public BaseModel ModalDialogViewModel
        {
            get => modalDialogViewModel;
            set
            {
                if (modalDialogViewModel != value)
                {
                    modalDialogViewModel = value;
                    OnPropertyChanged(nameof(ModalDialogViewModel));
                }
            }
        }

        public bool IsMessageBoxVisible
        {
            get => isMessageBoxVisible;
            set
            {
                if (isMessageBoxVisible != value)
                {
                    isMessageBoxVisible = value;
                    if (!value)
                    {
                        MessageViewModel = null;

                        //Don't hide overlay panel if a mmodal dialog is visible yet
                        if (!IsModalDialogVisible)
                            IsOverlayVisible = false;

                        OverlayLevel = previousOverlayLevel;
                    }

                    OnPropertyChanged(nameof(IsMessageBoxVisible));
                }
            }
        }

        public ModalDialogViewModel MessageViewModel
        {
            get => messageViewModel;
            set
            {
                if (messageViewModel != value)
                {
                    messageViewModel = value;
                    OnPropertyChanged(nameof(MessageViewModel));
                }
            }
        }

        public UserListViewModel UserViewModel
        {
            get => userViewModel;
            set
            {
                if (userViewModel != value)
                {
                    userViewModel = value;
                    OnPropertyChanged(nameof(UserViewModel));
                }
            }
        }

        public BaseModel ChildDialogViewModel
        {
            get => childDialogViewModel;
            set
            {
                if (childDialogViewModel != value)
                {
                    childDialogViewModel = value;
                    OnPropertyChanged(nameof(ChildDialogViewModel));
                }
            }
        }
        public bool IsChildDialogVisible
        {
            get => isChildDialogVisible;
            set
            {
                if (isChildDialogVisible != value)
                {
                    isChildDialogVisible = value;
                    if (!value)
                    {
                        ChildDialogViewModel = null;
                        OverlayLevel = 1;
                    }
                    OnPropertyChanged(nameof(IsChildDialogVisible));
                }
            }
        }

        public int OverlayLevel
        {
            get => overlayLevel;
            set
            {
                if (overlayLevel != value)
                {
                    overlayLevel = value;
                    OnPropertyChanged(nameof(OverlayLevel));
                }
            }
        }

        #endregion

        public ICommand ViewSettingsCommand { get; private set; }

        public ApplicationViewModel()
        {
            IsSidePanelVisible = false;
            IsOverlayVisible = false;
            CurrentPage = ApplicationPage.Session;
            CurrentViewModel = null;
            IsPlayerVisible = false;

            ViewSettingsCommand = new RelayCommand(ViewSettings);

            IsSidePanelVisible = CurrentPage == ApplicationPage.Session || CurrentPage == ApplicationPage.Chat;


            if (!IoCContainer.UI.IsInDesignTime())
            {
                GoToPage(ApplicationPage.Load);
            }
        }

        public async void SignOut()
        {
            try
            {
                GoToPage(ApplicationPage.SignOut);

                IsSidePanelVisible = false;
                IsOverlayVisible = false;
                CurrentViewModel = null;
                IsPlayerVisible = false;

                ModalDialogViewModel = null;
                IsModalDialogVisible = false;
                PlayerViewModel = null;
                MessageViewModel = null;
                IsOverlayVisible = false;
                UserViewModel = null;

                IoCContainer.Chat.Clear();

                await TaskEx.Run(() =>
                {
                    IoCContainer.Media.Dispose();
                    IoCContainer.Text.Dispose();
                    IoCContainer.Get<IAudioObservable>().StopPlayer();
                    IoCContainer.Get<ChatMessageStateManager>().StopProcessor();
                    IoCContainer.Get<UserManager>().StopProcessor();
                    Engine.Stop();
                });

                GoToPage(ApplicationPage.Login);
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }

        /// <summary>
        /// Opens settings dialog
        /// </summary>
        /// <param name="obj"></param>
        private void ViewSettings(object obj)
        {
            //Set settings list as first page
            var pagedVM = new PagedDialogViewModel();

            //Set list of settings as first page
            pagedVM.CurrentPage = ApplicationPage.SettingsList;

            //Title for page
            pagedVM.Category = "Settings";

            //Set the view model for settings list
            pagedVM.CurrentViewModel = new SettingControllerViewModel(pagedVM);

            pagedVM.SetHomePage(pagedVM.CurrentPage, pagedVM.Category, pagedVM.CurrentViewModel);

            //Open a modal dialog for settings
            IoCContainer.UI.ShowDialog(pagedVM);
        }


        /// <summary>
        /// Navigates to page with specific view model
        /// </summary>
        /// <param name="page">The page to navigate to</param>
        /// <param name="viewModel">The view model for page to use</param>
        public void GoToPage(ApplicationPage page, BaseModel viewModel = null)
        {
            //View model for page
            CurrentViewModel = viewModel;

            //Set current page to navigate to
            CurrentPage = page;

            OnPropertyChanged(nameof(CurrentPage));

            IsSidePanelVisible = CurrentPage == ApplicationPage.Session || CurrentPage == ApplicationPage.Chat;

            if (Engine.State == EngineState.Executing &&
                CurrentPage != ApplicationPage.SignOut &&
                UserViewModel == null)
            {
                LoadUsers();
            }
        }

        /// <summary>
        /// Get users from data source
        /// </summary>
        /// <returns><see cref="Task"/> that performs loading</returns>
        private void LoadUsers()
        {
            try
            {
                UserViewModel = IoCContainer.Get<UserListViewModel>();
                var users = IoCContainer.Get<Messenger>().GetPeers();

                if (users == null)
                    return;

                //Load profiles of users
                for (int i = 0; i < users.Count; i++)
                {
                    IoCContainer.Get<UserManager>().ProcessState(new UserState
                    {
                        Token = users[i].AccountToken,
                        Change = UserChangeType.ProfileLoading,
                    });
                }

                //Load avatars of users
                for (int i = 0; i < users.Count; i++)
                {
                    IoCContainer.Get<UserManager>().ProcessState(new UserState
                    {
                        Token = users[i].AccountToken,
                        Change = UserChangeType.AvatarLoading,
                    });
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
            }
        }

        /// <summary>
        /// Shows audio player
        /// </summary>
        /// <param name="viewModel">The view model for audio player</param>
        public void ShowPlayer(BaseModel viewModel)
        {
            PlayerViewModel = viewModel;
            IsPlayerVisible = true;
        }

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <param name="viewModel">The view model for dialog</param>
        public void ShowModalDialog(BaseModel viewModel)
        {
            if (CurrentPage == ApplicationPage.SignOut)
                return;

            ModalDialogViewModel = viewModel;
            OverlayLevel = 1;
            IsOverlayVisible = true;
            IsModalDialogVisible = true;
        }

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <param name="viewModel">The view model for dialog</param>
        public void ShowChildModalDialog(BaseModel viewModel)
        {
            if (CurrentPage == ApplicationPage.SignOut)
                return;

            ChildDialogViewModel = viewModel;
            OverlayLevel = 3;
            IsOverlayVisible = true;
            IsChildDialogVisible = true;
        }

        /// <summary>
        /// Showa a modal message box
        /// </summary>
        /// <param name="viewModel">The view model for message box</param>
        public void ShowMessageBox(ModalDialogViewModel viewModel)
        {
            if (CurrentPage == ApplicationPage.SignOut)
                return;

            MessageViewModel = viewModel;

            if (OverlayLevel != 5)
                previousOverlayLevel = OverlayLevel;
            OverlayLevel = 5;
            IsOverlayVisible = true;
            IsMessageBoxVisible = true;
        }

        /// <summary>
        /// Unselect an user from list when manin window is shown after it was hidden
        /// </summary>
        public void ClearChatPage()
        {
            if (CurrentPage == ApplicationPage.Session || CurrentPage == ApplicationPage.Chat)
                IoCContainer.Get<UserListViewModel>().ClearSelection();
        }
    }
}
