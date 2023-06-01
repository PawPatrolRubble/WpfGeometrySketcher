#region

#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#endregion

namespace Lan.ImageViewer
{
    /// <summary>
    /// a basic image displaying control, only provide zoom and translation
    /// </summary>
    [TemplatePart(Type = typeof(Canvas), Name = "containerCanvas")]
    [TemplatePart(Type = typeof(Image), Name = "ImageViewer")]
    [TemplatePart(Type = typeof(Grid), Name = "GridContainer")]
    [TemplatePart(Type = typeof(TextBlock), Name = "TbMousePosition")]
    [TemplatePart(Type = typeof(Button), Name = "BtnFit")]
    [TemplatePart(Type = typeof(Border), Name = "BorderContainer")]
    [TemplatePart(Type = typeof(Line), Name = "VerticalLine")]
    [TemplatePart(Type = typeof(Line), Name = "HorizontalLine")]
    public class ImageViewerBasic : Control
    {
        #region fields



        private readonly MatrixTransform _matrixTransform = new MatrixTransform();
        private readonly ScaleTransform _scaleTransform = new ScaleTransform();
        private readonly TranslateTransform _translateTransform = new TranslateTransform();
        private readonly TransformGroup _transformGroup = new TransformGroup();
        private Canvas? _containerCanvas;

        private bool _disablePropertyChangeCallback;
        private Grid? _gridContainer;
        private Image? _image;
        private bool _isImageScaledByMouseWheel;
        private bool _isMouseFirstClick = true;
        private Point? _lastMouseDownPoint;
        private Point? _mousePos;
        private TextBlock? _textBlock;
        private Button? _fitButton;
        private Border? _borderContainer;

        private Line? _verticalLineGeometry;
        private Line? _horizontalLineGeometry;
        #endregion

        #region Propeties

        public static readonly DependencyProperty MouseDoubleClickPositionProperty = DependencyProperty.Register(
            nameof(MouseDoubleClickPosition), typeof(Point), typeof(ImageViewerBasic), new FrameworkPropertyMetadata(default(Point))
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });


        private static readonly DependencyPropertyKey PixelWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "PixelWidthProperty",
                typeof(double),
                typeof(ImageViewerBasic),
                new FrameworkPropertyMetadata());

        public static DependencyProperty PixelWidthProperty = PixelWidthPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey PixelHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "PixelHeightProperty",
                typeof(double),
                typeof(ImageViewerBasic),
                new FrameworkPropertyMetadata());

        public static DependencyProperty PixelHeightProperty = PixelHeightPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(ImageViewerBasic),
            new PropertyMetadata(default(ImageSource), OnImageSourceChangedCallback));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale", typeof(double), typeof(ImageViewerBasic),
            new PropertyMetadata(default(double), OnScaleChangedCallback));


        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness), typeof(double), typeof(ImageViewerBasic), new PropertyMetadata(1.0));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty CrossLineColorProperty = DependencyProperty.Register(
            nameof(CrossLineColor), typeof(Brush), typeof(ImageViewerBasic), new PropertyMetadata(Brushes.Lime));

        public Brush CrossLineColor
        {
            get { return (Brush)GetValue(CrossLineColorProperty); }
            set { SetValue(CrossLineColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(
            nameof(StrokeDashArray), typeof(DoubleCollection), typeof(ImageViewerBasic), new PropertyMetadata(default(DoubleCollection)));

        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }


        public static readonly DependencyProperty ShowCrossLineProperty = DependencyProperty.Register(
            nameof(ShowCrossLine), typeof(bool), typeof(ImageViewerBasic), new PropertyMetadata(true));

        public bool ShowCrossLine
        {
            get { return (bool)GetValue(ShowCrossLineProperty); }
            set { SetValue(ShowCrossLineProperty, value); }
        }


        public Point MouseDoubleClickPosition
        {
            get { return (Point)GetValue(MouseDoubleClickPositionProperty); }
            set { SetValue(MouseDoubleClickPositionProperty, value); }
        }




        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public double PixelHeight
        {
            get => (double)GetValue(PixelHeightProperty);
        }

        public double PixelWidth
        {
            get => (double)GetValue(PixelWidthProperty);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        #endregion


        public ImageViewerBasic()
        {
            SizeChanged += (s, e) =>
            {
                if (ImageSource is BitmapSource bitmap && !_isImageScaledByMouseWheel)
                {
                    AutoScaleImageToFit(
                        e.NewSize.Width,
                        e.NewSize.Height,
                        bitmap.PixelWidth,
                        bitmap.PixelHeight);
                }
            };
        }

        #region others



        private void AutoScaleImageToFit(double width, double height, double pixelWidth, double pixelHeight)
        {
            var ratio = AutoScaleImageToFitRatio(width, height, pixelWidth, pixelHeight);

            var matrix = new Matrix();
            matrix.ScaleAt(
                ratio,
                ratio,
                0,
                0);
            matrix.Translate((width - pixelWidth * ratio) / 2, (height - pixelHeight * ratio) / 2);
            _matrixTransform.Matrix = matrix;
        }

        private double AutoScaleImageToFitRatio(double width, double height, double pixelWidth, double pixelHeight)
        {
            return Math.Min(width / pixelWidth, height / pixelHeight);
        }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            _containerCanvas ??= GetTemplateChild("containerCanvas") as Canvas;
            _image = GetTemplateChild("ImageViewer") as Image;

            _gridContainer ??= GetTemplateChild("GridContainer") as Grid;
            _textBlock ??= GetTemplateChild("TbMousePosition") as TextBlock;
            _fitButton ??= GetTemplateChild("BtnFit") as Button;
            _borderContainer ??= GetTemplateChild("BorderContainer") as Border;
            _horizontalLineGeometry ??= GetTemplateChild("HorizontalLine") as Line;
            _verticalLineGeometry ??= GetTemplateChild("VerticalLine") as Line;

            _transformGroup.Children.Add(_matrixTransform);
            _transformGroup.Children.Add(_scaleTransform);

            if (_borderContainer != null)
            {
                _borderContainer.SizeChanged += (s, e) =>
                {
                    if (_verticalLineGeometry != null && _horizontalLineGeometry != null)
                    {
                        _verticalLineGeometry.X1 = _borderContainer.ActualWidth / 2;
                        _verticalLineGeometry.Y1 = 0;

                        _verticalLineGeometry.X2 = _borderContainer.ActualWidth / 2;
                        _verticalLineGeometry.Y2 = _borderContainer.ActualHeight;

                        _horizontalLineGeometry.X1 = 0;
                        _horizontalLineGeometry.Y1 = _borderContainer.ActualHeight / 2;

                        _horizontalLineGeometry.X2 = _borderContainer.ActualWidth;
                        _horizontalLineGeometry.Y2 = _borderContainer.ActualHeight / 2;
                    }

                    if (PixelHeight != 0 && PixelWidth != 0)
                    {
                        AutoScaleImageToFit(_borderContainer.ActualWidth, _borderContainer.ActualHeight, PixelWidth, PixelHeight);
                    }
                };


                if (_fitButton != null)
                {
                    _fitButton.Click += (s, e) =>
                    {
                        if (_borderContainer != null)
                        {
                            AutoScaleImageToFit(_borderContainer.ActualWidth, _borderContainer.ActualHeight, PixelWidth,
                                PixelHeight);
                        }
                    };
                }

                if (_gridContainer != null) _gridContainer.RenderTransform = _transformGroup;
            }
        }

        private static void OnImageSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageViewer = (ImageViewerBasic)d;
            double pixelWidth = 0;
            double pixelHeight = 0;

            if (e.NewValue is BitmapSource source)
            {
                pixelWidth = source.Width;
                pixelHeight = source.Height;
            }

            if (e.NewValue is DrawingImage drawingImage)
            {
                pixelWidth = drawingImage.Width;
                pixelHeight = drawingImage.Height;
            }

            if (imageViewer._borderContainer != null && (Math.Abs(imageViewer.PixelWidth - pixelWidth) > Double.Epsilon
                                                         || Math.Abs(imageViewer.PixelHeight - pixelHeight) > double.Epsilon))
            {
                Console.WriteLine($"auto fit in image source change");
                imageViewer.AutoScaleImageToFit(
                    imageViewer._borderContainer.ActualWidth,
                    imageViewer._borderContainer.ActualHeight,
                    pixelWidth, pixelHeight);
            }

            imageViewer.SetValue(PixelWidthPropertyKey, pixelWidth * 1.0);
            imageViewer.SetValue(PixelHeightPropertyKey, pixelHeight * 1.0);
        }

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

        /// <summary>Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick" /> routed event.</summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            MouseDoubleClickPosition = e.GetPosition(this);
        }


        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var p = e.GetPosition(_image);
            var p1 = e.GetPosition(_borderContainer);
            var mousePositionRelativeToCanvas = e.GetPosition(_containerCanvas);
            if (_textBlock != null)
            {
                _textBlock.Text = $"X:{p.X:f}, Y:{p.Y:f}";
            }


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

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            _mousePos = null;
            _isMouseFirstClick = false;
            base.OnMouseUp(e);
        }

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

        /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged" /> event, using the specified information as part of the eventual event data.</summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);



        }

        private static void OnScaleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerBasic imageViewer && !imageViewer._disablePropertyChangeCallback)
            {
                var newValue = (double)e.NewValue;
                imageViewer.ScaleAtMouseDownPos(newValue);
            }
        }

        private void ScaleAtMouseDownPos(double scaleFactor)
        {
            if (_lastMouseDownPoint.HasValue)
            {
                var matrix = _matrixTransform.Matrix;
                ScaleGridContainer(scaleFactor / matrix.M11, _lastMouseDownPoint.Value);
            }
        }

        private void ScaleGridContainer(double scaleDelta, Point pos)
        {
            var matrix = _matrixTransform.Matrix;
            matrix.ScaleAt(scaleDelta, scaleDelta, pos.X, pos.Y);
            Scale = matrix.M11;
            //Debug.WriteLine($"x scale factor: {matrix.M11}");
            _matrixTransform.Matrix = matrix;
        }

        #endregion
    }
}