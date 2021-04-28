using Khernet.UI.Converters;
using Khernet.UI.IoC;
using System.Windows.Input;

namespace Khernet.UI
{
    public class PagedDialogViewModel : BaseModel, IPagedDialog
    {
        #region Properties

        /// <summary>
        /// The type of page.
        /// </summary>
        private ApplicationPage homePage;

        /// <summary>
        /// Title of home page.
        /// </summary>
        private string homeCategory;

        /// <summary>
        /// The model of home page.
        /// </summary>
        private BaseModel homeModel;

        /// <summary>
        /// Indicates if child dialog is visible.
        /// </summary>
        private bool isChildDialogVisible;

        /// <summary>
        /// The view model for dialog showed inside paged dialog.
        /// </summary>
        private BaseModel childDialogModel;

        /// <summary>
        /// The current page for settings.
        /// </summary>
        private ApplicationPage currentPage;

        /// <summary>
        /// The category of setting.
        /// </summary>
        private string category;

        /// <summary>
        /// The view model used for current page.
        /// </summary>
        private BaseModel currentViewModel;

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
                    OnPropertyChanged(nameof(IsHomePageEnabled));
                }
            }
        }

        public string Category
        {
            get
            {
                return category;
            }

            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(Category));
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

        /// <summary>
        /// Indicates whether this page is home.
        /// </summary>
        public bool IsHomePageEnabled
        {
            get
            {
                return CurrentPage != homePage;
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
                    OnPropertyChanged(nameof(IsChildDialogVisible));
                }
            }
        }

        public BaseModel ChildDialogModel
        {
            get => childDialogModel;
            set
            {
                if (childDialogModel != value)
                {
                    childDialogModel = value;
                    OnPropertyChanged(nameof(ChildDialogModel));
                }
            }
        }

        #endregion

        public ICommand GoToPageCommand { get; private set; }

        public ICommand CloseChildDialogCommand { get; private set; }


        public PagedDialogViewModel()
        {
            GoToPageCommand = new RelayCommand(GoToPage);
            CloseChildDialogCommand = new RelayCommand(CloseChildDialog);
        }

        public void CloseChildDialog()
        {
            IsChildDialogVisible = false;
        }

        public void ShowChildDialog(BaseModel model)
        {
            ChildDialogModel = model;
            IsChildDialogVisible = true;
        }

        private void GoToPage()
        {
            //Title for page
            Category = homeCategory;

            CurrentViewModel = homeModel;
            CurrentPage = homePage;
        }

        public void SetHomePage(ApplicationPage page, string category = null, BaseModel model = null)
        {
            //Set type of page
            homePage = page;

            //Title for page
            homeCategory = category;

            //Set the home page view model
            homeModel = model;
        }

        public void GoToPage(ApplicationPage page, BaseModel viewModel, string title)
        {
            Category = title;

            CurrentViewModel = viewModel;
            CurrentPage = page;
        }
    }
}
