namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile.
    /// </summary>
    public class ConnectionDesignModel : ConnectionViewModel
    {
        public ConnectionDesignModel()
        {
            IPAddress = "10.0.8.3";
            Port = 3533;
        }
    }
}
