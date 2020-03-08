using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Control to play audio files within a popup dialog.
    /// </summary>
    public partial class AudioPlayerControl : BasePopUpControl
    {
        /// <summary>
        /// Timer to control when video controls hide.
        /// </summary>
        DispatcherTimer timer;

        /// <summary>
        /// Model for global audio player.
        /// </summary>
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
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                StopTimer();
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool playerVisible = IoCContainer.Get<ApplicationViewModel>().IsPlayerVisible;

            //Dispose resources when control is hidden
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

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StartTimer();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Close volume control
            popUp.IsOpen = false;

            StopTimer();
        }

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                timer.Tick += Timer_Tick;
            }

            timer.IsEnabled = true;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.IsEnabled = false;
                timer.Stop();
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StopTimer();

            //Open volume control
            popUp.IsOpen = true;
            popUp.Focus();

            //Create timer
            if (timer == null)
            {
                timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                timer.Tick += Timer_Tick;
            }
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (timer != null)
            {
                StartTimer();
            }
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StopTimer();
        }
    }
}
