using Khernet.UI.Managers;
using System;
using System.Windows.Input;
using Unosquare.FFME;

namespace Khernet.UI
{
    /// <summary>
    /// The global model for audio files, only a single audio file playing is allowed.
    /// </summary>
    public class AudioPlayerViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The actual player of audio files.
        /// </summary>
        private MediaElement player;

        /// <summary>
        /// The model of audio chat message.
        /// </summary>
        private AudioChatMessageViewModel currentViewModel;

        /// <summary>
        /// Manages audio tasks.
        /// </summary>
        private readonly IAudioPlayer audioPlayer;

        /// <summary>
        /// The current state of audio playing.
        /// </summary>
        private MediaPlayerState state;

        public MediaElement Player
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

        public MediaPlayerState State
        {
            get => state;
            private set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to play or pause audio files.
        /// </summary>
        public ICommand PlayCommand { get; private set; }

        #endregion

        public AudioPlayerViewModel(IAudioPlayer audioPlayer)
        {
            this.audioPlayer = audioPlayer ?? throw new ArgumentNullException($"{nameof(audioPlayer)} cannot be null");

            PlayCommand = new RelayCommand(audioPlayer.Play);

            State = MediaPlayerState.Stopped;
        }

        /// <summary>
        /// Play or pause the audio file.
        /// </summary>
        public async void Play()
        {
            if (State == MediaPlayerState.Playing)
            {
                State = MediaPlayerState.Paused;
                await Player.Pause();
            }
            else if (State == MediaPlayerState.Paused)
            {
                State = MediaPlayerState.Playing;
                await Player.Play();
            }
            else if (State == MediaPlayerState.Stopped)
            {
                State = MediaPlayerState.Playing;
                Player.Position = TimeSpan.Zero;
                await Player.Play();
            }
        }

        /// <summary>
        /// Stops the audio player.
        /// </summary>
        public void Stop()
        {
            if (Player != null && Player.IsPlaying)
                Player.Stop();

            if (Player != null)
                Player.MediaEnded -= Player_MediaEnded;

            State = MediaPlayerState.Stopped;
            CurrentViewModel = null;
            audioPlayer.Stop();
        }

        /// <summary>
        /// Create the player to start playing a audio file.
        /// </summary>
        /// <param name="audioChatModel">The model of audio chat message.</param>
        public void CreatePlayerFor(AudioChatMessageViewModel audioChatModel)
        {
            if (Player == null)
            {
                Player = new MediaElement();
                Player.LoadedBehavior = System.Windows.Controls.MediaState.Play;
            }

            Stop();

            Player.MediaEnded += Player_MediaEnded;
            CurrentViewModel = audioChatModel;
            Player.Source = new Uri(CurrentViewModel.FilePath);
            State = MediaPlayerState.Playing;
        }

        private void Player_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            Stop();
        }
    }
}
