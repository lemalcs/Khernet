using Khernet.UI.Files;
using Khernet.UI.Media;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Khernet.UI.Managers
{
    /// <summary>
    /// Provides notification to observers about media operation
    /// </summary>
    public class FileManager : IFileObservable
    {
        /// <summary>
        /// The observers queue whom listen for notifications
        /// </summary>
        private ConcurrentQueue<IFileObserver> observersList;

        /// <summary>
        /// The process for media operations request
        /// </summary>
        private Thread processor;

        /// <summary>
        /// Indicates if processor should continue runnig
        /// </summary>
        private bool stopProcessing = false;

        /// <summary>
        /// Controls when to start to process text message
        /// </summary>
        private AutoResetEvent autoReset;

        public FileManager()
        {
            autoReset = new AutoResetEvent(false);
        }

        /// <summary>
        /// Posts and request to process a media file
        /// </summary>
        /// <param name="observer">The observer that request media processing</param>
        public void ProcessFile(IFileObserver observer)
        {
            //Check if observer is null
            if (observer == null)
            {
                throw new ArgumentNullException($"{nameof(IFileObserver)} cannot be null");
            }

            //Creates the observer list
            if (observersList == null)
                observersList = new ConcurrentQueue<IFileObserver>();

            //Enqueue a media process request
            observersList.Enqueue(observer);

            //Start the processor of media files
            StartProcessor();
        }

        /// <summary>
        /// Starts the processor of media files, creates a new one if does not exists yet
        /// </summary>
        private void StartProcessor()
        {
            if (processor == null)
            {
                processor = new Thread(new ThreadStart(ProcessRequest));
                stopProcessing = false;
                processor.Start();
            }

            autoReset.Set();
        }

        /// <summary>
        /// Performs operations over media files
        /// </summary>
        private void ProcessRequest()
        {
            while (!stopProcessing)
            {
                try
                {
                    FileOperations fileOperations = new FileOperations();

                    //Check if observer queue has elements
                    while (!observersList.IsEmpty)
                    {
                        //Get the next observer
                        IFileObserver observer;
                        observersList.TryPeek(out observer);

                        try
                        {
                            //Check if an observer was retrieved
                            if (observersList.TryPeek(out observer))
                            {
                                switch (observer.Media.FileType)
                                {
                                    case MessageType.Image:
                                        fileOperations.ProcessImage(observer);

                                        break;
                                    case MessageType.GIF:
                                        fileOperations.ProcessGIF(observer);

                                        break;
                                    case MessageType.Video:
                                        fileOperations.ProcessVideo(observer);

                                        break;
                                    case MessageType.Audio:
                                        fileOperations.ProcessAudio(observer);
                                        break;
                                    default:
                                        fileOperations.ProcessFile(observer);
                                        break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            System.Diagnostics.Debugger.Break();
                            throw;
                        }
                        finally
                        {
                            observersList.TryDequeue(out observer);
                        }
                    }

                    if (observersList.IsEmpty)
                        autoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }



        /// <summary>
        /// Stops the processor of media files
        /// </summary>
        public void StopProcessor()
        {
            try
            {
                observersList = null;
                if (processor != null && processor.ThreadState != ThreadState.Unstarted)
                {
                    autoReset.Set();

                    stopProcessing = true;
                    processor.Interrupt();

                    //If thread does not stop through 1 minute, abort thread
                    if (!processor.Join(TimeSpan.FromMinutes(1)))
                        processor.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                processor = null;

                if (autoReset != null)
                    autoReset.Close();
            }
        }

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
                    StopProcessor();
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
