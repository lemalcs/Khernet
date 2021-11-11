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
        void Stop();
    }

    public interface IAudioObserver
    {
        /// <summary>
        /// Notify when a new audio file is being tracked.
        /// </summary>
        /// <param name="audioModel">The model of global audio player.</param>
        void OnChangeAudio(AudioPlayerViewModel audioModel);
    }

    /// <summary>
    /// Interface to play audio files.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Plays an audio file.
        /// </summary>
        /// <param name="mediaViewModel">The view model with details about audio file.</param>
        void Play(object mediaViewModel);

        /// <summary>
        /// Stops the audio player.
        /// </summary>
        void Stop();

        /// <summary>
        /// Mutes the audio file.
        /// </summary>
        void Mute();
    }
}
