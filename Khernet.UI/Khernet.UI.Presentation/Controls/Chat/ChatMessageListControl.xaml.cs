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

        /// <summary>
        /// Indicates if chat list is scrolling to bottom side
        /// </summary>
        private bool scrollingToEnd = false;

        /// <summary>
        /// Indicates if the backing of vertical offset is allowed
        /// </summary>
        bool allowScroll = false;

        /// <summary>
        /// Indicates if it is the first time this control is loaded so it does not have any messages
        /// </summary>
        bool isFirstLoad = false;

        /// <summary>
        /// The chat message model to scroll to when chat list is loaded
        /// </summary>
        private ChatMessageItemViewModel penddingChatModel;

        /// <summary>
        /// Index of chat message within underlying items source
        /// </summary>
        private int penddingChatModelIndex = -1;

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

            var scrollControl = FindVisualChild<ScrollViewer>(container);

            if (scrollControl == null)
                return;

            if (IoCContainer.Get<ChatMessageListViewModel>().Items == null)
                return;

            int count = IoCContainer.Get<ChatMessageListViewModel>().Items.Count;

            if (count == 0)
                return;

            var panel = FindVisualChild<VirtualizingStackPanelEx>(container);

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

            scrollingToEnd = false;
        }

        private void Container_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Scroll to up
            if (e.Delta > 0)
            {
                var scroll = FindVisualChild<ScrollViewer>(container);

                if (scroll.VerticalOffset == 0)
                {
                    LoadMessages(false);
                }
            }
            allowScroll = true;
        }

        private void container_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollControl = FindVisualChild<ScrollViewer>(container);

            var panel = FindVisualChild<VirtualizingStackPanel>(scrollControl);

            var hitTest = VisualTreeHelper.HitTest(panel, new Point(1, e.ViewportHeight));

            if (hitTest != null)
            {
                IoCContainer.Get<ChatMessageListViewModel>().CheckUnreadMessageAsRead((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext);
                if (allowScroll)
                    IoCContainer.Get<ChatMessageListViewModel>().SetCurrentChatModel((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext);
            }

            var firstItem = VisualTreeHelper.HitTest(panel, new Point(1, 30));

            if (firstItem != null)
            {
                IoCContainer.Get<ChatMessageListViewModel>().SetFirstViewChatModel(
                    (ChatMessageItemViewModel)((FrameworkElement)firstItem.VisualHit).DataContext);
            }

            if (e.VerticalOffset == 0 && e.VerticalChange < 0)
            {
                if (allowLoadMessages)
                {
                    LoadMessages(false);
                }
                allowLoadMessages = true;
            }
            else if (!scrollingToEnd)
            {
                //Detect if scroll bar is at bottom of list
                double scrollDifference = Math.Abs(e.ExtentHeight - e.VerticalOffset - e.ViewportHeight);

                if (scrollDifference >= 0 && scrollDifference <= 1)
                {
                    LoadMessages(true);
                }
            }
        }

        /// <summary>
        /// Loads messages to chat list asynchronously.
        /// </summary>
        /// <param name="loadFordward">True to load new messages otherwise false.</param>
        private void LoadMessages(bool loadFordward)
        {
            ChatMessageItemViewModel lastChatModel = null;

            if (IoCContainer.Get<ChatMessageListViewModel>().UserContext != null)
            {
                lastChatModel = IoCContainer.Get<ChatMessageListViewModel>().UserContext.CurrentChatModel;
            }

            IoCContainer.UI.ExecuteAsync(() =>
            {
                IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(loadFordward);

                if (loadFordward)
                    return;

                ChatMessageItemViewModel firstChatModel = null;

                if (IoCContainer.Get<ChatMessageListViewModel>().UserContext != null)
                {
                    firstChatModel = IoCContainer.Get<ChatMessageListViewModel>().UserContext.FirstViewChatModel;
                }
                if (IoCContainer.Get<ChatMessageListViewModel>().Items.IndexOf(firstChatModel) != 0)
                {

                    int lastIndex = IoCContainer.Get<ChatMessageListViewModel>().Items.IndexOf(lastChatModel);
                    ScrollToItem(lastChatModel, lastIndex);
                }
            });
        }

        public void ScrollToItem(ChatMessageItemViewModel chatModel, int index)
        {
            if (chatModel == null)
                return;

            if (container.IsLoaded)
                GetTreeViewItem(container, chatModel, index);
            else
            {
                penddingChatModel = chatModel;
                penddingChatModelIndex = index;
            }
        }


        /// <summary>
        /// Recursively search for an item in this subtree.
        /// </summary>
        /// <param name="container">
        /// The parent ItemsControl. This can be a TreeView or a TreeViewItem.
        /// </param>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The TreeViewItem that contains the specified item.
        /// </returns>
        private TreeViewItem GetTreeViewItem(ItemsControl container, object item, int startIndex = 0)
        {
            if (container != null)
            {
                if (container.DataContext == item)
                {
                    return container as TreeViewItem;
                }

                // Expand the current container
                if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
                {
                    container.SetValue(TreeViewItem.IsExpandedProperty, true);
                }

                // Try to generate the ItemsPresenter and the ItemsPanel.
                // by calling ApplyTemplate.  Note that in the 
                // virtualizing case even if the item is marked 
                // expanded we still need to do this step in order to 
                // regenerate the visuals because they may have been virtualized away.

                container.ApplyTemplate();
                ItemsPresenter itemsPresenter =
                    (ItemsPresenter)container.Template.FindName("ItemsHost", container);
                if (itemsPresenter != null)
                {
                    itemsPresenter.ApplyTemplate();
                }
                else
                {
                    // The Tree template has not named the ItemsPresenter, 
                    // so walk the descendents and find the child.
                    itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    if (itemsPresenter == null)
                    {
                        container.UpdateLayout();

                        itemsPresenter = FindVisualChild<ItemsPresenter>(container);
                    }
                }

                Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

                VirtualizingStackPanelEx virtualizingPanel = itemsHostPanel as VirtualizingStackPanelEx;

                for (int i = startIndex, count = container.Items.Count; i < count; i++)
                {
                    TreeViewItem subContainer;
                    if (virtualizingPanel != null)
                    {
                        // Bring the item into view so 
                        // that the container will be generated.
                        virtualizingPanel.BringIntoViewPublic(i);

                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);
                    }
                    else
                    {
                        subContainer =
                            (TreeViewItem)container.ItemContainerGenerator.
                            ContainerFromIndex(i);

                        // Bring the item into view to maintain the 
                        // same behavior as with a virtualizing panel.
                        subContainer.BringIntoView();
                    }

                    if (subContainer != null)
                    {
                        // Search the next level for the object.
                        TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                        else
                        {
                            // The object is not under this TreeViewItem
                            // so collapse it.
                            subContainer.IsExpanded = false;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        private T FindVisualChild<T>(Visual visual) where T : Visual
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                    {
                        return correctlyTyped;
                    }

                    T descendent = FindVisualChild<T>(child);
                    if (descendent != null)
                    {
                        return descendent;
                    }
                }
            }

            return null;
        }

        private void container_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                var scroll = FindVisualChild<ScrollViewer>(sender as Visual);

                if (scroll.VerticalOffset == 0)
                {
                    IoCContainer.UI.ExecuteAsync(() =>
                    {
                        IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(false);
                    });
                }
            }

            if (e.Key == Key.PageUp || e.Key == Key.PageDown)
                allowScroll = true;
        }

        private void container_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var scrollControl = FindVisualChild<ScrollViewer>(sender as Visual);

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
            if (penddingChatModel != null)
            {
                ScrollToItem(penddingChatModel, penddingChatModelIndex);
                penddingChatModel = null;
                penddingChatModelIndex = -1;
            }

            if (isFirstLoad)
            {
                var scrollControl = FindVisualChild<ScrollViewer>(sender as Visual);
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
    }

    public class VirtualizingStackPanelEx : VirtualizingStackPanel
    {
        public void BringIntoViewPublic(int index)
        {
            base.BringIndexIntoView(index);
        }
    }
}
