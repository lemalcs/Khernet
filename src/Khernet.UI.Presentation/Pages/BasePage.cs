using Khernet.UI.Animations;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Khernet.UI.Pages
{
    public class BasePage : Page
    {
        /// <summary>
        /// The view model for <see cref="Page"/>.
        /// </summary>
        private object viewModel;

        public object ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if (viewModel != value)
                {
                    viewModel = value;
                    this.DataContext = value;
                }
            }
        }

        /// <summary>
        /// Animation to play when page is loaded.
        /// </summary>
        public PageAnimation LoadAnimation { get; set; } = PageAnimation.SlideAndFadeFromRight;

        /// <summary>
        /// Animation to play when page is unloaded.
        /// </summary>
        public PageAnimation UnloadAnimation { get; set; } = PageAnimation.SlideAndFadeFromLeft;

        /// <summary>
        /// Indicate whether to animate out a <see cref="Page"/>.
        /// </summary>
        public bool ShouldAnimateOut { get; set; }

        /// <summary>
        /// The duration animation will take.
        /// </summary>
        public float SlideSecondsDuration { get; set; } = 0.5f;

        /// <summary>
        /// Event fired when any data is confirmed.
        /// </summary>
        public event EventHandler Commited;

        public BasePage()
        {
            //Hide page if an animation is applied
            if (LoadAnimation != PageAnimation.None)
                Visibility = System.Windows.Visibility.Collapsed;

            //Set an handler for page load
            Loaded += BasePage_Loaded;
        }

        /// <summary>
        /// Perform an animation when page is loaded.
        /// </summary>
        /// <param name="sender">The object source of event.</param>
        /// <param name="e">The event arguments.</param>
        private async void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Check whether animate out or in
            if (ShouldAnimateOut)
                await AnimateOut();
            else
                await AnimateIn();
        }

        /// <summary>
        /// Animates a page when load.
        /// </summary>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        private async Task AnimateIn()
        {
            //If there is nothing to animate, return
            if (LoadAnimation == PageAnimation.None)
                return;

            switch (LoadAnimation)
            {
                case PageAnimation.SlideAndFadeFromRight:

                    //Start the animation
                    await this.SlideAndFadeFromRight(SlideSecondsDuration);

                    break;
            }
        }

        /// <summary>
        /// Animates a page when unload.
        /// </summary>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public async Task AnimateOut()
        {
            //If there is nothing to animate, return
            if (UnloadAnimation == PageAnimation.None)
                return;

            switch (UnloadAnimation)
            {
                case PageAnimation.SlideAndFadeFromLeft:

                    //Start the animation
                    await this.SlideAndFadeToLeft(SlideSecondsDuration);

                    break;
            }
        }

        /// <summary>
        /// Executes <see cref="Commited"/> event.
        /// </summary>
        protected void OnCommited()
        {
            Commited?.Invoke(this, new EventArgs());
        }
    }

    public class BasePage<VM> : BasePage
        where VM : BaseModel, new()
    {
        /// <summary>
        /// Gets or set the specific view model.
        /// </summary>
        public VM SpecificViewModel
        {
            get { return (VM)ViewModel; }
            set
            {
                if (ViewModel != value)
                    ViewModel = value;
            }
        }
        public BasePage() : base()
        {
            ViewModel = new VM();
        }

        public BasePage(VM viewModel = null) : base()
        {
            if (viewModel == null)
            {
                ViewModel = new VM();
            }
            else
                ViewModel = viewModel;
        }

        /// <summary>
        /// Search for an element of a certain type in the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of element to find.</typeparam>
        /// <param name="visual">The parent element.</param>
        /// <returns></returns>
        public T FindVisualChild<T>(Visual visual) where T : Visual
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
    }
}
