using Khernet.UI.IoC;
using System;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides an mechanism to get notifications about media processing.
    /// </summary>
    public interface IFileObserver
    {
        /// <summary>
        /// The media to process.
        /// </summary>
        MediaRequest Media { get; set; }

        /// <summary>
        /// Notifies when media metadata was read.
        /// </summary>
        /// <param name="info">The metadata of media.</param>
        void OnGetMetadata(FileResponse info);

        /// <summary>
        /// Notifies about the current progress of media processing.
        /// </summary>
        /// <param name="operation">Information about current progress.</param>
        void OnProcessing(long bytesProcessed);

        /// <summary>
        /// Notifies when an operation has ended.
        /// </summary>
        /// <param name="operationType">The type of completed operation.</param>
        void OnCompleted(ChatMessageProcessResult result);

        /// <summary>
        /// Notifies when an error occurs.
        /// </summary>
        /// <param name="exception">The error detail.</param>
        void OnError(Exception exception);
    }

    /// <summary>
    /// Provides notification to observers about media operation.
    /// </summary>
    public interface IFileObservable : IDisposable
    {
        /// <summary>
        /// Process the media file to be sent or received.
        /// </summary>
        /// <param name="observer">The observer to track operations on files.</param>
        void ProcessFile(IFileObserver observer);
    }


    /// <summary>
    /// Provides an mechanism to get notifications about media processing.
    /// </summary>
    public interface ITextObserver
    {
        /// <summary>
        /// The text message to process.
        /// </summary>
        TextRequest Text { get; set; }

        /// <summary>
        /// Notifies when an operation has ended.
        /// </summary>
        /// <param name="operationType">The type of completed operation.</param>
        void OnGetMetadata(TextResponse info);

        /// <summary>
        /// Notifies when an operation has ended.
        /// </summary>
        /// <param name="operationType">The type of completed operation.</param>
        void OnCompleted(ChatMessageProcessResult result);

        /// <summary>
        /// Notifies when an error occurs.
        /// </summary>
        /// <param name="exception">The error detail.</param>
        void OnError(Exception exception);
    }

    /// <summary>
    /// Provides notification to observers about media operation.
    /// </summary>
    public interface ITextObservable : IDisposable
    {
        /// <summary>
        /// Process the media file to be sent or received.
        /// </summary>
        /// <param name="observer">The observer to track operations on files.</param>
        void ProcessText(ITextObserver observer);
    }


    public interface IMessageEventObserver : IIdentity
    {
        /// <summary>
        /// Notifies when a message event has arrived.
        /// </summary>
        /// <param name="operationType">The event data.</param>
        void OnUpdate(MessageEventData info);
    }

    public interface IMessageEventObservable : IDisposable
    {
        /// <summary>
        /// Subscribe a listener of message events.
        /// </summary>
        /// <param name="eventObserver">The listener to subscribe to.</param>
        void Subscribe(IMessageEventObserver eventObserver);

        /// <summary>
        /// Unsubscribe the listener of message events.
        /// </summary>
        /// <param name="eventObserver">The listener to unsubscribe to.</param>
        void Unsubscribe(IMessageEventObserver eventObserver);
    }
}
