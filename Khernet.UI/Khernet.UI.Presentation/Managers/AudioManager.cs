using System.Collections.Generic;

namespace Khernet.UI.Managers
{
    public class AudioManager : IAudioObservable, IAudioPlayer
    {
        /// <summary>
        /// The observers queue whom listen for notifications.
        /// </summary>
        private SynchronizedCollection<IAudioObserver> observersList;

        /// <summary>
        /// The global player model for audio files.
        /// </summary>
        public AudioPlayerViewModel AudioModel { get; private set; }

        public AudioManager()
        {
            AudioModel = new AudioPlayerViewModel(this);
        }

        #region IAudioPlayer members

        /// <summary>
        /// Starts to play the audio file
        /// </summary>
        /// <param name="mediaViewModel">The model of audio chat message.</param>
        public void Play(object mediaViewModel)
        {
            if (AudioModel.State == MediaPlayerState.Stopped ||
               (AudioModel.CurrentViewModel != null && AudioModel.CurrentViewModel.FilePath != (mediaViewModel as AudioChatMessageViewModel).FilePath))
            {
                AudioModel.CreatePlayerFor(mediaViewModel as AudioChatMessageViewModel);
                OnMediaChanged();
            }
            else
                AudioModel.Play();
        }

        /// <summary>
        /// Notifies that audio file stopped playing.
        /// </summary>
        public void Stop()
        {
            OnMediaChanged();
        }

        #endregion

        /// <summary>
        /// Notifies subscribers that and change occurred in an audio playing.
        /// </summary>
        protected void OnMediaChanged()
        {
            if (observersList == null || observersList.Count == 0)
                return;

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
        /// Variable to detect reentry calls.
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
