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
        #region Properties

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
            get => currentViewModel;
            set
            {
                if (currentViewModel != value)
                {
                    currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command for play of pause audio files
        /// </summary>
        public ICommand PlayCommand { get; private set; }

        /// <summary>
        /// The command for mute sound
        /// </summary>
        public ICommand MuteCommand { get; private set; }

        #endregion

        public AudioPlayerViewModel(Action<object> Play, Action Mute)
        {
            PlayCommand = new RelayCommand(Play);
            MuteCommand = new RelayCommand(Mute);
        }
    }
}
