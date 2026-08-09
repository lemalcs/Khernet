using System;
using System.Windows.Input;
using Unosquare.FFME;

namespace Khernet.UI
{
    /// <summary>
    /// The states that and audio or video file can have.
    /// </summary>
    public enum MediaPlayerState
    {
        /// <summary>
        /// Media file is playing.
        /// </summary>
        Playing = 1,

        /// <summary>
        /// Media file is paused.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// Media file is stopped.
        /// </summary>
        Stopped = 3,
    }

    /// <summary>
    /// The model for video files.
    /// </summary>
    public class VideoPlayerViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The player of video files.
        /// </summary>
        private MediaElement player;

        /// <summary>
        /// The current state of video.
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
        /// The command to play or pause video.
        /// </summary>
        public ICommand PlayCommand { get; private set; }

        /// <summary>
        /// The command to close a video.
        /// </summary>
        public ICommand CloseCommand { get; private set; }

        #endregion

        public VideoPlayerViewModel()
        {
            PlayCommand = new RelayCommand(PlayVideo);
            CloseCommand = new RelayCommand(CloseVideo);
            State = MediaPlayerState.Playing;
        }

        private async void CloseVideo(object videoPath)
        {
            await Player.Close();
        }

        /// <summary>
        /// Plays or pauses the current video.
        /// </summary>
        private async void PlayVideo()
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
        /// Sets the state of video to <see cref="MediaPlayerState.Stopped"/>
        /// </summary>
        public void SetStoppedVideo()
        {
            State = MediaPlayerState.Stopped;
        }

    }
}
