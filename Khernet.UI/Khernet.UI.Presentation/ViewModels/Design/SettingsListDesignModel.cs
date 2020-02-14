using System.Collections.Generic;

namespace Khernet.UI
{
    public class SettingsListDesignModel : SettingControllerViewModel
    {
        public SettingsListDesignModel()
        {
            var items = new List<SettingItemViewModel>();
            items.Add(new SettingItemViewModel(null)
            {
                Name = "Profile",
                IconName = "AccountCircle",
            });

            SetItems(items);
        }
    }
}
