using Khernet.Core.Processor.IoC;
using Khernet.Core.Processor.Managers;

namespace Khernet.Core.Processor
{
    public static class Manager
    {
        public static void StartNotificationManager()
        {
            IoCContainer.Configure<NotificationManager>();
            IoCContainer.Get<NotificationManager>().Start();
        }

        public static void StartMessageManager()
        {
            IoCContainer.Configure<MessageManager>();
            IoCContainer.Get<MessageManager>().Start();
        }

        public static void StartFileManager()
        {
            IoCContainer.Configure<FileManager>();
            IoCContainer.Get<FileManager>().Start();
        }

        public static void StartTextMessageManager()
        {
            IoCContainer.Configure<TextMessageManager>();
            IoCContainer.Get<TextMessageManager>().Start();
        }

        public static void StopNotificationManager()
        {
            IoCContainer.Get<NotificationManager>().Stop();
            IoCContainer.UnConfigure<NotificationManager>();
        }

        public static void StopMessageManager()
        {
            IoCContainer.Get<MessageManager>().Stop();
            IoCContainer.UnConfigure<MessageManager>();
        }

        public static void StopFileManager()
        {
            IoCContainer.Get<FileManager>().Stop();
            IoCContainer.UnConfigure<FileManager>();
        }

        public static void StopTextMessageManager()
        {
            IoCContainer.Get<TextMessageManager>().Stop();
            IoCContainer.UnConfigure<TextMessageManager>();
        }
    }
}
