using System;
using System.Windows.Input;
using Vlc.DotNet.Wpf;

namespace Khernet.UI
{
    /// <summary>
    /// The global model for audio files, only a single audio file playing allowed.
    /// </summary>
    public class AudioPlayerViewModel : BaseModel
    {
        /// <summary>
        /// The player of audio files that uses VLC library
        /// </summary>
        private VlcControl player;

        /// <summary>
        /// The model of audio messages
        /// </summary>
        private AudioChatMessageViewModel currentViewModel;

        public VlcControl Player
        {
            get
            {
                return player;
            }
            set
            {
                if (player != value)
                {
                    player = value;
                    OnPropertyChanged(nameof(Player));
                }
            }
        }
        public AudioChatMessageViewModel CurrentViewModel 
        { 
            get =>currentViewModel; 
            set
            {
                if(currentViewModel!=value)
                {
                    currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        /// <summary>
        /// The command for play of pause audio files
        /// </summary>
        public ICommand PlayCommand { get; private set; }

        public AudioPlayerViewModel(Action<object> Play)
        {
            PlayCommand = new RelayCommand(Play);
        }
    }
}
