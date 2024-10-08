﻿using Hardcodet.Wpf.TaskbarNotification;
using Khernet.UI.Controls;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Khernet.UI.Resources;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Khernet.UI
{
    public class UIManager : IUIManager
    {
        /// <summary>
        /// Get a singleton instance for notification icon.
        /// </summary>
        private TaskbarIcon notificationIcon;

        /// <summary>
        /// Indicates whether tray message was showed.
        /// </summary>
        private bool trayMessageShowed = false;

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="dialogModel">The view model.</param>
        /// <returns><see cref="Task"/> for dialog.</returns>
        public async Task ShowMessageBox(MessageBoxViewModel dialogModel)
        {
            await new MessageBoxUserControl().ShowMessageBox(dialogModel);

            // Deallocate unused objects
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Shows a single message dialog into a specific parent window.
        /// </summary>
        /// <param name="dialogModel">View model for message dialog.</param>
        /// <param name="newWindow">True to show message in a new windows, false to show message into main window.</param>
        /// <returns><see cref="Task"/> for dialog.</returns>
        public async Task ShowMessageBox(MessageBoxViewModel dialogModel, bool newWindow)
        {
            if (newWindow)
                await new MessageBoxUserControl().ShowMessage(dialogModel);
            else
                await new MessageBoxUserControl().ShowMessageBox(dialogModel);

            // Deallocate unused objects
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <typeparam name="T">View model type.</typeparam>
        /// <param name="viewModel">the view model.</param>
        /// <returns><see cref="Task"/> for dialog.</returns>
        public async Task ShowDialog<T>(T viewModel) where T : BaseModel
        {
            if (viewModel is PagedDialogViewModel)
            {
                await new PagedDialogControl().ShowModalDialog(viewModel);
            }
            else if (viewModel is ImageChatMessageViewModel)
            {
                await new ImageViewerControl().ShowModalDialog(viewModel, true);
            }
            else if (viewModel is VideoChatMessageViewModel)
            {
                await new VideoPlayerControl().ShowModalDialog(viewModel, true);
            }
            else if (viewModel is AnimationChatMessageViewModel)
            {
                await new AnimationViewerControl().ShowModalDialog(viewModel, true);
            }

            // Deallocate unused objects
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Shows <see cref="OpenFileDialog"/> to choose files.
        /// </summary>
        /// <returns>List of file names.</returns>
        public string[] ShowOpenFileDialog()
        {
            //Show open file dialog 
            OpenFileDialog op = new OpenFileDialog();
            bool? result = op.ShowDialog();

            //If user clicks OK ShowDialog() returns true
            //otherwise returns false
            if (!result.Value)
                return null;

            //Return all files selected by user
            return op.FileNames;
        }

        /// <summary>
        /// Opens audio player into main window.
        /// </summary>
        /// <typeparam name="T">The type view model.</typeparam>
        /// <param name="viewModel">The view model for player.</param>
        public void ShowPlayer<T>(T viewModel) where T : BaseModel
        {
            IoCContainer.Get<ApplicationViewModel>().ShowPlayer(viewModel);
        }

        /// <summary>
        /// Open a file with the default application.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        public void OpenFile(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = fileName;

            try
            {
                Process.Start(processInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Open folder in file explorer with selected file.
        /// </summary>
        /// <param name="fileName">The path of file.</param>
        public void OpenFolderForFile(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "explorer.exe";
            processInfo.Arguments = $"/select,{fileName}";

            try
            {
                Process.Start(processInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Indicates if it is design time.
        /// </summary>
        /// <returns>True if it is design time otherwise false.</returns>
        public bool IsInDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        /// <summary>
        /// Shows a notification balloon.
        /// </summary>
        /// <param name="notificationModel">The notification model.</param>
        public void ShowNotification(NotificationViewModel notificationModel)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;

                notificationIcon.ShowCustomBalloon(new NotificationControl(notificationModel), System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                notificationIcon.HideBalloonTip();

                ShowUnreadMessagesNumber(IoCContainer.Get<UserListViewModel>().TotalUnreadMessages);

                //Show application on taskbar if it is hidden
                if (!App.Current.MainWindow.IsVisible)
                {
                    Application.Current.MainWindow.Show();
                    App.Current.MainWindow.WindowState = WindowState.Minimized;
                }

                //Flash window in task bar when no one is at foreground
                if (!IsMainWindowActive())
                {
                    IntPtr hWnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;

                    FLASHWINFO fInfo = new FLASHWINFO();
                    fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                    fInfo.hwnd = hWnd;
                    fInfo.dwFlags = (uint)(FlashWindow.FLASHW_ALL | FlashWindow.FLASHW_TIMERNOFG);
                    fInfo.uCount = 10;
                    fInfo.dwTimeout = 0;

                    NativeMethods.FlashWindowEx(ref fInfo);
                }
            }));
        }

        /// <summary>
        /// Display unread chat messages.
        /// </summary>
        /// <param name="idMessage">The id of message.</param>
        public void ShowUnReadMessage(int idMessage)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (IoCContainer.Get<UserListViewModel>().SelectedUser != null)
                {
                    IoCContainer.Get<ChatMessageListViewModel>().LoadMessage(idMessage);
                }
            }));
        }

        /// <summary>
        /// Indicates if main window is active.
        /// </summary>
        /// <returns>True if window has physical focused otherwise false.</returns>
        public bool IsMainWindowActive()
        {
            return (bool)Application.Current.Dispatcher.Invoke(new Func<bool>(() =>
            {
                return Application.Current.MainWindow.IsActive;
            }));
        }

        /// <summary>
        /// Shows windows if it is not visible.
        /// </summary>
        public void ShowWindow()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.Show();
                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
            }));
        }

        /// <summary>
        /// Execute a task on user interface thread.
        /// </summary>
        /// <param name="action">The task to be executed.</param>
        public void ExecuteAsync(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                action();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        /// <summary>
        /// Execute a task synchronously on user interface thread.
        /// </summary>
        public void Execute(Action action)
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                action();
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        /// <summary>
        /// Show an child modal dialog owned by parent modal dialog.
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A <see cref="Task"/> for dialog.</returns>
        public async Task ShowChildDialog<T>(T viewModel) where T : BaseModel
        {
            if (viewModel is PagedDialogViewModel)
            {
                await new PagedDialogControl().ShowChildModalDialog(viewModel);
            }
            else if (viewModel is ImageChatMessageViewModel)
            {
                await new ImageViewerControl().ShowChildModalDialog(viewModel, true);
            }
            else if (viewModel is VideoChatMessageViewModel)
            {
                await new VideoPlayerControl().ShowChildModalDialog(viewModel, true);
            }

            // Deallocate unused objects
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Converts a <see cref="string"/> HTML document to XAML document byte array.
        /// </summary>
        /// <param name="value">The HTML document.</param>
        /// <returns>A <see cref="byte[]"/> array with document for view.</returns>
        public byte[] ConvertHtmlToDocument(string html)
        {
            byte[] result = null;

            result = (byte[])Application.Current.Dispatcher.Invoke(new Func<string, byte[]>((h) =>
               {
                   FlowDocumentHtmlConverter documentConverter = new FlowDocumentHtmlConverter();
                   FlowDocument document = documentConverter.ConvertFromHtml(h);

                   TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
                   byte[] messageContent = new byte[0];
                   using (MemoryStream mem = new MemoryStream())
                   {
                       range.Save(mem, DataFormats.XamlPackage);
                       messageContent = mem.ToArray();
                   }

                   return messageContent;
               }), html);

            return result;
        }

        /// <summary>
        /// Converts markdown text to XAML document byte array.
        /// </summary>
        /// <param name="markdownText">The text in markdown syntax.</param>
        /// <returns>A <see cref="byte[]"/> array with document for view.</returns>
        public byte[] ConvertMarkdownToDocument(string markdownText)
        {
            byte[] result = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                FlowDocumentMarkdownConverter documentConverter = new FlowDocumentMarkdownConverter();
                FlowDocument document = documentConverter.ConvertToFlowDocument(markdownText);

                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
                byte[] messageContent = new byte[0];
                using (MemoryStream mem = new MemoryStream())
                {
                    range.Save(mem, DataFormats.XamlPackage);
                    messageContent = mem.ToArray();
                }

                result = messageContent;
            }));

            return result;
        }

        /// <summary>
        /// Shows the notification icon for application.
        /// </summary>
        public void ShowNotificationIcon()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (notificationIcon == null)
                    notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;
            }));
        }

        /// <summary>
        /// Hide the new message indicator of icon located in notification area.
        /// </summary>
        public void ClearNotificationNewMessageIcon()
        {
            if (Application.Current == null)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (notificationIcon != null)
                {
                    notificationIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/LogoIcon.ico", UriKind.RelativeOrAbsolute));

                    if (App.Current != null && App.Current.MainWindow.TaskbarItemInfo != null)
                        App.Current.MainWindow.TaskbarItemInfo = null;
                }
            }));
        }

        /// <summary>
        /// Shows the number of unread message on taskbar icon.
        /// </summary>
        /// <param name="unreadMessages">The number of unreadMessages.</param>
        public void ShowUnreadMessagesNumber(int unreadMessages)
        {
            if (unreadMessages <= 0)
            {
                ClearNotificationNewMessageIcon();
                return;
            }

            ShowTrayIconWithIndicator();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                double textLeftMargin = 12;

                if (unreadMessages >= 10)
                    textLeftMargin = 2;

                FormattedText text = new FormattedText
                (
                    unreadMessages.ToString(),//Text to render
                    new CultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface((FontFamily)App.Current.FindResource("RobotoRegularFont"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                    43, //Font size
                    (Brush)(Brush)App.Current.FindResource("LightBrush")//Foreground
                );

                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                drawingContext.DrawEllipse((Brush)App.Current.FindResource("LightRedBrush"),
                                            new Pen((Brush)App.Current.FindResource("LightRedBrush"), 0),
                                            new Point(26, 33), 28, 28);
                drawingContext.DrawText(text, new Point(textLeftMargin, 6));
                drawingContext.Close();

                RenderTargetBitmap newMessageImage = new RenderTargetBitmap(68, 68, 120, 96, PixelFormats.Pbgra32);
                newMessageImage.Render(drawingVisual);

                //Show overlay icon in taskbar with unread message count
                if (App.Current.MainWindow.TaskbarItemInfo == null)
                {
                    System.Windows.Shell.TaskbarItemInfo taskBarInfo = new System.Windows.Shell.TaskbarItemInfo();
                    App.Current.MainWindow.TaskbarItemInfo = taskBarInfo;
                }

                //Show unread messages count over taskbar icon
                App.Current.MainWindow.TaskbarItemInfo.Overlay = newMessageImage;
            }));
        }

        /// <summary>
        /// Shows the tray icon with and indicator that says there are unread messages.
        /// </summary>
        private void ShowTrayIconWithIndicator()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;
                notificationIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/newMessageIcon.ico"));
            }));
        }

        /// <summary>
        /// Show a message indicating that application is minimized to tray area.
        /// </summary>
        public void ShowTrayMessage()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (trayMessageShowed || notificationIcon == null)
                    return;

                //Do not show message if application is shutting down
                if (!((TaskbarIconViewModel)(notificationIcon.DataContext)).IsRunning)
                    return;

                notificationIcon.ShowBalloonTip("Khernet", "Application continues executing and can be restored with the icon located in system tray.", BalloonIcon.Info);

                //Show tray message just once
                trayMessageShowed = true;
            }));
        }

        public void ShowMainWindow()
        {
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Exists the whole application.
        /// </summary>
        /// <param name="exitCode">The exit code that the application will return to caller.</param>
        public void ShutDownApplication(int exitCode)
        {
            Application.Current.Shutdown(exitCode);
        }
    }
}
