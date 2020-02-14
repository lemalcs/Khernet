using Hardcodet.Wpf.TaskbarNotification;
using Khernet.UI.Controls;
using Khernet.UI.IoC;
using System.Windows;

namespace Khernet.UI.Managers
{
    public class PresentationApplicationNotification : IApplicationNotification
    {
        public void ShowNotification(NotificationViewModel notificationModel)
        {
            Application.Current.Dispatcher.Invoke(new DoAction(() =>
            {
                var notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;

                notificationIcon.ShowCustomBalloon(new NotificationControl(notificationModel), System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                notificationIcon.HideBalloonTip();
            }));
        }
    }
}
