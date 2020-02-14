using System;
using System.Windows;

namespace Khernet.UI.DependencyProperties
{
    /// <summary>
    /// Base class for attached properties
    /// </summary>
    /// <typeparam name="Owner">The class that owns the property</typeparam>
    /// <typeparam name="Property">The type of property</typeparam>
    public abstract class BaseAttachedProperty<Owner, Property>
        where Owner : new()
    {
        /// <summary>
        /// The class which contains attached properties
        /// </summary>
        public static Owner Instance { get; private set; } = new Owner();

        /// <summary>
        /// Event to be fired when property value changes
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, args) => { };

        /// <summary>
        /// Event to be fired when value updates, even if it has the same value
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, value) => { };

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(Property),
            typeof(BaseAttachedProperty<Owner, Property>),
            new UIPropertyMetadata(
                default(Property),
                new PropertyChangedCallback(OnPropertyChanged),
                new CoerceValueCallback(OnPropertyUpdated))
            );

        /// <summary>
        /// The callback when <see cref="ValueProperty"/>  changes its value, even when it is the same value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object OnPropertyUpdated(DependencyObject d, object baseValue)
        {
            (Instance as BaseAttachedProperty<Owner, Property>)?.OnValueUpdated(d, baseValue);

            return baseValue;
        }

        /// <summary>
        /// The callback method when <see cref="ValueProperty"/> changes its value
        /// </summary>
        /// <param name="d">The element property</param>
        /// <param name="e">Arguments for event</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (Instance as BaseAttachedProperty<Owner, Property>)?.OnValueChanged(d, e);
        }

        /// <summary>
        /// Gets the value of element
        /// </summary>
        /// <param name="d">The element to get value from</param>
        /// <returns>Value of property</returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        /// <summary>
        /// Sets the value of elent property
        /// </summary>
        /// <param name="d">The element to set the property value</param>
        /// <param name="value">The value for property</param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);


        protected virtual void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnValueUpdated(DependencyObject d, object baseValue)
        {
        }

    }
}
