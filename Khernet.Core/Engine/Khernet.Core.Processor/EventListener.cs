using Khernet.Core.Data;
using Khernet.Core.Processor.IoC;
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

        public Dictionary<string, short> GetNotificationsList()
        {
            EventListenerData eventData = new EventListenerData();

            DataTable data = eventData.GetNotificationsList();
            Dictionary<string, short> notificationList = new Dictionary<string, short>();

            for (int i = 0; i < data.Rows.Count; i++)
            {
                notificationList.Add(data.Rows[i][0].ToString(), Convert.ToInt16(data.Rows[i][1]));
            }

            return notificationList;
        }

        public byte[] GetNotificationDetail(string id)
        {
            EventListenerData eventData = new EventListenerData();

            DataTable data = eventData.GetNotificationDetail(id);
            byte[] notification = null;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                notification = data.Rows[i][0] as byte[];
            }

            return notification;
        }

        public void DeleteNotification(string id)
        {
            EventListenerData.DeleteNotification(id);
        }

        public void SaveNotification(string id, NotificationType type, byte[] content)
        {
            EventListenerData eventData = new EventListenerData();
            eventData.SaveNotification(id, (short)type, content);
        }

        public void Suscribe(string listenerKey)
        {
            IoCContainer.Get<NotificationManager>().Suscribe(listenerKey);
        }


        public void UnSuscribe(string listenerKey)
        {

            IoCContainer.Get<NotificationManager>().UnSuscribe(listenerKey);
        }

        public void ProcessNewMessage(MessageNotification notification)
        {
            IoCContainer.Get<NotificationManager>().ProcessNewMessage(notification);
        }

        public void ProcessContactChange(PeerNotification notification)
        {
            IoCContainer.Get<NotificationManager>().ProcessContactChange(notification);
        }

        public void ProcessMessageProcessing(MessageProcessingNotification notification)
        {
            IoCContainer.Get<NotificationManager>().ProcessMessageProcessing(notification);
        }

        public void ProcessMessageStateChanged(MessageStateNotification notification)
        {
            IoCContainer.Get<NotificationManager>().ProcessMessageStateChanged(notification);
        }
    }
}
