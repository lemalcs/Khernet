using System;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Wpf;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para ImageViewerControl.xaml
    /// </summary>
    public partial class AudioMessageControl : UserControl
    {
        /// <summary>
        /// Indicates it this control is suscriber to <see cref="MediaViewModel"/> events
        /// </summary>
        private bool suscribedToMediaChaged = false;

        /// <summary>
        /// Gets or set the current <see cref="VlcControl"/> for playing audio files
        /// </summary>
        public VlcControl CurrentPlayer
        {
            get { return (VlcControl)GetValue(CurrentPlayerProperty); }
            set { SetValue(CurrentPlayerProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty as the backing store for CurrentPlayer.
        /// </summary>
        public static readonly DependencyProperty CurrentPlayerProperty =
            DependencyProperty.Register(nameof(CurrentPlayer), typeof(VlcControl), typeof(AudioMessageControl), new PropertyMetadata(null, null, OnPropertyUpdated));

        private static object OnPropertyUpdated(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        public AudioMessageControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mediaVm = ((MediaViewModel)Application.Current.Resources["MediaVM"]);
            if (mediaVm != null && !suscribedToMediaChaged)
            {
                //Suscribe to event for notification when media changes
                //this allow to open the AudioPlayerControl to play audio
                //or to control the audio file when it is playing
                mediaVm.MediaChanged += MediaVm_MediaChanged;
                suscribedToMediaChaged = true;
            }
        }

        private void MediaVm_MediaChanged(object sender, EventArgs e)
        {
            var mediaVm = sender as MediaViewModel;

            if (mediaVm.CurrentViewModel != null && mediaVm.CurrentViewModel.Equals(DataContext))
            {
                //Add binding to AudioPlaerControl
                CurrentPlayer = mediaVm.Player;

                //Set command to control audio playing
                playButton.Command = mediaVm.PlayCommand;
            }
            else
            {
                //Remove binding to AudioPlayerControl
                CurrentPlayer = null;

                //Check if DataContext has not been disconnected
                if (DataContext is AudioChatMessageViewModel)
                {
                    //Restore the command to open audio file
                    playButton.Command = ((AudioChatMessageViewModel)DataContext).OpenMediaCommand;
                }

            }
        }
    }
}
