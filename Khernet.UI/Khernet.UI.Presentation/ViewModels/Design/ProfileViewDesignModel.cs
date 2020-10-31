namespace Khernet.UI
{
    /// <summary>
    /// Design model for user profile.
    /// </summary>
    public class ProfileViewDesignModel : ProfileViewModel
    {

        public ProfileViewDesignModel() : base(new ChatMessageListViewModel())
        {
            User = new UserItemViewModel
            {
                State = "Available",
                Group = "Programming",
                Slogan = "Learn for life, this is a large slogan that must fit in profile window, let's see how does it look like...",
            };
        }
    }
}
