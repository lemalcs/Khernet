using Khernet.UI.Cache;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Vlc.DotNet.Wpf;

namespace Khernet.UI
{
    public class MediaViewModel : BaseModel, IDisposable
    {
        /// <summary>
        /// Directory where libvlc (C library) is located.
        /// </summary>
        private readonly DirectoryInfo vlcLibDirectory;

        /// <summary>
        /// The audio volume in percents, the default range is from 0% to 125%
        /// </summary>
        private int defaultVolume = 60;

        private bool isPlayerCreated = false;

        private VlcControl player;

        public event EventHandler MediaChanged;

        public VlcControl Player
        {
            get
            {
                return player;
            }
            private set
            {
                if (player != value)
                {
                    player = value;
                    OnPropertyChanged(nameof(Player));
                }
            }
        }

        public ICommand PlayCommand { get; private set; }
        public AudioChatMessageViewModel CurrentViewModel { get; private set; }

        public MediaViewModel()
        {
            PlayCommand = new RelayCommand(Play);

            var currentAssembly = Assembly.GetEntryAssembly();

            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            vlcLibDirectory = Configurations.VlcDirectory;
        }

        private void Play(object mediaViewModel)
        {
            if (!isPlayerCreated || Player.SourceProvider.MediaPlayer == null||
                (CurrentViewModel!=null&&CurrentViewModel.FilePath!=(mediaViewModel as AudioChatMessageViewModel).FilePath))
            {
                CurrentViewModel = mediaViewModel as AudioChatMessageViewModel;
                CreatePlayer(CurrentViewModel.FilePath);
                return;
            }

            //Pauses the video if it is playing
            if (Player.SourceProvider.MediaPlayer.IsPlaying())
                Player.SourceProvider.MediaPlayer.Pause();
            else if (Player.SourceProvider.MediaPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Paused)
            {
                //Continue playing video if it is paused
                Player.SourceProvider.MediaPlayer.Play();
            }
            else
            {
                CurrentViewModel = mediaViewModel as AudioChatMessageViewModel;
                //Otherwise play video again
                CreatePlayer(CurrentViewModel.FilePath);
            }
        }

        private void CreatePlayer(string fileName)
        {
            if (Player != null)
                Player.Dispose();

            Player = new VlcControl();

            //Sets the directory path for vlc library
            Player.SourceProvider.CreatePlayer(vlcLibDirectory);

            //Redirect log to console output rather than the default logger in vlc ibrary.
            Player.SourceProvider.MediaPlayer.Log += (s, ev) =>
            {
                string message = $"libVlc : {ev.Level} {ev.Message} @ {ev.Module}";
                System.Diagnostics.Debug.WriteLine(message);
            };

            //Set default volume to 50 db (decibels)
            Player.Volume = defaultVolume;

            //Set media source
            Player.SourceProvider.MediaPlayer.SetMedia(new Uri(fileName as string));

            //Plays the video from file video
            Player.SourceProvider.MediaPlayer.Play();

            isPlayerCreated = true;

            OnMediaChanged();
        }

        protected void OnMediaChanged()
        {
            MediaChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            if (Player != null)
            {
                Player.Dispose();
                Player = null;
                isPlayerCreated = false;
                CurrentViewModel = null;
                OnMediaChanged();
            }

        }
    }
}
