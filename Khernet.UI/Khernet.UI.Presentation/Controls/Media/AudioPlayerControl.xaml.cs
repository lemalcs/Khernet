using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System.Windows;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Control to play audio fileswith a popup style
    /// </summary>
    public partial class AudioPlayerControl : BasePopUpControl
    {

        public AudioPlayerViewModel PlayerViewModel
        {
            get { return (AudioPlayerViewModel)GetValue(AudioPLayerViewModelProperty); }
            set { SetValue(AudioPLayerViewModelProperty, value); }
        }

        // The DependencyProperty as the backing store for AudioPLayerViewModel.
        public static readonly DependencyProperty AudioPLayerViewModelProperty =
            DependencyProperty.Register(nameof(PlayerViewModel), typeof(AudioPlayerViewModel), typeof(AudioPlayerControl), new PropertyMetadata(null));

        public AudioPlayerControl()
        {
            InitializeComponent();
        }

        private void BaseDialogUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Dispose MediaViewModel when control is unloaded
            IoCContainer.Get<IAudioObservable>().StopPlayer();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool playerVisible = IoCContainer.Get<ApplicationViewModel>().IsPlayerVisible;

            ////Dispose MediaViewModel when control is hidden
            if (!(bool)e.NewValue && !playerVisible)
            {
                DataContext = null;
                IoCContainer.Get<IAudioObservable>().StopPlayer();
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
