using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Khernet.UI.Animations
{
    public static class StoryboardHelper
    {
        /// <summary>
        /// Adds slide animation from right to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance from right.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddSlideFromRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
        {
            //Create an animation for margin property from right
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(offset, 0, -offset, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds slide animation from left to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance from left.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddSlideFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepWidth = true)
        {
            //Create an animation for margin property from left
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(-offset, 0, keepWidth ? offset : 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds slide animation to left to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance to left to end at.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        /// <param name="keepWidth">Indicates if width element must be kept during animation.</param>
        public static void AddSlideToLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepWidth = true)
        {
            //Create an animation for margin property from left
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(-offset, 0, keepWidth ? offset : 0, 0),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds slide animation to right to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance to right to end at.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddSlideToRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
        {
            //Create an animation for margin property from left
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(offset, 0, -offset, 0),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds slide animation from left to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance from left.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddSlideFromBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepHeight = true)
        {
            //Create an animation for margin property from left
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0, keepHeight ? offset : 0, 0, -offset),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds slide animation to left to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance to left to end at.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        /// <param name="keepWidth">Indicates if width element must be kept during animation.</param>
        public static void AddSlideToBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepWidth = true)
        {
            //Create an animation for margin property from left
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(0, keepWidth ? offset : 0, 0, -offset),
                DecelerationRatio = decelerationRatio
            };

            //Set Margin as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds fade in animation to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance from right.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddFadeIn(this Storyboard storyboard, float seconds)
        {
            //Create an animation for Opacity property
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 0,
                To = 1
            };

            //Set Opacity as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds fade out animation to storyboard.
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to.</param>
        /// <param name="seconds">The duration of animation.</param>
        /// <param name="offset">The distance from right.</param>
        /// <param name="decelerationRatio">The rate of deceleration.</param>
        public static void AddFadeOut(this Storyboard storyboard, float seconds)
        {
            //Create an animation for Opacity property
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 1,
                To = 0
            };

            //Set Opacity as target of animation
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            //Add the animation to storyboard
            storyboard.Children.Add(animation);
        }
    }
}
