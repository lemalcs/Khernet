using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Shown of progress in a circular style.
    /// </summary>
    public partial class ProgressCircleControl : UserControl
    {
        /// <summary>
        /// The radius of circle.
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        // The dependencyProperty as the backing store for Radius.
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(ProgressCircleControl), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, OnRadiusChanged));


        /// <summary>
        /// The thickness of progress indicator.
        /// </summary>
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thickness.
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(ProgressCircleControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, OnThicknessChanged));


        /// <summary>
        /// The current value of progress.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // The dependencyProperty as the backing store for Value. 
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(ProgressCircleControl), new PropertyMetadata(0d, OnValueChanged, OnValidateValue));

        /// <summary>
        /// The maximum value of progress.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // The dependencyProperty as the backing store for Maximum. 
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(ProgressCircleControl), new PropertyMetadata(0d, null, OnValidateMaximunValue));

        /// <summary>
        /// Verify if <see cref="Maximum"/> if greater or equals to <see cref="Minimum"/>.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="baseValue">The current value.</param>
        private static object OnValidateMaximunValue(DependencyObject d, object baseValue)
        {
            var control = d as ProgressCircleControl;
            if (control.Maximum < control.Minimum)
                return control.Minimum;
            return baseValue;
        }


        /// <summary>
        /// The minimum value of progress.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // The dependencyProperty as the backing store for Minimum. 
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(ProgressCircleControl), new PropertyMetadata(0d, null, OnValidateMinimuValue));

        /// <summary>
        /// Verify if <see cref="Minimum"/> if less or equals to <see cref="Maximum"/>.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="baseValue">The current value.</param>
        /// <returns></returns>
        private static object OnValidateMinimuValue(DependencyObject d, object baseValue)
        {
            var control = d as ProgressCircleControl;
            if (control.Minimum > control.Maximum)
                return control.Maximum;
            return baseValue;
        }

        /// <summary>
        /// The <see cref="Brush"/> of path that progress indicator follows.
        /// </summary>
        public Brush StrokeBackground
        {
            get { return (Brush)GetValue(StrokeBackgroundProperty); }
            set { SetValue(StrokeBackgroundProperty, value); }
        }

        // The DependencyProperty as the backing store for StrokeBackground.  
        public static readonly DependencyProperty StrokeBackgroundProperty =
            DependencyProperty.Register(nameof(StrokeBackground), typeof(Brush), typeof(ProgressCircleControl), new PropertyMetadata(null, OnStrokeBackGroundChanged));

        /// <summary>
        /// Sets the background color path that progress indicator follows.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="e">The current value.</param>
        private static void OnStrokeBackGroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressCircleControl;
            control.OnStrokeBackGroundChanged((Brush)e.NewValue);
        }

        private void OnStrokeBackGroundChanged(Brush backgroudColor)
        {
            backgroudEllipse.Stroke = backgroudColor;
        }

        /// <summary>
        /// Specifies the <see cref="Brush"/> of arcs.
        /// </summary>
        public Brush StrokeBrush
        {
            get { return (Brush)GetValue(StrokeBrushProperty); }
            set { SetValue(StrokeBrushProperty, value); }
        }

        // The DependencyProperty as the backing store for StrokeBrush.
        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register(nameof(StrokeBrush), typeof(Brush), typeof(ProgressCircleControl), new PropertyMetadata(null, OnStrokeBrushChanged));

        /// <summary>
        /// Sets the color of arc paths.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="e">The current value.</param>
        private static void OnStrokeBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressCircleControl;
            control.OnStrokeBrushChanged((Brush)e.NewValue);
        }

        /// <summary>
        /// Sets color of arc paths.
        /// </summary>
        /// <param name="brush">The <see cref="Brush"/> to color paths.</param>
        protected void OnStrokeBrushChanged(Brush brush)
        {
            startingPath.Stroke = brush;
            endingPath.Stroke = brush;
        }

        /// <summary>
        /// Verify if <see cref="Value"/> is between <see cref="Minimum"/> and <see cref="Maximum"/> values.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="baseValue">The current value.</param>
        /// <returns>The validated value.</returns>
        private static object OnValidateValue(DependencyObject d, object baseValue)
        {
            var control = d as ProgressCircleControl;
            double currentValue = (double)baseValue;

            if (currentValue < control.Minimum)
                return control.Minimum;

            if (currentValue > control.Maximum)
                return control.Maximum;

            return baseValue;
        }

        /// <summary>
        /// Sets the current <see cref="Value"/> of progress.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="baseValue">The current value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressCircleControl;
            control.OnValueChanged((double)e.NewValue);
        }

        protected void OnValueChanged(double value)
        {
            //Endpoint for arcs
            double xPoint = 0, yPoint = 0;

            //Perimeter of circular progress
            double perimeter = 2 * Math.PI * Radius;

            //Calculate the proportional value from Value property of this control
            double realValue = value * perimeter / Maximum;

            //The length of arc according to current value
            double arcLenght = 0;

            //If value is less or equal to half perimeter, set endpoints of right arc
            if (realValue <= perimeter / 2)
            {
                startingPath.Visibility = Visibility.Visible;
                endingPath.Visibility = Visibility.Hidden;

                arcLenght = realValue / Radius;

                yPoint = Thickness / 2 + Radius * (1 + Math.Sin(Math.Asin(-1) + arcLenght));
                xPoint = Math.Sqrt(Math.Pow(Radius, 2) - Math.Pow(yPoint - Radius - Thickness / 2, 2));

                SetArcFinalPoint(firstPath, new Point(xPoint, yPoint));
            }
            //If value is greater than half perimeter, set endpoints of left arc
            else if (realValue > perimeter / 2)
            {
                //Show the entire right arc
                SetArcFinalPoint(
                    firstPath,
                    new Point(0, Radius * 2 + Thickness / 2));

                startingPath.Visibility = Visibility.Visible;
                endingPath.Visibility = Visibility.Visible;

                arcLenght = realValue / Radius;

                yPoint = Thickness / 2 + Radius * (1 + Math.Sin(Math.Asin(-1) + arcLenght));
                xPoint = Radius + Thickness / 2 - Math.Sqrt(Math.Pow(Radius, 2) - Math.Pow(yPoint - Radius - Thickness / 2, 2));

                SetArcFinalPoint(secondPath, new Point(xPoint, yPoint));
            }
            else
            {
                //Hide left and right arcs if value is zero
                startingPath.Visibility = Visibility.Hidden;
                endingPath.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Changes the visual progress when <see cref="Thickness"/> is changed.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="e">The current and old value.</param>
        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressCircleControl;
            control.OnThicknessChanged((double)e.NewValue);
            control.OnValueChanged(control.Value);
        }

        /// <summary>
        /// Changes the control layout when <see cref="Thickness"/> changes.
        /// </summary>
        /// <param name="thickValue">The value of thickness.</param>
        protected void OnThicknessChanged(double thickValue)
        {
            SetArcThickness(thickValue);

            SetEllipseSize();

            SetContainerSize();
        }

        /// <summary>
        /// Sets arc thickness.
        /// </summary>
        /// <param name="thickValue">The thickness value.</param>
        private void SetArcThickness(double thickValue)
        {
            //Set thickness of left and right arc
            startingPath.StrokeThickness = thickValue;
            endingPath.StrokeThickness = thickValue;

            //Configure right arc 
            SetArcEndpoints(
                firstPath,
                new Point(0, Thickness / 2),
                new Point(0, Radius * 2 + Thickness / 2));

            //Configure left arc
            SetArcEndpoints(
                secondPath,
                new Point(Radius + Thickness / 2, Radius * 2 + Thickness / 2),
                new Point(Radius + Thickness / 2, Thickness / 2));
        }

        /// <summary>
        /// Changes the control layout when <see cref="Radius"/> is changed.
        /// </summary>
        /// <param name="d">The owner of this property.</param>
        /// <param name="e">The current and old value.</param>
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressCircleControl;
            control.OnRadiusChanged((double)e.NewValue);
            control.OnValueChanged(control.Value);
        }

        /// <summary>
        /// Changes the control layout when <see cref="Radius"/> is changed.
        /// </summary>
        /// <param name="radius">The value of radius.</param>
        protected void OnRadiusChanged(double radius)
        {
            //Configure right arc 
            SetArcRadius(
                firstPath,
                radius);

            //Configure left arc
            SetArcRadius(
                secondPath,
                radius);

            //Configure right arc 
            SetArcEndpoints(
                firstPath,
                new Point(0, Thickness / 2),
                new Point(0, Radius * 2 + Thickness / 2));

            //Configure left arc
            SetArcEndpoints(
                secondPath,
                new Point(Radius + Thickness / 2, Radius * 2 + Thickness / 2),
                new Point(Radius + Thickness / 2, Thickness / 2));

            SetEllipseSize();

            SetContainerSize();
        }

        /// <summary>
        /// Sets the size of <see cref="Grid"/> that contains the arcs.
        /// </summary>
        private void SetContainerSize()
        {
            gridContainer.Width = Radius * 2 + Thickness;
            gridContainer.Height = Radius * 2 + Thickness;
            gridContainer.ColumnDefinitions[0].Width = new GridLength(Radius + Thickness / 2);
            gridContainer.ColumnDefinitions[1].Width = new GridLength(Radius + Thickness / 2);
        }

        /// <summary>
        /// Sets <see cref="Ellipse"/> radius.
        /// </summary>
        private void SetEllipseSize()
        {
            backgroudEllipse.Width = Radius * 2 + Thickness;
            backgroudEllipse.Height = Radius * 2 + Thickness;
            backgroudEllipse.StrokeThickness = Thickness;
        }

        /// <summary>
        /// Sets the endpoint of arc.
        /// </summary>
        /// <param name="path">The <see cref="PathGeometry"/> that owns the arc.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The final point.</param>
        private void SetArcEndpoints(PathGeometry path, Point startPoint, Point endPoint)
        {
            var figures = path.Figures;
            var rightArc = figures[0];

            rightArc.StartPoint = startPoint;

            ArcSegment segment = (ArcSegment)figures[0].Segments[0];
            segment.Point = endPoint;
        }

        /// <summary>
        /// Sets the final <see cref="Point"/> of arc.
        /// </summary>
        /// <param name="path">The <see cref="PathGeometry"/> that owns the arc.</param>
        /// <param name="finalPoint">The final point.</param>
        private void SetArcFinalPoint(PathGeometry path, Point finalPoint)
        {
            var figures = path.Figures;
            var rightArc = figures[0];

            ArcSegment segment = (ArcSegment)figures[0].Segments[0];
            segment.Point = finalPoint;
        }

        /// <summary>
        /// Sets the radius of arc.
        /// </summary>
        /// <param name="path">The <see cref="PathGeometry"/> that owns the arc.</param>
        /// <param name="radius">The radius value.</param>
        private void SetArcRadius(PathGeometry path, double radius)
        {
            var figures = path.Figures;
            var rightArc = figures[0];

            ArcSegment segment = (ArcSegment)figures[0].Segments[0];
            segment.Size = new Size(radius, radius);
        }
        public ProgressCircleControl()
        {
            InitializeComponent();
        }
    }
}
