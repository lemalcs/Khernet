using Khernet.UI.Cache;
using System;
using System.Collections.Generic;
using Vlc.DotNet.Wpf;

namespace Khernet.UI.Managers
{
    public class AudioManager : IAudioObservable
    {
        /// <summary>
        /// The observers queue whom listen for notifications
        /// </summary>
        private SynchronizedCollection<IAudioObserver> observersList;

        /// <summary>
        /// The audio volume in percents, the default range is from 0% to 125%
        /// </summary>
        private int defaultVolume = 60;

        /// <summary>
        /// Indicates if VlcControl is created
        /// </summary>
        private bool isPlayerCreated = false;

        /// <summary>
        /// The global player model for audio files
        /// </summary>
        public AudioPlayerViewModel AudioModel { get; private set; }

        public AudioManager()
        {
            AudioModel = new AudioPlayerViewModel(Play);
        }

        private void Play(object mediaViewModel)
        {
            if (!isPlayerCreated || AudioModel.Player.SourceProvider.MediaPlayer == null ||
                (AudioModel.CurrentViewModel != null && AudioModel.CurrentViewModel.FilePath != (mediaViewModel as AudioChatMessageViewModel).FilePath))
            {
                AudioModel.CurrentViewModel = mediaViewModel as AudioChatMessageViewModel;
                CreatePlayer(AudioModel.CurrentViewModel.FilePath);
                return;
            }

            //Pauses the video if it is playing
            if (AudioModel.Player.SourceProvider.MediaPlayer.IsPlaying())
                AudioModel.Player.SourceProvider.MediaPlayer.Pause();
            else if (AudioModel.Player.SourceProvider.MediaPlayer.State == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Paused)
            {
                //Continue playing video if it is paused
                AudioModel.Player.SourceProvider.MediaPlayer.Play();
            }
            else
            {
                AudioModel.CurrentViewModel = mediaViewModel as AudioChatMessageViewModel;
                //Otherwise play video again
                CreatePlayer(AudioModel.CurrentViewModel.FilePath);
            }
        }

        private void CreatePlayer(string fileName)
        {
            if (AudioModel.Player != null)
                AudioModel.Player.Dispose();

            AudioModel.Player = new VlcControl();

            //Sets the directory path for vlc library
            AudioModel.Player.SourceProvider.CreatePlayer(Configurations.VlcDirectory);

            //Redirect log to console output rather than the default logger in vlc ibrary.
            AudioModel.Player.SourceProvider.MediaPlayer.Log += (s, ev) =>
            {
                string message = $"libVlc : {ev.Level} {ev.Message} @ {ev.Module}";
                System.Diagnostics.Debug.WriteLine(message);
            };

            //Set default volume to 50 db (decibels)
            AudioModel.Player.Volume = defaultVolume;

            //Set media source
            AudioModel.Player.SourceProvider.MediaPlayer.SetMedia(new Uri(fileName as string));

            //Plays the video from file video
            AudioModel.Player.SourceProvider.MediaPlayer.Play();

            isPlayerCreated = true;

            OnMediaChanged();
        }

        public void StopPlayer()
        {
            if (AudioModel.Player != null)
            {
                AudioModel.Player.Dispose();
                AudioModel.Player = null;
                isPlayerCreated = false;
                AudioModel.CurrentViewModel = null;
                OnMediaChanged();
            }
        }

        protected void OnMediaChanged()
        {
            for (int i = 0; i < observersList.Count; i++)
            {
                observersList[i].OnChangeAudio(AudioModel);
            }
        }

        #region IAudioObservable members
        public void Suscribe(IAudioObserver audioObserver)
        {
            if (observersList == null)
                observersList = new SynchronizedCollection<IAudioObserver>();

            if (!observersList.Contains(audioObserver))
                observersList.Add(audioObserver);
        }

        public void Unsuscribe(IAudioObserver audioObserver)
        {
            if (observersList == null)
                return;

            if (observersList.Contains(audioObserver))
                observersList.Remove(audioObserver);
        }
        #endregion


        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopPlayer();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
