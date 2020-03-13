using Khernet.UI.IoC;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// List of chat messages.
    /// </summary>
    public partial class ChatMessageListControl : UserControl
    {
        /// <summary>
        /// Indicates if loading of messages is allowed
        /// </summary>
        bool allowLoadMessages = false;

        private bool scrollingToEnd = false;

        /// <summary>
        /// Indicates if the backing of vertical offset is allowed
        /// </summary>
        bool allowScroll = false;

        /// <summary>
        /// Indicates if there is a request to make an item visible in chat list
        /// </summary>
        bool requestingBringIntoView = false;

        /// <summary>
        /// Indicates if it is the first time this control is loaded so it does not have any messages
        /// </summary>
        bool isFirstLoad = false;

        public ChatMessageListControl()
        {
            InitializeComponent();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            //Show surface when mouse enters this control
            dropSurface.Visibility = Visibility.Visible;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);

            //Hide and collapse surface when mouse leaves from this control
            dropSurface.Visibility = Visibility.Collapsed;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            //Hide and collapse surface when data is dropped on this control
            dropSurface.Visibility = Visibility.Collapsed;
        }

        public void ScrollToEnd()
        {
            scrollingToEnd = true;

            var scrollControl = GetScrollViewer(container as DependencyObject);

            if (scrollControl == null)
                return;

            if (IoCContainer.Get<ChatMessageListViewModel>().Items == null)
                return;

            int count = IoCContainer.Get<ChatMessageListViewModel>().Items.Count;

            if (count == 0)
                return;

            requestingBringIntoView = true;

            var panel = (VirtualizingStackPanelEx)GetPanel(container);

            if (!panel.IsLoaded)
                return;

            panel.BringIntoViewPublic(count - 1);

            TreeViewItem i = null;

            Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object unused)
            {
                i = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(count - 1);
                return null;
            }, null);

            if (i != null)
            {
                i.IsSelected = true;
                i.BringIntoView();
            }
            else
            {
                scrollControl.ScrollToEnd();
                scrollControl.ScrollToBottom();
                scrollControl.ScrollToVerticalOffset(scrollControl.ScrollableHeight - scrollControl.ViewportHeight);
            }

            requestingBringIntoView = false;

            scrollingToEnd = false;
        }

        public void ScrollToItem(ChatMessageItemViewModel messageModel)
        {
            if (messageModel == null)
                return;

            var scrollControl = GetScrollViewer(container as DependencyObject);

            if (scrollControl == null)
                return;

            if (IoCContainer.Get<ChatMessageListViewModel>().Items == null)
                return;

            int count = IoCContainer.Get<ChatMessageListViewModel>().Items.IndexOf(messageModel);

            if (count <= 0)
                return;

            if (!container.HasItems)
                return;

            var panel = (VirtualizingStackPanelEx)GetPanel(container);

            panel.BringIntoViewPublic(count);

            TreeViewItem i = null;

            //Determine if the calling thread has access to the thread the TreeView is on
            if (container.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object unused)
                           {
                               i = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(count);
                               var i2 = (TreeViewItem)container.ItemContainerGenerator.ContainerFromItem(messageModel);
                               return null;
                           }, null);

                if (i != null)
                {
                    i.IsSelected = true;
                    i.BringIntoView();
                    i.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));

                }
            }

            requestingBringIntoView = true;
        }

        private ScrollViewer GetScrollViewer(DependencyObject hostControl)
        {
            ScrollViewer scroll = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(hostControl as DependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(hostControl as DependencyObject, i);
                if (child is ScrollViewer)
                {
                    scroll = child as ScrollViewer;
                    break;
                }
                else
                {
                    scroll = GetScrollViewer(child);
                    if (scroll is ScrollViewer)
                        break;
                }

            }
            return scroll;
        }

        private VirtualizingStackPanel GetPanel(DependencyObject hostControl)
        {
            VirtualizingStackPanel scroll = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(hostControl as DependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(hostControl as DependencyObject, i);
                if (child is VirtualizingStackPanel)
                {
                    scroll = child as VirtualizingStackPanel;
                    break;
                }
                else
                {
                    scroll = GetPanel(child);
                    if (scroll is VirtualizingStackPanel)
                        break;
                }

            }
            return scroll;
        }

        private void Container_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Scroll to up
            if (e.Delta > 0)
            {
                var scroll = GetScrollViewer(sender as DependencyObject);

                if (scroll.VerticalOffset == 0)
                {
                    IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(false);
                }
            }
            allowScroll = true;
        }

        private void container_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollControl = GetScrollViewer(container as DependencyObject);
            var panel = GetPanel(scrollControl);

            var hitTest = VisualTreeHelper.HitTest(panel, new Point(1, e.ViewportHeight));

            if (hitTest != null)
            {
                IoCContainer.Get<ChatMessageListViewModel>().SetCurrentMessage((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext);
                if (allowScroll)
                    IoCContainer.Get<ChatMessageListViewModel>().SetCurrentChatModel((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext);
            }

            if (e.VerticalOffset == 0 && e.VerticalChange < 0)
            {
                if (allowLoadMessages)
                {
                    IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(false);
                }
                allowLoadMessages = true;
            }
            else if (!scrollingToEnd)
            {
                //Detect if scroll bar is at bottom of list
                double scrollDifference = Math.Abs(e.ExtentHeight - e.VerticalOffset - e.ViewportHeight);

                if (scrollDifference >= 0 && scrollDifference <= 1)
                    IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(true);
            }
            if (!allowScroll)
            {
                if (!requestingBringIntoView)
                {
                    var messageModel = IoCContainer.Get<ChatMessageListViewModel>().GetCurrentChatModel();

                    ScrollToItem(messageModel);
                }
            }
        }

        private void container_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                var scroll = GetScrollViewer(sender as DependencyObject);

                if (scroll.VerticalOffset == 0)
                {
                    IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(false);
                }
            }

            if (e.Key == Key.PageUp || e.Key == Key.PageDown)
                allowScroll = true;
        }

        private void container_TargetUpdated(object sender, DataTransferEventArgs e)
        {

            var scrollControl = GetScrollViewer(sender as DependencyObject);

            if (scrollControl == null)
            {
                isFirstLoad = true;
                return;
            }

            allowLoadMessages = false;
            allowScroll = false;
            IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }

        private void container_Loaded(object sender, RoutedEventArgs e)
        {
            if (isFirstLoad)
            {
                var scrollControl = GetScrollViewer(sender as DependencyObject);
                if (scrollControl != null)
                {
                    isFirstLoad = false;
                }

            }
            IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }

        private void container_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            allowScroll = true;
        }

        private void VirtualizingStackPanelEx_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.Source != null &&
                e.Source.GetType() == typeof(ContentPresenter) &&
                (((ContentPresenter)e.Source).Content.GetType() == typeof(TextChatMessageViewModel) ||
                ((ContentPresenter)e.Source).Content.GetType() == typeof(HtmlChatMessageViewModel) ||
                ((ContentPresenter)e.Source).Content.GetType() == typeof(MarkdownChatMessageViewModel) ||
                ((ContentPresenter)e.Source).Content.GetType() == typeof(ReplyMessageViewModel)||
                ((ContentPresenter)e.Source).Content.GetType() == typeof(AudioChatMessageViewModel)||
                ((ContentPresenter)e.Source).Content.GetType() == typeof(FileChatMessageViewModel)
                )
                )
                return;

            IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }
    }

    public class VirtualizingStackPanelEx : VirtualizingStackPanel
    {
        public void BringIntoViewPublic(int index)
        {
            base.BringIndexIntoView(index);
        }
    }
}
