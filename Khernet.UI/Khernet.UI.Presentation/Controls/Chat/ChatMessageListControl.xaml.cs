using Khernet.UI.IoC;
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
        /// Indicates if loading of messages is allowed.
        /// </summary>
        bool allowLoadMessages = false;

        /// <summary>
        /// Indicates if chat list is scrolling to bottom side.
        /// </summary>
        private bool scrollingToEnd = false;

        /// <summary>
        /// Indicates if the backing of vertical offset is allowed.
        /// </summary>
        bool allowScroll = false;

        /// <summary>
        /// Indicates if it is the first time this control is loaded so it does not have any messages.
        /// </summary>
        bool isFirstLoad = false;

        /// <summary>
        /// The chat message model to scroll to when chat list is loaded.
        /// </summary>
        private ChatMessageItemViewModel pendingChatModel;

        /// <summary>
        /// Index of chat message within underlying items source.
        /// </summary>
        private int pendingChatModelIndex = -1;

        /// <summary>
        /// Indicates whether it's necessary to attempt do scroll to current item again.
        /// </summary>
        private bool scrollToCurrentItemAgain = true;

        /// <summary>
        /// The <see cref="ScrollViewer"/> owned by <see cref="TreeView"/> container.
        /// </summary>
        private ScrollViewer scrollViewer;

        /// <summary>
        /// The <see cref="VirtualizingStackPanelEx"/> owned by <see cref="ScrollViewer"/>.
        /// </summary>
        private VirtualizingStackPanelEx panel;

        /// <summary>
        /// Holds the found control after hit test within ScrollViewer.
        /// </summary>
        private HitTestResult hitResult;

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
            allowLoadMessages = false;

            if (scrollViewer == null)
                return;

            if (IoCContainer.Get<ChatMessageListViewModel>().Items == null)
                return;

            int count = IoCContainer.Get<ChatMessageListViewModel>().Items.Count;

            if (count == 0)
                return;

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
                scrollViewer.ScrollToEnd();
                scrollViewer.ScrollToBottom();
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight - scrollViewer.ViewportHeight);
            }

            allowLoadMessages = true;
            scrollingToEnd = false;
        }

        private void Container_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollingToEnd = e.Delta < 0;

            //Scroll to up
            if (e.Delta > 0)
            {
                if (scrollViewer.VerticalOffset == 0)
                {
                    LoadMessages(false);
                }
            }
            allowScroll = true;
        }

        private void container_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (scrollViewer == null)
            {
                scrollViewer = FindVisualChild<ScrollViewer>(container);
                panel = FindVisualChild<VirtualizingStackPanelEx>(scrollViewer);
            }

            hitResult = VisualTreeHelper.HitTest(panel, new Point(1, e.ViewportHeight));

            if (hitResult != null && allowScroll)
                IoCContainer.Get<ChatMessageListViewModel>().SetCurrentChatModel((ChatMessageItemViewModel)((FrameworkElement)hitResult.VisualHit).DataContext);

            //Detect if scroll bar is at top of list before load messages
            if (e.VerticalOffset == 0 && e.VerticalChange < 0)
            {
                if (allowLoadMessages)
                    LoadMessages(false);

                allowLoadMessages = true;
            }
            //Detect if scroll bar is at bottom of list before load messages
            else if ((scrollingToEnd && allowScroll && allowLoadMessages) &&
                (scrollViewer.ExtentHeight == scrollViewer.VerticalOffset + scrollViewer.ViewportHeight))
            {
                LoadMessages(true);
            }
        }

        /// <summary>
        /// Loads messages to chat list asynchronously.
        /// </summary>
        /// <param name="loadFordward">True to load new messages otherwise false.</param>
        private void LoadMessages(bool loadFordward)
        {
            IoCContainer.UI.Execute(() =>
            {
                IoCContainer.Get<ChatMessageListViewModel>().LoadMessages(loadFordward);
            });
        }

        public void ScrollToItem(ChatMessageItemViewModel chatModel, int index)
        {
            if (chatModel == null)
                return;

            if (container.IsLoaded)
            {
                TreeViewItem item = GetTreeViewItem(container, chatModel, index);
                item.IsSelected = true;
            }
            else
            {
                //Save item to scroll to when list of messages (TreeView) is ready to use
                pendingChatModel = chatModel;
                pendingChatModelIndex = index;
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
                    // so walk the descendants and find the child.
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
                if (scrollViewer.VerticalOffset == 0)
                    LoadMessages(false);

                scrollingToEnd = false;
            }

            if (e.Key == Key.PageUp || e.Key == Key.PageDown)
            {
                allowScroll = true;
                scrollingToEnd = true;
            }
        }

        private void container_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (scrollViewer == null)
            {
                isFirstLoad = true;
                return;
            }

            pendingChatModel = null;
            scrollToCurrentItemAgain = true;

            allowLoadMessages = false;
            allowScroll = false;
            IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }

        private void container_Loaded(object sender, RoutedEventArgs e)
        {
            if (pendingChatModel != null)
            {
                ScrollToItem(pendingChatModel, pendingChatModelIndex);
                pendingChatModel = null;
                pendingChatModelIndex = -1;
                scrollToCurrentItemAgain = true;
            }

            if (isFirstLoad)
            {
                if (scrollViewer != null)
                {
                    isFirstLoad = false;
                }
            }
            IoCContainer.Get<ChatMessageListViewModel>().FocusTextBox();
        }

        private void container_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (container.SelectedItem == null)
                return;

            if (!scrollToCurrentItemAgain)
                return;

            if (panel == null || scrollViewer == null)
                return;

            var hitTest = VisualTreeHelper.HitTest(panel, new Point(1, scrollViewer.ViewportHeight));

            if (hitTest != null)
            {
                //If current message is not visible in chat list then try to select it again
                if (((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext).Id !=
                    IoCContainer.Get<ChatMessageListViewModel>().UserContext.CurrentChatModel.Id)
                {
                    ChatMessageItemViewModel currentModel = IoCContainer.Get<ChatMessageListViewModel>().UserContext.CurrentChatModel;
                    scrollToCurrentItemAgain = false;
                    ScrollToItem(currentModel, IoCContainer.Get<ChatMessageListViewModel>().Items.IndexOf(currentModel));
                }
            }
        }

        private void container_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //ScrollChanged event is not fired when thumb is dragged.
            //Set the current message when thumb is released after dragged.
            if (!allowScroll)
            {
                var hitTest = VisualTreeHelper.HitTest(panel, new Point(1, scrollViewer.ViewportHeight));

                if (hitTest != null)
                {
                    IoCContainer.Get<ChatMessageListViewModel>().SetCurrentChatModel((ChatMessageItemViewModel)((FrameworkElement)hitTest.VisualHit).DataContext);
                }
            }

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