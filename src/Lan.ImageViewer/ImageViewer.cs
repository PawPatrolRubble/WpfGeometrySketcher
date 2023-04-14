using Lan.SketchBoard;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lan.ImageViewer
{
    [TemplatePart(Type = typeof(Canvas), Name = "containerCanvas")]
    [TemplatePart(Type = typeof(Image), Name = "ImageViewer")]
    [TemplatePart(Type = typeof(Grid), Name = "GridContainer")]
    [TemplatePart(Type = typeof(TextBlock), Name = "TbMousePosition")]

    public class ImageViewer : Control
    {


        private Point? _mousePos;
        private Point? _lastMouseDownPoint;
        private readonly MatrixTransform _matrixTransform = new MatrixTransform();
        private readonly ScaleTransform _scaleTransform = new ScaleTransform();
        private Canvas _containerCanvas;
        private Image _image;
        private Grid _gridContainer;
        private readonly TransformGroup _transformGroup = new TransformGroup();
        private TextBlock _textBlock;
        private bool _isMouseFirstClick = true;
        private bool _isImageScaledByMouseWheel;

        #region binding properties



        #endregion


        #region dependency properties

        // Register a dependency property with the specified property name,
        // property type, owner type, and property metadata.
        // Assign DependencyPropertyKey to a nonpublic field.
        private static readonly DependencyPropertyKey PixelWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                name: "PixelWidthProperty",
                propertyType: typeof(double),
                ownerType: typeof(ImageViewer),
                typeMetadata: new FrameworkPropertyMetadata());

        public static DependencyProperty PixelWidthProperty = PixelWidthPropertyKey.DependencyProperty;

        // Declare a public get accessor.
        public double PixelWidth
        {
            get => (double)GetValue(PixelWidthProperty);
        }



        // Register a dependency property with the specified property name,
        // property type, owner type, and property metadata.
        // Assign DependencyPropertyKey to a nonpublic field.
        private static readonly DependencyPropertyKey PixelHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                name: "PixelHeightProperty",
                propertyType: typeof(double),
                ownerType: typeof(ImageViewer),
                typeMetadata: new FrameworkPropertyMetadata());


        public static DependencyProperty PixelHeightProperty = PixelHeightPropertyKey.DependencyProperty;

        // Declare a public get accessor.
        public double PixelHeight =>
            (double)GetValue(PixelHeightProperty);



        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata(default(ImageSource), OnImageSourceChangedCallback));

        private static void OnImageSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewer imageViewer = (ImageViewer)d;
            if (e.NewValue is BitmapSource source)
            {
                imageViewer.SetValue(PixelWidthPropertyKey, source.PixelWidth * 1.0);
                imageViewer.SetValue(PixelHeightPropertyKey, source.PixelHeight * 1.0);
            }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }


        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            "SketchBoardDataManager", typeof(ISketchBoardDataManager), typeof(ImageViewer), new PropertyMetadata(default(ISketchBoardDataManager)));

        public ISketchBoardDataManager SketchBoardDataManager
        {
            get { return (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty); }
            set { SetValue(SketchBoardDataManagerProperty, value); }
        }


        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale", typeof(double), typeof(ImageViewer), new PropertyMetadata(default(double), OnScaleChangedCallback));

        private static void OnScaleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer imageViewer && !imageViewer._disablePropertyChangeCallback)
            {
                var newValue = (double)e.NewValue;
                imageViewer.ScaleAtMouseDownPos(newValue);
            }
        }



        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }


        #endregion


        static ImageViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer), new FrameworkPropertyMetadata(typeof(ImageViewer)));
        }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            _containerCanvas = GetTemplateChild("containerCanvas") as Canvas;
            _image = GetTemplateChild("ImageViewer") as Image;

            _gridContainer = GetTemplateChild("GridContainer") as Grid;
            _textBlock = GetTemplateChild("TbMousePosition") as TextBlock;

            _transformGroup.Children.Add(_matrixTransform);
            _transformGroup.Children.Add(_scaleTransform);
            _gridContainer.RenderTransform = _transformGroup;

        }



        /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged" /> event, using the specified information as part of the eventual event data.</summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ImageSource is BitmapSource bitmap && !_isImageScaledByMouseWheel)
            {
                AutoScaleImageToFit(
                    sizeInfo.NewSize.Width,
                    sizeInfo.NewSize.Height,
                    bitmap.PixelWidth,
                    bitmap.PixelHeight);

            }
        }


        private double AutoScaleImageToFitRatio(double width, double height, double pixelWidth, double pixelHeight)
        {
            return Math.Min(width / pixelWidth, height / pixelHeight);
        }

        private void AutoScaleImageToFit(double width, double height, double pixelWidth, double pixelHeight)
        {
            var ratio = AutoScaleImageToFitRatio(width, height, pixelWidth, pixelHeight);
            var matrix = new Matrix();
            matrix.ScaleAt(
                ratio,
                ratio,
                (width - pixelWidth * ratio) / 2,
                (height - pixelHeight * ratio) / 2);

            _matrixTransform.Matrix = matrix;
        }



        #region events handlers

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised on this element. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _lastMouseDownPoint = e.GetPosition(this);
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                //capture the mouse, even when the mouse is not above the control, the mouse events will still be fired
                CaptureMouse();
                _mousePos = _lastMouseDownPoint;
            }
        }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {

            ReleaseMouseCapture();
            _mousePos = null;
            _isMouseFirstClick = false;
            base.OnMouseUp(e);
        }


        private bool _disablePropertyChangeCallback;

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _lastMouseDownPoint = e.GetPosition(this);
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;


            _disablePropertyChangeCallback = true;
            ScaleGridContainer(scale, _lastMouseDownPoint.Value);
            _disablePropertyChangeCallback = false;
            _isImageScaledByMouseWheel = true;
        }


        private void ScaleGridContainer(double scaleDelta, Point pos)
        {
            var matrix = _matrixTransform.Matrix;
            matrix.ScaleAt(scaleDelta, scaleDelta, pos.X, pos.Y);
            Scale = matrix.M11;
            //Debug.WriteLine($"x scale factor: {matrix.M11}");
            _matrixTransform.Matrix = matrix;
        }


        private void ScaleAtMouseDownPos(double scaleFactor)
        {
            if (_lastMouseDownPoint.HasValue)
            {
                var matrix = _matrixTransform.Matrix;
                ScaleGridContainer(scaleFactor / matrix.M11, _lastMouseDownPoint.Value);
            }
        }



        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var p = e.GetPosition(_image);
            var mousePositionRelativeToCanvas = e.GetPosition(_containerCanvas);
            _textBlock.Text = $"X:{p.X:f}, Y:{p.Y:f}";


            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Mouse.SetCursor(Cursors.Hand);

                if (_mousePos.HasValue && e.LeftButton == MouseButtonState.Pressed && !_isMouseFirstClick)
                {
                    var matrix = _matrixTransform.Matrix;

                    var dx = mousePositionRelativeToCanvas.X - _mousePos.Value.X;
                    var dy = mousePositionRelativeToCanvas.Y - _mousePos.Value.Y;

                    matrix.Translate(dx, dy);
                    _matrixTransform.Matrix = matrix;
                    _mousePos = mousePositionRelativeToCanvas;
                }
            }
        }



        #endregion
    }
}
