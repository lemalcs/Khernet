using System;
using System.Windows.Input;

namespace Khernet.UI
{
    public class SettingItemViewModel : BaseModel
    {
        /// <summary>
        /// Action to be performed when this setting item is openned
        /// </summary>
        private readonly Action openSetting;

        /// <summary>
        /// The setting option name
        /// </summary>
        private string name;

        /// <summary>
        /// The type of setting
        /// </summary>
        private AppOptions setting;

        /// <summary>
        /// The Hexadecimal value for color, for example: F5A2D8
        /// </summary>
        private string iconName;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string IconName
        {
            get { return iconName; }
            set
            {
                if (iconName != value)
                {
                    iconName = value;
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        public AppOptions Setting
        {
            get => setting;
            set
            {
                if (setting != value)
                {
                    setting = value;
                    OnPropertyChanged(nameof(Setting));
                }
            }
        }

        /// <summary>
        /// Command to open setting
        /// </summary>
        public ICommand OpenSettingCommand { get; private set; }

        public SettingItemViewModel(Action openSetting)
        {
            this.openSetting = openSetting;

            OpenSettingCommand = new RelayCommand(OpenSetting);
        }

        /// <summary>
        /// Open a specific setting
        /// </summary>
        private void OpenSetting()
        {
            openSetting?.Invoke();
        }
    }
}
