using Khernet.UI.Converters;
using Khernet.UI.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Lógica de interacción para PageContainer.xaml
    /// </summary>
    public partial class PageContainer : UserControl, ICommandSource
    {

        /// <summary>
        /// Indicates the current page that is shown
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get { return (ApplicationPage)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // The dependencyProperty as the backing store for CurrentPage
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(ApplicationPage), typeof(PageContainer), new UIPropertyMetadata(default(ApplicationPage), null, OnPropertyChanged));


        /// <summary>
        /// The current view model for page
        /// </summary>
        public BaseModel CurrentViewModel
        {
            get { return (BaseModel)GetValue(CurrentViewModelProperty); }
            set { SetValue(CurrentViewModelProperty, value); }
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
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PageContainer), new PropertyMetadata(default(ICommand), CommandChanged));

        // The dependencyProperty as the backing store for CurrentViewModel. 
        public static readonly DependencyProperty CurrentViewModelProperty =
             DependencyProperty.Register(nameof(CurrentViewModel), typeof(BaseModel), typeof(PageContainer), new PropertyMetadata(null));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageContainer pageContainer = d as PageContainer;

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

        private static object OnPropertyChanged(DependencyObject d, object baseValue)
        {
            //Get frames
            var oldFrame = (d as PageContainer).OldFrame;
            var newFrame = (d as PageContainer).NewFrame;

            //Get current content
            var oldContent = newFrame.Content as BasePage;

            if (oldContent != null)
                oldContent.Commited -= (d as PageContainer).Page_Commited;

            var currentPage = (ApplicationPage)d.GetValue(CurrentPageProperty);
            var currentViewModel = d.GetValue(CurrentViewModelProperty);

            var page = newFrame.Content as BasePage;

            if (newFrame.Content != null && ((ApplicationPage)baseValue) == currentPage)
            {
                page.ViewModel = currentViewModel;
                return baseValue;
            }

            newFrame.Content = null;

            page = ((ApplicationPage)baseValue).ToBasePage(currentViewModel);
            page.Commited += (d as PageContainer).Page_Commited;

            newFrame.Content = page;

            return baseValue;
        }

        private void Page_Commited(object sender, EventArgs e)
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

        public PageContainer()
        {
            InitializeComponent();
        }
    }
}
