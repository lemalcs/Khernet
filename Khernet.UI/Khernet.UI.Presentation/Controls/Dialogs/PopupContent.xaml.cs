using Khernet.UI.IoC;
using Khernet.UI.Managers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Container for popup controls.
    /// </summary>
    public partial class PopupContent : UserControl, ICommandSource
    {
        /// <summary>
        /// The view model for control inside popup.
        /// </summary>
        public BaseModel ViewModel
        {
            get { return (BaseModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public object CommandParameter { get; set; }

        public IInputElement CommandTarget { get; set; }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // The dependencyProperty as the backing store for MyProperty.
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PopupContent), new PropertyMetadata(default(ICommand), CommandChanged));

        /// <summary>
        /// The DependencyProperty as the backing store for ViewModel.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(BaseModel), typeof(PopupContent), new PropertyMetadata(null, null, OnPropertyUpdated));


        public PopupContent()
        {
            InitializeComponent();
        }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PopupContent pageContainer = d as PopupContent;

            if (e.OldValue != null)
            {
                pageContainer.RemoveCommand((ICommand)e.NewValue);
            }

            if (e.NewValue != null)
            {
                pageContainer.AddCommand((ICommand)e.NewValue);
            }
        }

        private void AddCommand(ICommand newCommand)
        {
            if (newCommand != null)
            {
                EventHandler handler = CanExecuteChanged;
                newCommand.CanExecuteChanged += handler;
            }
        }

        private void RemoveCommand(ICommand newCommand)
        {
            if (newCommand != null)
            {
                EventHandler handler = CanExecuteChanged;
                newCommand.CanExecuteChanged -= handler;
            }
        }

        [DebuggerStepThrough]
        private void CanExecuteChanged(object sender, EventArgs e)
        {

            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                // If a RoutedCommand.
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand.
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        this.IsEnabled = true;
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                }
            }
        }

        private void PopUp_Commited(object sender, EventArgs e)
        {
            if (this.Command != null)
            {
                RoutedCommand command = Command as RoutedCommand;

                if (command != null)
                {
                    command.Execute(CommandParameter, CommandTarget);
                }
                else
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        private static object OnPropertyUpdated(DependencyObject d, object baseValue)
        {
            var popUp = d as PopupContent;

            if (popUp != null)
            {
                //Delete current control if it is not longer useful
                if (popUp.control.Content != null && popUp.control.Content is BasePopUpControl)
                {
                    ((BasePopUpControl)popUp.control.Content).Commited -= popUp.PopUp_Commited;
                    popUp.control.Content = null;
                }

                //Shows an audio control
                if (baseValue is AudioChatMessageViewModel)
                {
                    var vm = baseValue as AudioChatMessageViewModel;

                    //Change view model is audio control has not been created or if view model has changed
                    if (popUp.control.Content == null || !popUp.GetValue(ViewModelProperty).Equals(vm))
                    {
                        //Remove previous event handler
                        if ((BasePopUpControl)popUp.control.Content != null)
                            ((BasePopUpControl)popUp.control.Content).Commited -= (d as PopupContent).PopUp_Commited;

                        //Remove previous control
                        popUp.control.Content = null;

                        //Set new control
                        AudioPlayerControl audio = new AudioPlayerControl();
                        audio.PlayerViewModel = IoCContainer.Get<IAudioObservable>().AudioModel;
                        audio.DataContext = baseValue;
                        popUp.control.Content = audio;
                    }

                }
                else if (baseValue is ReplyMessageViewModel)
                {
                    var vm = baseValue as ReplyMessageViewModel;

                    //Remove previous control
                    popUp.control.Content = null;

                    //Set new control
                    ReplyMessageControl reply = new ReplyMessageControl(vm);
                    //reply.DataContext = baseValue;
                    popUp.control.Content = reply;

                }
                else if (baseValue is FieldEditorViewModel)
                {
                    var vm = baseValue as FieldEditorViewModel;

                    //Remove previous control
                    popUp.control.Content = null;

                    //Set new control
                    FieldEditorControl reply = new FieldEditorControl(vm);

                    reply.Commited += (d as PopupContent).PopUp_Commited;


                    popUp.control.Content = reply;
                }
                else if (baseValue is CacheViewModel)
                {
                    var vm = baseValue as CacheViewModel;

                    popUp.control.Content = null;

                    CacheCleanerControl cache = new CacheCleanerControl(vm);
                    cache.Commited += (d as PopupContent).PopUp_Commited;

                    popUp.control.Content = cache;
                }
            }

            return baseValue;
        }
    }
}
