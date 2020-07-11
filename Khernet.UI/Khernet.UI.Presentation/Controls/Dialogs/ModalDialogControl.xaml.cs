using System.Windows;
using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Shown of an modal dialog inside parent window
    /// </summary>
    public partial class ModalDialogControl : UserControl
    {
        /// <summary>
        /// Default Up and down margin for ContentControl
        /// </summary>
        double defaultMargin = 20;

        /// <summary>
        /// Current up and down margin for ContentControl
        /// </summary>

        double currentMargin = 20;

        /// <summary>
        /// The height of windows that owns this control
        /// </summary>
        double windowTitleHeight = 30;

        /// <summary>
        /// The current height of window owner
        /// </summary>
        double currentOwnerHeight;

        /// <summary>
        /// The height of parent windows that owns this control
        /// </summary>
        public double OwnerHeight
        {
            get { return (double)GetValue(OwnerHeightProperty); }
            set { SetValue(OwnerHeightProperty, value); }
        }

        // The dependencyProperty as the backing store for OwnerHeight.
        public static readonly DependencyProperty OwnerHeightProperty =
            DependencyProperty.Register(nameof(OwnerHeight), typeof(double), typeof(ModalDialogControl), new PropertyMetadata(0d, OnOwnerHeightChanged));

        /// <summary>
        /// Set height of <see cref="ModalDialogControl"/> based on <see cref="OwnerHeight"/>
        /// </summary>
        /// <param name="d">The control that owns this property</param>
        /// <param name="e">The new value</param>
        private static void OnOwnerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ModalDialogControl;
            control.OnOwnerHeightChanged((double)e.NewValue);
        }

        /// <summary>
        /// Sets the height of this control
        /// </summary>
        /// <param name="ownerHeight">The height value</param>
        protected void OnOwnerHeightChanged(double ownerHeight)
        {
            //Set the current owner height
            currentOwnerHeight = ownerHeight;

            //Check if new value is greater or equal to new value
            //otherwise do not change MinHeight
            if (ownerHeight >= windowTitleHeight + currentMargin)
                MinHeight = ownerHeight - windowTitleHeight - currentMargin;
        }

        /// <summary>
        /// The view model for control to show in modal dialog
        /// </summary>
        public BaseModel DialogViewModel
        {
            get { return (BaseModel)GetValue(DialogViewModelProperty); }
            set { SetValue(DialogViewModelProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty as the backing store for DialogViewModel.
        /// </summary>
        public static readonly DependencyProperty DialogViewModelProperty =
            DependencyProperty.Register(nameof(DialogViewModel), typeof(BaseModel), typeof(ModalDialogControl), new PropertyMetadata(null, OnViewModelUpdated, null));

        /// <summary>
        /// Set view model for <see cref="ModalDialogControl"/>
        /// </summary>
        /// <param name="d">The control that owns this property</param>
        /// <param name="e">The height value</param>
        private static void OnViewModelUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ModalDialogControl;

            if (control == null)
                return;

            control.OnViewModelUpdated((BaseModel)e.NewValue);
        }

        /// <summary>
        /// Sets view model for this control
        /// </summary>
        /// <param name="viewModel">The view model value</param>
        protected void OnViewModelUpdated(BaseModel viewModel)
        {
            DataContext = viewModel;
            var modalModel = viewModel as ModalDialogViewModel;

            //Set margin to zero for full screen mode
            if (modalModel != null && modalModel.IsFullScreen)
            {
                Width = double.NaN;
            }
            else
            {
                //Set margin to 20 (up margin + down margin)
                currentMargin = defaultMargin;

                //Set width of this control
                if (modalModel != null)
                {
                    //Width = modalModel.Content.Width;
                }
            }

            //Set new MinHeight value
            OnOwnerHeightChanged(currentOwnerHeight);
        }

        public ModalDialogControl()
        {
            InitializeComponent();
        }
    }
}
