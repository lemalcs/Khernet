using Khernet.Core.Common;
using Khernet.Core.Utility;
using Khernet.Services.Contracts;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;

namespace Khernet.Core.Processor.Managers
{
    public class NotificationManager : IDisposable
    {
        private static IEventListenerCallBack suscriber;
        private static Thread notificationMonitor;
        private static AutoResetEvent autoReset;
        private static volatile bool continueMonitoring = false;

        static NotificationManager()
        {
            //Delete pendding notifications
            EventListener eventData = new EventListener();
            eventData.ClearNotifications();
        }

        public void Start()
        {
            notificationMonitor = new Thread(CheckPendingNotifications);
            notificationMonitor.Name = "NotificationMonitor";

            autoReset = new AutoResetEvent(false);
            continueMonitoring = true;

            notificationMonitor.Start();
        }

        private void CheckPendingNotifications()
        {
            try
            {
                Dictionary<string, short> notificationList = null;

                while (continueMonitoring)
                {
                    if (suscriber == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    try
                    {
                        EventListener eventListener = new EventListener();
                        notificationList = eventListener.GetNotificationsDetail();
                        string notificationId = null;

                        foreach (var key in notificationList.Keys)
                        {
                            notificationId = key;

                            var notificationDetail = eventListener.GetNotificationDetail(notificationId);
                            NotificationType notificationType = (NotificationType)notificationList[key];

                            switch (notificationType)
                            {
                                case NotificationType.MessageProcessingChange:
                                    suscriber.ProcessMessageProcessing(JSONSerializer<MessageProcessingNotification>.DeSerialize(notificationDetail));
                                    break;
                                case NotificationType.NewMessage:
                                    suscriber.ProcessNewMessage(JSONSerializer<MessageNotification>.DeSerialize(notificationDetail));
                                    break;

                                case NotificationType.MessageChange:
                                    suscriber.ProcessMessageStateChanged(JSONSerializer<MessageStateNotification>.DeSerialize(notificationDetail));
                                    break;
                            }

                            eventListener.DeleteNotification(key);
                        }
                    }
                    catch (Exception error)
                    {
                        LogDumper.WriteLog(error);
                    }
                    autoReset.WaitOne();
                }
            }
            catch (ThreadAbortException exception)
            {
                LogDumper.WriteLog(exception);
                return;
            }
            catch (ThreadInterruptedException exception)
            {
                LogDumper.WriteLog(exception);
                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Notification Monitor stopped.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Suscribe(string listenerKey)
        {

            if (suscriber != null)
            {
                ICommunicationObject suscriberClient = (ICommunicationObject)suscriber;
                if (suscriberClient.State == CommunicationState.Closed || suscriberClient.State == CommunicationState.Faulted)
                {
                    AddApplicationClient(listenerKey);
                    return;
                }
            }

            if (suscriber == null)
            {
                AddApplicationClient(listenerKey);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UnSuscribe(string listenerKey)
        {

            string tempListenerKey = Configuration.GetValue(Constants.ListenerKey);

            CryptographyProvider crypto = new CryptographyProvider();
            try
            {
                byte[] decodedKey = crypto.DecodeBase58Check(tempListenerKey);
            }
            catch (Exception)
            {

                throw new Exception("Invalid key"); ;
            }

            if (tempListenerKey == listenerKey)
            {
                suscriber = null;
                continueMonitoring = false;

                if (!autoReset.SafeWaitHandle.IsClosed)
                    autoReset.Set();
            }
            else
                throw new Exception("Invalid key");
        }

        private static void AddApplicationClient(string listenerKey)
        {
            string tempListenerKey = Configuration.GetValue(Constants.ListenerKey);

            CryptographyProvider crypto = new CryptographyProvider();
            try
            {
                byte[] decodedKey = crypto.DecodeBase58Check(tempListenerKey);
            }
            catch (Exception)
            {

                throw new Exception("Invalid key"); ;
            }

            if (tempListenerKey == listenerKey)
            {
                suscriber = OperationContext.Current.GetCallbackChannel<IEventListenerCallBack>();
            }
            else
                throw new Exception("Invalid key");
        }

        public void ProcessNewMessage(MessageNotification notification)
        {
            try
            {
                //Calls to this method can fail even if client can connet to this service
                suscriber.ProcessNewMessage(/*message*/notification);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                //Save notification on database if this could not be sent to client
                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(
                    Guid.NewGuid().ToString().Replace("-", ""),
                    NotificationType.NewMessage,
                    JSONSerializer<MessageNotification>.Serialize(notification));

                if (!autoReset.SafeWaitHandle.IsClosed)
                    autoReset.Set();
            }
        }

        public void ProcessContactChange(PeerNotification notification)
        {
            try
            {
                suscriber.ProcessContactChange(notification);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(
                    Guid.NewGuid().ToString().Replace("-", ""),
                    NotificationType.PeerChange,
                    JSONSerializer<PeerNotification>.Serialize(notification));

                if (!autoReset.SafeWaitHandle.IsClosed)
                    autoReset.Set();
            }
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            try
            {
                suscriber.ProcessMessageProcessing(notification);
            }
            catch (Exception)
            {
                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(
                    Guid.NewGuid().ToString().Replace("-", ""),
                    NotificationType.MessageProcessingChange,
                    JSONSerializer<MessageProcessingNotification>.Serialize(notification));

                if (!autoReset.SafeWaitHandle.IsClosed)
                    autoReset.Set();
            }
        }

        public void ProcessMessageStateChanged(MessageStateNotification notification)
        {
            try
            {
                suscriber.ProcessMessageStateChanged(notification);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(
                    Guid.NewGuid().ToString().Replace("-", ""),
                    NotificationType.MessageChange,
                    JSONSerializer<MessageStateNotification>.Serialize(notification));

                if (!autoReset.SafeWaitHandle.IsClosed)
                    autoReset.Set();
            }
        }


        public void Stop()
        {
            try
            {
                continueMonitoring = false;
                if (notificationMonitor != null && notificationMonitor.ThreadState != ThreadState.Unstarted)
                {
                    notificationMonitor.Interrupt();
                    autoReset.Set();
                    notificationMonitor.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
            finally
            {
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
                    Stop();
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
