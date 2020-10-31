using System;

namespace Khernet.UI.Managers
{
    public interface IAudioObservable : IDisposable
    {
        /// <summary>
        /// The model used to play a single audio file along application (global player)
        /// </summary>
        AudioPlayerViewModel AudioModel { get; }

        /// <summary>
        /// Subscribes to audio tracker.
        /// </summary>
        void Suscribe(IAudioObserver audioObserver);

        /// <summary>
        /// Unsubscribe from audio tracker.
        /// </summary>
        void Unsuscribe(IAudioObserver audioObserver);

        /// <summary>
        /// Stop playing and audio file and release resources.
        /// </summary>
        void StopPlayer();
    }

    public interface IAudioObserver
    {
        /// <summary>
        /// Notify when a new audio file is being tracked.
        /// </summary>
        /// <param name="audioModel">The model of global audio player.</param>
        void OnChangeAudio(AudioPlayerViewModel audioModel);
    }
}
