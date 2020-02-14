using Khernet.Core.Data;
using Khernet.Core.Host.IoC;
using Khernet.Core.Processor.Managers;
using Khernet.Services.Messages;
using System;
using System.Collections.Generic;
using System.Data;

namespace Khernet.Core.Processor
{
    public class EventListener
    {
        public void ClearNotifications()
        {
            EventListenerData eventData = new EventListenerData();
            eventData.ClearNotifications();
        }

        public List<Notification> GetNotificationsDetail()
        {
            EventListenerData eventData = new EventListenerData();

            DataTable data = eventData.GetNotificationsDetail();
            List<Notification> notificationList = new List<Notification>();

            for (int i = 0; i < data.Rows.Count; i++)
            {
                Notification notification = new Notification
                {
                    Token = data.Rows[i][0].ToString(),
                    Content = data.Rows[i][2].ToString(),
                    Type = (NotificationType)Enum.Parse(typeof(NotificationType), data.Rows[i][1].ToString())
                };

                notificationList.Add(notification);
            }

            return notificationList;
        }

        public void DeleteNotification(string token, NotificationType type)
        {
            EventListenerData.DeleteNotification(token, (byte)type);
        }

        public void SaveNotification(Notification notification)
        {
            EventListenerData eventData = new EventListenerData();
            eventData.SaveNotification(notification.Token, (byte)notification.Type, notification.Content);
        }

        public void Suscribe(string listenerKey)
        {
            IoCContainer.Get<NotificationManager>().Suscribe(listenerKey);
        }


        public void UnSuscribe(string listenerKey)
        {

            IoCContainer.Get<NotificationManager>().UnSuscribe(listenerKey);
        }

        public void ProcessNewMessage(InternalConversationMessage message)
        {
            IoCContainer.Get<NotificationManager>().ProcessNewMessage(message);
        }

        public void ProcessNewFile(InternalFileMessage fileMessage)
        {
            IoCContainer.Get<NotificationManager>().ProcessNewFile(fileMessage);
        }

        public void ProcessContactChange(Notification info)
        {
            IoCContainer.Get<NotificationManager>().ProcessContactChange(info);
        }

        public void ProcessBeginSendingFile(string token)
        {
            IoCContainer.Get<NotificationManager>().ProcessBeginSendingFile(token);
        }

        public void ProcessEndSendingFile(string token)
        {
            IoCContainer.Get<NotificationManager>().ProcessEndSendingFile(token);
        }

        public void ProcessReadingFile(string token, string idFile, long readBytes)
        {
            IoCContainer.Get<NotificationManager>().ProcessReadingFile(token, idFile, readBytes);
        }

        public void ProcessWritingMessage(string token)
        {
            IoCContainer.Get<NotificationManager>().ProcessWritingMessage(token);
        }

        public void ProcessMessageSent(string token, int idMessage)
        {
            IoCContainer.Get<NotificationManager>().ProcessMessageSent(token, idMessage);
        }
    }
}
