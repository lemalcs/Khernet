using System;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides an mechanism to get notifications about media processing.
    /// </summary>
    public interface IFileObserver
    {
        /// <summary>
        /// The media to process
        /// </summary>
        MediaRequest Media { get; set; }

        /// <summary>
        /// Notifies when media medtadata was read.
        /// </summary>
        /// <param name="info">The metadata of media</param>
        void OnGetMetadata(FileResponse info);

        /// <summary>
        /// Notifies about the current progress of media processing
        /// </summary>
        /// <param name="operation">Information about current progress</param>
        void OnProcessing(long bytesProcessed);

        /// <summary>
        /// Notifies when an operation has ended
        /// </summary>
        /// <param name="operationType">The type of completed operation</param>
        void OnCompleted(ChatMessageProcessResult result);

        /// <summary>
        /// Notifies when an error ocurrs.
        /// </summary>
        /// <param name="exception">The error detail</param>
        void OnError(Exception exception);
    }

    /// <summary>
    /// Provides notification to observers about media operation
    /// </summary>
    public interface IFileObservable : IDisposable
    {
        /// <summary>
        /// Process the media file to be sent or received
        /// </summary>
        /// <param name="observer"></param>
        void ProcessFile(IFileObserver observer);
    }


    /// <summary>
    /// Provides an mechanism to get notifications about media processing.
    /// </summary>
    public interface ITextObserver
    {
        /// <summary>
        /// The text message to process
        /// </summary>
        TextRequest Text { get; set; }

        /// <summary>
        /// Notifies when an operation has ended
        /// </summary>
        /// <param name="operationType">The type of completed operation</param>
        void OnGetMetadata(TextResponse info);

        /// <summary>
        /// Notifies when an operation has ended
        /// </summary>
        /// <param name="operationType">The type of completed operation</param>
        void OnCompleted(ChatMessageProcessResult result);

        /// <summary>
        /// Notifies when an error ocurrs.
        /// </summary>
        /// <param name="exception">The error detail</param>
        void OnError(Exception exception);
    }

    /// <summary>
    /// Provides notification to observers about media operation
    /// </summary>
    public interface ITextObservable : IDisposable
    {
        /// <summary>
        /// Process the media file to be sent or received
        /// </summary>
        /// <param name="observer"></param>
        void ProcessText(ITextObserver observer);
    }
}
