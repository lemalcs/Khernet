using Khernet.UI.Pages;

namespace Khernet.UI.Converters
{
    public enum ApplicationPage
    {
        Login = 0,
        SignUp = 1,
        Session = 2,
        Chat = 3,
        SettingsList = 4,
        Profile = 5,
        ProfileViewer = 6,
        AudioPage = 7,
        Resend = 8,
        Cache = 9,
        SignOut = 10,
        ImageList = 11,
        VideoList = 12,
        FileList = 13,
        AudioList = 14,
        About = 15,
        Load = 16,
        Updates = 17,
        UpdatesProgress = 18,
        AddContact = 19,
        Connection = 20,
        General= 21,
    }

    /// <summary>
    /// Convert the <see cref="ApplicationPage"/>  to an actual Page for user interface.
    /// </summary>
    public static class PageHelper
    {
        /// <summary>
        /// Converts a <see cref="ApplicationPage"/> to <see cref="BasePage"/> .
        /// </summary>
        /// <param name="page">The page to get to.</param>
        /// <param name="viewModel">Optional view model.</param>
        /// <returns>The <see cref="BasePage"/> object for requested page.</returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            //Returns the page according to KhernetPage enumeration in value parameter
            switch (page)
            {
                case ApplicationPage.Login:
                    return new LoginPage(viewModel as LoginViewModel);
                case ApplicationPage.SignUp:
                    return new SignUpPage(viewModel as SignUpViewModel);
                case ApplicationPage.Session:
                    return new SessionPage();
                case ApplicationPage.Chat:
                    return new ChatPage(viewModel as ChatMessageListViewModel);
                case ApplicationPage.SettingsList:
                    return new SettingsPage(viewModel as SettingControllerViewModel);
                case ApplicationPage.Profile:
                    return new ProfilePage(viewModel as ProfileViewModel);
                case ApplicationPage.ProfileViewer:
                    return new ProfileViewerPage(viewModel as ProfileViewModel);
                case ApplicationPage.Resend:
                    return new ResendPage(viewModel as ResendViewModel);
                case ApplicationPage.Cache:
                    return new CachePage(viewModel as CacheViewModel);
                case ApplicationPage.SignOut:
                    return new SignOutPage();
                case ApplicationPage.ImageList:
                    return new ImageListPage(viewModel as FileListViewModel);
                case ApplicationPage.VideoList:
                    return new VideoListPage(viewModel as FileListViewModel);
                case ApplicationPage.FileList:
                    return new FileListPage(viewModel as FileListViewModel);
                case ApplicationPage.AudioList:
                    return new AudioListPage(viewModel as FileListViewModel);
                case ApplicationPage.About:
                    return new AboutPage(viewModel as AboutViewModel);
                case ApplicationPage.Load:
                    return new LoadPage();
                case ApplicationPage.Updates:
                    return new UpdatePage(viewModel as UpdateViewModel);
                case ApplicationPage.UpdatesProgress:
                    return new UpdateProgressPage();
                case ApplicationPage.AddContact:
                    return new AddContactPage(viewModel as AddContactViewModel);
                case ApplicationPage.Connection:
                    return new ConnectionPage(viewModel as ConnectionViewModel);
                case ApplicationPage.General:
                    return new GeneralSettingsPage(viewModel as GeneralSettingsViewModel);
                default:
                    System.Diagnostics.Debugger.Break();
                    return null;
            }
        }
    }
}
