using Khernet.UI.Animations;
using System.Windows;

namespace Khernet.UI.DependencyProperties
{
    public class AnimationBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {
        /// <summary>
        /// Indicates if it the first time this property is loaded
        /// </summary>
        public bool IsFirstLoad { get; set; } = true;

        protected override void OnValueUpdated(DependencyObject d, object baseValue)
        {
            //Check if it is a framework element
            if (!(d is FrameworkElement))
                return;

            //Check if value has changed
            if (d.GetValue(ValueProperty) == baseValue && !IsFirstLoad)
                return;

            //Get the framework element
            FrameworkElement element = d as FrameworkElement;
            if (IsFirstLoad)
            {
                //Hook an callback just for waiting the element to load

                RoutedEventHandler onLoaded = null;

                onLoaded = (sender, e) =>
                {
                    //Unhook an for loaded event 
                    element.Loaded -= onLoaded;

                    //Start animation
                    DoAnimation(element, baseValue);

                    IsFirstLoad = false;
                };

                //Hook an for loaded event 
                element.Loaded += onLoaded;
            }
            else
                //Start animation once again
                DoAnimation(element, baseValue);
        }

        public virtual void DoAnimation(FrameworkElement d, object baseValue)
        {

        }
    }

    public class SlideFromLeftAnimationProperty : AnimationBaseProperty<SlideFromLeftAnimationProperty>
    {
        /// <summary>
        /// Do a slide from left animation to WPF element
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        public override async void DoAnimation(FrameworkElement d, object baseValue)
        {
            //Check if an element have to animate in or animate out
            if ((bool)baseValue)
                //Animate in
                await d.SlideAndFadeInFromLeft(IsFirstLoad ? 0 : 0.5f, false);
            else
                //Animate out
                await d.SlideAndFadeOutToLeft(IsFirstLoad ? 0 : 0.5f, false);
        }
    }

    /// <summary>
    /// Animates an element sliding up from bottom to show an sliding down from botton to hide
    /// </summary>
    public class SlideFromBottomAnimationProperty : AnimationBaseProperty<SlideFromBottomAnimationProperty>
    {
        /// <summary>
        /// Do a slide from bottom animation to WPF element
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        public override async void DoAnimation(FrameworkElement d, object baseValue)
        {
            //Check if an element have to animate in or animate out
            if ((bool)baseValue)
                //Animate in
                await d.SlideAndFadeInFromBottom(IsFirstLoad ? 0 : 0.3f, false);
            else
                //Animate out
                await d.SlideAndFadeOutToBottom(IsFirstLoad ? 0 : 0.3f, false);
        }
    }

    /// <summary>
    /// Animates an element sliding up from bottom to show an sliding down from botton to hide
    /// </summary>
    public class FadeAnimationProperty : AnimationBaseProperty<FadeAnimationProperty>
    {
        /// <summary>
        /// Do a slide from bottom animation to WPF element
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        public override async void DoAnimation(FrameworkElement d, object baseValue)
        {
            //Check if an element have to animate in or animate out
            if ((bool)baseValue)
                //Animate in
                await d.FadeIn(IsFirstLoad ? 0 : 0.2f);
            else
                //Animate out
                await d.FadeOut(IsFirstLoad ? 0 : 0.2f);
        }
    }
}
