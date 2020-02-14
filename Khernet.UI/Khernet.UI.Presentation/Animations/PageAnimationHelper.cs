using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Khernet.UI.Animations
{
    public static class PageAnimationHelper
    {
        /// <summary>
        /// Adds slide from right animation to page
        /// </summary>
        /// <param name="page">The page to animate</param>
        /// <param name="seconds">The time animation will take</param>
        /// <returns></returns>
        public static async Task SlideAndFadeFromRight(this Page page, float seconds)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideFromRight(seconds, 32);

            //Add fade in animation
            storyborad.AddFadeIn(seconds / 4f);

            //Start the animation
            storyborad.Begin(page);

            //Make page visible when animation completes playing
            page.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Adds slide from right animation to page
        /// </summary>
        /// <param name="page">The page to animate</param>
        /// <param name="seconds">The time animation will take</param>
        /// <returns></returns>
        public static async Task SlideAndFadeToLeft(this Page page, float seconds)
        {
            //Adds an animation to page
            var storyborad = new Storyboard();

            //Add slide animation
            storyborad.AddSlideToLeft(seconds, 32);

            //Add fade in animation
            storyborad.AddFadeOut(seconds / 4f);

            //Start the animation
            storyborad.Begin(page);

            //Make page visible when animation completes playing
            page.Visibility = Visibility.Visible;

            //Wait seconds to finish
            await TaskEx.Delay((int)(seconds * 1000));
        }
    }
}
