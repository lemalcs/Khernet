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
        private static ManualResetEvent manualReset;
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

            manualReset = new ManualResetEvent(false);
            continueMonitoring = true;

            notificationMonitor.Start();
        }

        private void CheckPendingNotifications()
        {
            try
            {
                List<Notification> notificationList = null;

                while (continueMonitoring)
                {
                    EventListener eventListener = new EventListener();
                    notificationList = eventListener.GetNotificationsDetail();
                    Notification notification = null;
                    for (int i = 0; i < notificationList.Count; i++)
                    {
                        notification = notificationList[i];
                        try
                        {

                            try
                            {
                                switch (notification.Type)
                                {
                                    case NotificationType.WritingMessage:
                                        suscriber.ProcessWritingMessage(notification.Token);
                                        break;
                                    case NotificationType.BeginSendingFile:
                                        suscriber.ProcessBeginSendingFile(notification.Token);
                                        break;
                                    case NotificationType.EndSendingFile:
                                        suscriber.ProcessEndSendingFile(notification.Token);
                                        break;
                                    case NotificationType.ReadingFile:

                                        string[] readFileDetails = notification.Content.Split('|');
                                        suscriber.ProcessReadingFile(notification.Token, readFileDetails[0], long.Parse(readFileDetails[1]));
                                        break;
                                    case NotificationType.NewMessage:
                                        int idMessage = Convert.ToInt32(notification.Content);

                                        Communicator comm = new Communicator();
                                        ConversationMessage message = comm.GetMessageDetail(idMessage);

                                        InternalConversationMessage conversation = new InternalConversationMessage();
                                        conversation.SenderToken = message.SenderToken;
                                        conversation.ReceiptToken = message.ReceiptToken;
                                        conversation.SendDate = message.SendDate;
                                        conversation.Id = idMessage;

                                        //Set notification as processed
                                        suscriber.ProcessNewMessage(conversation);

                                        break;
                                    case NotificationType.NewFile:
                                        int idFileMessage = Convert.ToInt32(notification.Content);

                                        comm = new Communicator();
                                        message = comm.GetMessageDetail(idFileMessage);


                                        InternalFileMessage fileMessage = new InternalFileMessage();
                                        fileMessage.SenderToken = message.SenderToken;
                                        fileMessage.ReceiptToken = message.ReceiptToken;

                                        FileInformation info = JSONSerializer<FileInformation>.DeSerialize(comm.GetMessageContent(idFileMessage));

                                        fileMessage.Metadata = info;
                                        fileMessage.Type = message.Type;
                                        fileMessage.UID = message.UID;

                                        fileMessage.SendDate = message.SendDate;
                                        fileMessage.Id = idFileMessage;

                                        suscriber.ProcessNewFile(fileMessage);
                                        //marcar la notificacion como procesada
                                        break;
                                    case NotificationType.MessageSent:

                                        suscriber.ProcessMessageSent(notification.Token, int.Parse(notification.Content));

                                        break;
                                }

                                eventListener.DeleteNotification(notification.Token, notification.Type);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("No client is connected.");
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    manualReset.WaitOne();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Notification Monitor aborted.");
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
                manualReset.Set();
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

        public void ProcessNewMessage(InternalConversationMessage message)
        {
            try
            {
                //Registrar el evento en la base de datos con el estado sin procesar
                suscriber.ProcessNewMessage(message);
                //No borrar el mensaje, solo dejarlo en estado PROCESADO
                //El borrado de esta notificacion se la hara cuando se marque el mensaje como leido

                //Esta llamada falla incluso si del lado del cliente puede conectarse sin problemas al servicio
            }
            catch (Exception exception)
            {
                //Implemetar guardado de log de esta excepcion
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = message.SenderToken,
                    Type = NotificationType.NewMessage,
                    Content = message.Id.ToString()
                });

                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessNewFile(InternalFileMessage fileMessage)
        {
            try
            {
                //Registrar el evento en la base de datos con el estado sin procesar
                suscriber.ProcessNewFile(fileMessage);
                System.Diagnostics.Debug.WriteLine("Processing new file " + fileMessage.Metadata.FileName);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = fileMessage.SenderToken,
                    Type = NotificationType.NewFile,
                    Content = fileMessage.Id.ToString()
                });

                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessContactChange(Notification info)
        {
            try
            {
                suscriber.ProcessContactChange(info);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                //Verify if info.Content is a enum value
                PeerState content;
                bool result = Enum.TryParse(info.Content, out content);

                //If info.Content is not a enum value get the current value
                string state = result ? ((sbyte)content).ToString() : info.Content;

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = info.Token,
                    Type = info.Type,
                    Content = state
                });

                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessBeginSendingFile(string token)
        {
            try
            {
                suscriber.ProcessBeginSendingFile(token);
            }
            catch (Exception)
            {
                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = token,
                    Type = NotificationType.BeginSendingFile,
                    Content = string.Empty
                });

                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessEndSendingFile(string token)
        {
            try
            {
                suscriber.ProcessEndSendingFile(token);
            }
            catch (Exception)
            {
                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = token,
                    Type = NotificationType.EndSendingFile,
                    Content = string.Empty
                });


                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessReadingFile(string token, string idFile, long readBytes)
        {
            try
            {
                suscriber.ProcessReadingFile(token, idFile, readBytes);
            }
            catch (Exception)
            {
                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = token,
                    Type = NotificationType.ReadingFile,
                    Content = string.Format("{0}|{1}", idFile, readBytes)
                });


                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessWritingMessage(string token)
        {
            try
            {
                suscriber.ProcessWritingMessage(token);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = token,
                    Type = NotificationType.WritingMessage,
                    Content = string.Empty
                });

                manualReset.Set();
                manualReset.Reset();
            }
        }

        public void ProcessMessageSent(string token, int idMessage)
        {
            try
            {
                suscriber.ProcessMessageSent(token, idMessage);
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);

                EventListener eventListener = new EventListener();
                eventListener.SaveNotification(new Notification
                {
                    Token = token,
                    Type = NotificationType.MessageSent,
                    Content = idMessage.ToString()
                });

                manualReset.Set();
                manualReset.Reset();
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
                    manualReset.Set();

                    if (!notificationMonitor.Join(TimeSpan.FromMinutes(2)))
                        notificationMonitor.Abort();
                }
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
            finally
            {
                if (manualReset != null)
                    manualReset.Close();
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
