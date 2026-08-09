namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile.
    /// </summary>
    public class GeneralSettingsDesingModel : GeneralSettingsViewModel
    {
        public GeneralSettingsDesingModel()
        {
            Size = 758;
            IsCleaning = false;
            TextProgress = "Cache cleared successfully";
        }
    }
}
