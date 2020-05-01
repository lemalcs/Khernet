using Hardcodet.Wpf.TaskbarNotification;
using Khernet.UI.Controls;
using Khernet.UI.Converters;
using Khernet.UI.IoC;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
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
        /// Shows a message box
        /// </summary>
        /// <param name="dialogModel">The view model</param>
        /// <returns></returns>
        public async Task ShowMessageBox(MessageBoxViewModel dialogModel)
        {
            await new MessageBoxUserControl().ShowMessageBox(dialogModel);
        }

        /// <summary>
        /// Shows a single message dialog into a specific parent window.
        /// </summary>
        /// <param name="dialogModel">View model for message dialog</param>
        /// <param name="newWindow">True to show message in a new windows, false to show message into main window</param>
        /// <returns></returns>
        public async Task ShowMessageBox(MessageBoxViewModel dialogModel, bool newWindow)
        {
            if (newWindow)
                await new MessageBoxUserControl().ShowMessage(dialogModel);
            else
                await new MessageBoxUserControl().ShowMessageBox(dialogModel);
        }

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <typeparam name="T">View model type</typeparam>
        /// <param name="viewModel">the view model</param>
        /// <returns></returns>
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
        }

        /// <summary>
        /// Shows <see cref="OpenFileDialog"/> to choose files
        /// </summary>
        /// <returns>List of dile names</returns>
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
        /// Opens audio player into main window
        /// </summary>
        /// <typeparam name="T">The type view model</typeparam>
        /// <param name="viewModel">The view model for player</param>
        public void ShowPlayer<T>(T viewModel) where T : BaseModel
        {
            IoCContainer.Get<ApplicationViewModel>().ShowPlayer(viewModel);
        }

        /// <summary>
        /// Open a file with the defualt application.
        /// </summary>
        /// <param name="fileName">The path of file</param>
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
        /// Opens a specified chat
        /// </summary>
        /// <typeparam name="T">The type of view model</typeparam>
        /// <param name="viewModel">The view model of contact to chat</param>
        public void ShowChat<T>(T viewModel) where T : BaseModel
        {
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Indicates if it is design time
        /// </summary>
        /// <returns>True if it is design time otherwise false</returns>
        public bool IsInDesignTime()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public byte[] ConvertStringToDocument(string value)
        {
            FlowDocument fw = new FlowDocument();

            fw.Blocks.Add(new Paragraph(new Run(value)));

            TextRange range = new TextRange(fw.ContentStart, fw.ContentEnd);
            byte[] messageContent = new byte[0];
            using (MemoryStream mem = new MemoryStream())
            {
                range.Save(mem, DataFormats.XamlPackage);
                messageContent = mem.ToArray();
            }
            return messageContent;
        }

        /// <summary>
        /// Shows a notification ballon
        /// </summary>
        /// <param name="notificationModel"></param>
        public void ShowNotification(NotificationViewModel notificationModel)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var notificationIcon = App.Current.Resources["notificationIcon"] as TaskbarIcon;

                notificationIcon.ShowCustomBalloon(new NotificationControl(notificationModel), System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                notificationIcon.HideBalloonTip();
                notificationIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/newMessageIcon.ico"));

                ////Show overlay icon in taskbar with unread message count
                //if(App.Current.MainWindow.TaskbarItemInfo==null)
                //{
                //    System.Windows.Shell.TaskbarItemInfo taskBarInfo = new System.Windows.Shell.TaskbarItemInfo();
                //    App.Current.MainWindow.TaskbarItemInfo = taskBarInfo;
                //}

                //int unreadMessagesCount = IoCContainer.Get<UserListViewModel>().TotalUnreadMessages;

                //double textLeftMargin = 12;

                //if (unreadMessagesCount >= 10)
                //    textLeftMargin = 2;

                //FormattedText text = new FormattedText
                //(
                //    unreadMessagesCount.ToString(),//Text to render
                //    new CultureInfo("en-us"),
                //    FlowDirection.LeftToRight,
                //    new Typeface((FontFamily)App.Current.FindResource("RobotoRegularFont"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                //    43, //Font size
                //    (Brush)(Brush)App.Current.FindResource("LightBrush")//Foreground
                //);

                //DrawingVisual drawingVisual = new DrawingVisual();
                //DrawingContext drawingContext = drawingVisual.RenderOpen();
                //drawingContext.DrawEllipse((Brush)App.Current.FindResource("LightRedBrush"), 
                //                            new Pen((Brush)App.Current.FindResource("LightRedBrush"), 0), 
                //                            new Point(26, 33), 28, 28);
                //drawingContext.DrawText(text, new Point(textLeftMargin, 6));
                //drawingContext.Close();

                //RenderTargetBitmap newMessageImage = new RenderTargetBitmap(68, 68, 120, 96, PixelFormats.Pbgra32);
                //newMessageImage.Render(drawingVisual);

                ////Show unread messages count over taskbar icon
                //App.Current.MainWindow.TaskbarItemInfo.Overlay = newMessageImage;

                ShowUnreadMessagesNumber(IoCContainer.Get<UserListViewModel>().TotalUnreadMessages);

                //Show application on taskbar if it is hidden
                if (!App.Current.MainWindow.IsVisible)
                {
                    Application.Current.MainWindow.Activate();
                    Application.Current.MainWindow.Show();
                    App.Current.MainWindow.WindowState = WindowState.Minimized;
                }
            }));
        }

        /// <summary>
        /// Display unread chat messages
        /// </summary>
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
        /// Indicates if main window is visible or active
        /// </summary>
        public bool IsMainWindowActive()
        {
            return (bool)Application.Current.Dispatcher.Invoke(new Func<bool>(() =>
            {
                return Application.Current.MainWindow.IsActive ||
                Application.Current.MainWindow.IsFocused ||
                Application.Current.MainWindow.IsVisible;
            }));
        }

        /// <summary>
        /// Shows windows if it is not visible
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
        /// Shows system dialog to save file
        /// </summary>
        /// <returns></returns>
        public string ShowSaveFileDialog(string fileName = null)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = fileName;

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Shows system dialog to save file
        /// </summary>
        /// <returns></returns>
        public string ShowSaveFileDialog(string filter, string defaultExtension)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = filter;
            saveDialog.DefaultExt = defaultExtension;
            saveDialog.AddExtension = false;

            if (saveDialog.ShowDialog().Value)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Execute a task on user interface thread
        /// </summary>
        /// <param name="action">The task to be executed</param>
        public void ExecuteAsync(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                action();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        /// <summary>
        /// Execute a task synchronously on user interface thread
        /// </summary>
        public void Execute(Action action)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                action();
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        /// <summary>
        /// Show an child modal dialog owned by parent modial dialog
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
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
        }

        /// <summary>
        /// Converts a <see cref="string"/> html document to XAML document byte array
        /// </summary>
        /// <param name="value">The html document</param>
        /// <returns>A <see cref="byte[]"/> array</returns>
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
        /// Converts markdown text to XAML document byte array
        /// </summary>
        /// <param name="markdownText"></param>
        /// <returns></returns>
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
        /// Hide the new message indicator of icon located in notification area
        /// </summary>
        public void ClearNotificationNewMessageIcon()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (notificationIcon != null)
                {
                    notificationIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/LogoIcon.ico", UriKind.RelativeOrAbsolute));

                    if (App.Current.MainWindow.TaskbarItemInfo != null)
                        App.Current.MainWindow.TaskbarItemInfo = null;
                }
            }));
        }

        /// <summary>
        /// Shows the number of unread message on taskbar icon.
        /// </summary>
        /// <param name="unreadMessages">The number of unreadMessages</param>
        public void ShowUnreadMessagesNumber(int unreadMessages)
        {
            if (unreadMessages <= 0)
            {
                ClearNotificationNewMessageIcon();
                return;
            }

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
    }
}
