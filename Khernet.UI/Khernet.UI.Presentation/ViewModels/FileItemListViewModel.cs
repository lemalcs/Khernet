using System;
using System.Windows.Input;

namespace Khernet.UI
{
    public class FileItemListViewModel : BaseModel
    {
        /// <summary>
        /// The setting option name
        /// </summary>
        private string name;

        /// <summary>
        /// The action to execute when this items is selected
        /// </summary>
        public Action FileAction { get; set; }

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

        /// <summary>
        /// Command to open the list of files
        /// </summary>
        public ICommand OpenFileListCommand { get; private set; }

        public FileItemListViewModel()
        {
            OpenFileListCommand = new RelayCommand(OpenSetting);
        }

        /// <summary>
        /// Open a specific setting
        /// </summary>
        private void OpenSetting()
        {
            FileAction();
        }
    }
}
