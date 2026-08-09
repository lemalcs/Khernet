using Khernet.UI.Converters;

namespace Khernet.UI.ViewModels
{
    public class PagedDialogDesignModel : PagedDialogViewModel
    {
        public PagedDialogDesignModel()
        {
            CurrentPage = ApplicationPage.SettingsList;
            Category = "Settings";
            CurrentViewModel = new SettingControllerViewModel();
        }

    }
}
