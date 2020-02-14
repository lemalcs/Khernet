using System.Windows;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Control to play audio files
    /// </summary>
    public partial class AudioPlayerControl : BasePopUpControl// UserControl
    {
        public AudioPlayerControl()
        {
            InitializeComponent();
        }

        private void BaseDialogUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Dispose MediaViewModel when control is unloaded
            var mediaVm = ((MediaViewModel)Application.Current.Resources["MediaVM"]);
            if (mediaVm != null)
            {
                mediaVm.Dispose();
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool playerVisible = IoC.IoCContainer.Get<ApplicationViewModel>().IsPlayerVisible;

            ////Dispose MediaViewModel when control is hidden
            if (!(bool)e.NewValue && !playerVisible)
            {
                DataContext = null;

                var mediaVm = ((MediaViewModel)Application.Current.Resources["MediaVM"]);
                if (mediaVm != null)
                {
                    mediaVm.Dispose();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Set command for play button
            btn.Command.Execute(btn.CommandParameter);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Open volume control
            popUp.IsOpen = true;
        }
    }
}
