using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// The type of modal dialog to show
    /// </summary>
    public enum Dialog
    {
        /// <summary>
        /// Message box dialog inside parent window
        /// </summary>
        Box,

        /// <summary>
        /// Dialog inside parent window
        /// </summary>
        Modal,

        /// <summary>
        /// Child modal for modal dialogs
        /// </summary>
        ChildModal,

        /// <summary>
        /// Dialog in new window
        /// </summary>
        NewWindow,
    }

    public class BaseDialogUserControl : UserControl
    {
        /// <summary>
        /// Controls the retrieving of result for modal dialog. Avoid close dialog before it is closed.
        /// </summary>
        TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

        /// <summary>
        /// The window that hosts this user control
        /// </summary>
        private DialogWindow dialog;

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        public RelayCommand CloseCommand { get; private set; }

        /// <summary>
        /// The type of dialog showed
        /// </summary>
        private Dialog dialogType;



        public double OwnerDialogHeight
        {
            get { return (double)GetValue(OwnerDialogHeightProperty); }
            set { SetValue(OwnerDialogHeightProperty, value); }
        }

        // The DependencyProperty as the backing store for OwnerDialogHeight.
        public static readonly DependencyProperty OwnerDialogHeightProperty =
            DependencyProperty.Register(nameof(OwnerDialogHeight), typeof(double), typeof(BaseDialogUserControl), new PropertyMetadata((double)0));


        /// <summary>
        /// default constructor
        /// </summary>
        public BaseDialogUserControl() : base()
        {
            CloseCommand = new RelayCommand(CloseDialog);
        }

        public BaseDialogUserControl(bool showInNewWindow) : base()
        {
            if (showInNewWindow)
            {
                dialog = new DialogWindow();
                //Detect if controls is in design mode
                //otherwise apply style to windows
                //This is important because there is an issue at design mode
                //with Window.
                if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                    dialog.Style = (Style)Application.Current.FindResource("WinStyle");
                dialog.DialogVM = new DialogWindowViewModel();
            }

            CloseCommand = new RelayCommand(CloseDialog);
        }

        private void CloseDialog(object obj)
        {
            //Close dialog
            switch (dialogType)
            {
                case Dialog.NewWindow:
                    dialog.Close();
                    break;

                case Dialog.Modal:
                    result.TrySetResult(true);
                    IoC.IoCContainer.Get<ApplicationViewModel>().IsModalDialogVisible = false;
                    break;

                case Dialog.Box:
                    result.TrySetResult(true);
                    IoC.IoCContainer.Get<ApplicationViewModel>().IsMessageBoxVisible = false;
                    break;

                case Dialog.ChildModal:
                    result.TrySetResult(true);
                    IoC.IoCContainer.Get<ApplicationViewModel>().IsChildDialogVisible = false;
                    break;
            }
        }

        /// <summary>
        /// Show a modal dialog message
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowMessage<T>(T viewModel, bool fullScreen = false, string backgroudnColorHex = null) where T : BaseModel
        {
            var result = new TaskCompletionSource<bool>();

            var t = TaskEx.Run(() =>
            {
                //Run on UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        //If there is not a main windows, don't show modal message.
                        //Usefull when the main window is closed and there are dialog in the dispatcher queue.
                        if (Application.Current.MainWindow == null)
                            return;

                        //Host this control within window
                        dialog.DialogVM.Content = this;

                        //Set view model for this control
                        DataContext = viewModel;

                        //Show dialog in the center of owner
                        dialog.Owner = Application.Current.MainWindow;
                        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                        //Check if windows is full screeen size
                        if (fullScreen)
                        {
                            //Do not size windows to content for full screem size
                            dialog.SizeToContent = SizeToContent.Manual;
                            dialog.WindowState = WindowState.Maximized;

                            //Windows style must be WindowStyle.None to allow transparency
                            dialog.WindowStyle = WindowStyle.None;

                            dialog.Background = Brushes.Transparent;
                            dialog.AllowsTransparency = true;

                            //Hide dimmed overlay
                            IoC.IoCContainer.Get<ApplicationViewModel>().IsOverlayVisible = false;
                        }
                        else
                            //Show dimmed overlay
                            IoC.IoCContainer.Get<ApplicationViewModel>().IsOverlayVisible = true;

                        dialogType = Dialog.NewWindow;

                        //Show the dialog
                        dialog.ShowDialog();

                        //Hide dimmed overlay
                        IoC.IoCContainer.Get<ApplicationViewModel>().IsOverlayVisible = false;
                    }
                    finally
                    {

                        result.TrySetResult(true);
                    }
                }));
            });

            return result.Task;
        }

        public Task<bool> ShowModalDialog<T>(T viewModel, bool fullScreen = false) where T : BaseModel
        {
            var t = TaskEx.Run(() =>
            {
                //Run on UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        //Set view model for this control
                        DataContext = viewModel;

                        //Modal height does not update when moving from maximized window to restore window
                        ModalDialogViewModel modalDialog = new ModalDialogViewModel();
                        modalDialog.IsFullScreen = fullScreen;
                        modalDialog.Content = this;

                        dialogType = Dialog.Modal;

                        //Show the dialog in main window
                        IoC.IoCContainer.Get<ApplicationViewModel>().ShowModalDialog(modalDialog);
                    }
                    catch (Exception)
                    {
                        result.TrySetResult(false);
                    }
                }));
            });

            return result.Task;
        }

        public Task<bool> ShowChildModalDialog<T>(T viewModel, bool fullScreen = false) where T : BaseModel
        {
            var t = TaskEx.Run(() =>
            {
                //Run on UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        //Set view model for this control
                        DataContext = viewModel;

                        //Modal height does not update when moving from maximized window to restore window
                        ModalDialogViewModel modalDialog = new ModalDialogViewModel();
                        modalDialog.IsFullScreen = fullScreen;
                        modalDialog.Content = this;

                        dialogType = Dialog.ChildModal;



                        //Show the dialog in main window
                        IoC.IoCContainer.Get<ApplicationViewModel>().ShowChildModalDialog(modalDialog);
                    }
                    catch (Exception)
                    {
                        result.TrySetResult(false);
                    }
                }));
            });

            return result.Task;
        }

        public Task<bool> ShowMessageBox<T>(T viewModel) where T : BaseModel
        {
            var t = TaskEx.Run(() =>
            {
                //Run on UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        //Set view model for this control
                        DataContext = viewModel;

                        //Modal height does not update when moving from maximized window to restore window
                        ModalDialogViewModel modalDialog = new ModalDialogViewModel();
                        modalDialog.Content = this;

                        dialogType = Dialog.Box;

                        //Show the dialog in main window
                        IoC.IoCContainer.Get<ApplicationViewModel>().ShowMessageBox(modalDialog);
                    }
                    catch (Exception)
                    {
                        result.TrySetResult(false);
                    }
                }));
            });

            return result.Task;
        }
    }
}
