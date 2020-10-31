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
            IoCContainer.Configure<PendingMessageManager>();
            IoCContainer.Get<PendingMessageManager>().Start();
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
            IoCContainer.UnSet<NotificationManager>();
        }

        public static void StopMessageManager()
        {
            IoCContainer.Get<PendingMessageManager>().Stop();
            IoCContainer.UnSet<PendingMessageManager>();
        }

        public static void StopFileManager()
        {
            IoCContainer.Get<FileManager>().Stop();
            IoCContainer.UnSet<FileManager>();
        }

        public static void StopTextMessageManager()
        {
            IoCContainer.Get<TextMessageManager>().Stop();
            IoCContainer.UnSet<TextMessageManager>();
        }
    }
}
