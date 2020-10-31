using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Khernet.UI.Animations
{
    public static class FramworkElementAnimations
    {
        /// <summary>
        /// Adds slide from right animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task SlideAndFadeInFromRight(this FrameworkElement element, float seconds)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideFromRight(seconds, element.ActualWidth);

            //Add fade in animation
            storyborad.AddFadeIn(seconds / 2f);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Adds slide from right animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task SlideAndFadeInFromLeft(this FrameworkElement element, float seconds, bool keepWidth = true)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideFromLeft(seconds, element.ActualWidth);

            //Add fade in animation
            storyborad.AddFadeIn(seconds / 2f);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Adds slide from right animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task SlideAndFadeOutToLeft(this FrameworkElement element, float seconds, bool keepWidth = true)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideToLeft(seconds, element.ActualWidth, keepWidth: keepWidth);

            //Add fade in animation
            storyborad.AddFadeOut(seconds / 2f);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }


        /// <summary>
        /// Adds slide in from bottom animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task SlideAndFadeInFromBottom(this FrameworkElement element, float seconds, bool keepHeight = true)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideFromBottom(seconds, element.ActualHeight);

            //Add fade in animation
            storyborad.AddFadeIn(seconds / 3f);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Adds slide out to bottom animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task SlideAndFadeOutToBottom(this FrameworkElement element, float seconds, bool keepHeight = true)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideToBottom(seconds, element.ActualHeight, keepWidth: keepHeight);

            //Add fade in animation
            storyborad.AddFadeOut(seconds / 3f);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }


        /// <summary>
        /// Adds fade in animation to framework element.
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task FadeIn(this FrameworkElement element, float seconds)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add fade in animation
            storyborad.AddFadeIn(seconds);

            //Start the animation
            storyborad.Begin(element);

            //Make page visible when animation completes playing
            element.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Adds fade out animation to framework element
        /// </summary>
        /// <param name="element">The framework element to animate.</param>
        /// <param name="seconds">The time animation will take.</param>
        /// <returns>A <see cref="Task"/> for animation.</returns>
        public static async Task FadeOut(this FrameworkElement element, float seconds)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add fade in animation
            storyborad.AddFadeOut(seconds);

            //Start the animation
            storyborad.Begin(element);

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));

            //Hide and collapse the element
            element.Visibility = Visibility.Collapsed;
        }

    }
}
