using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Wpf;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para ImageViewerControl.xaml
    /// </summary>
    public partial class AudioMessageControl : UserControl, IAudioObserver
    {
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
            //Suscribe to event for notification when media changes
            //this allow to open the AudioPlayerControl to play audio
            //or to control the audio file when it is playing
            IoCContainer.Get<IAudioObservable>().Suscribe(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Virtualization of chat list creates new objets or reuses them so it is necessary 
            //to check if current playing audio is the same as this control's datacontext.
            IoCContainer.Get<IAudioObservable>().Suscribe(this);

            OnChangeAudio(IoCContainer.Get<IAudioObservable>().AudioModel);
        }

        public void OnChangeAudio(AudioPlayerViewModel audioModel)
        {
            if (audioModel.CurrentViewModel != null && audioModel.CurrentViewModel.Equals(DataContext))
            {
                //Add binding to AudioPlaerControl
                CurrentPlayer = audioModel.Player;

                //Set command to control audio playing
                playButton.Command = audioModel.PlayCommand;
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            IoCContainer.Get<IAudioObservable>().Unsuscribe(this);

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
