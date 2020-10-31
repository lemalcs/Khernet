namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile.
    /// </summary>
    public class NotificationDesignModel : NotificationViewModel
    {

        public NotificationDesignModel()
        {
            User = new UserItemViewModel
            {
                Initials = "L",
            };

            MessageType = Media.MessageType.Text;
        }

    }
}
