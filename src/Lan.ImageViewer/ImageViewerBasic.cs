#region

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    //[TemplatePart(Type = typeof(TextBlock), Name = "TbMousePosition")]
    [TemplatePart(Type = typeof(Button), Name = "BtnFit")]
    [TemplatePart(Type = typeof(Border), Name = "BorderContainer")]
    [TemplatePart(Type = typeof(Line), Name = "VerticalLine")]
    [TemplatePart(Type = typeof(Line), Name = "HorizontalLine")]
    public class ImageViewerBasic : Control, INotifyPropertyChanged
    {
        #region fields

        public static readonly DependencyProperty MouseDoubleClickPositionProperty = DependencyProperty.Register(
            nameof(MouseDoubleClickPosition), typeof(Point), typeof(ImageViewerBasic),
            new FrameworkPropertyMetadata(default(Point))
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
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
            new FrameworkPropertyMetadata(default(double), OnScaleChangedCallback));


        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness), typeof(double), typeof(ImageViewerBasic), new PropertyMetadata(4.0));

        public static readonly DependencyProperty DefaultStrokeThicknessProperty = DependencyProperty.Register(
            "DefaultStrokeThickness", typeof(double), typeof(ImageViewerBasic), new PropertyMetadata(4.0));

        public static readonly DependencyProperty CrossLineColorProperty = DependencyProperty.Register(
            nameof(CrossLineColor), typeof(Brush), typeof(ImageViewerBasic), new PropertyMetadata(Brushes.Lime));

        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register(
            nameof(StrokeDashArray), typeof(DoubleCollection), typeof(ImageViewerBasic),
            new PropertyMetadata(default(DoubleCollection)));


        public static readonly DependencyProperty ShowCrossLineProperty = DependencyProperty.Register(
            nameof(ShowCrossLine), typeof(bool), typeof(ImageViewerBasic), new PropertyMetadata(true));


        private readonly MatrixTransform _matrixTransform = new MatrixTransform();
        private readonly ScaleTransform _scaleTransform = new ScaleTransform();
        private readonly TransformGroup _transformGroup = new TransformGroup();
        private readonly TranslateTransform _translateTransform = new TranslateTransform();
        private Border? _borderContainer;
        private Canvas? _containerCanvas;

        private bool _disablePropertyChangeCallback;
        private Button? _fitButton;
        private Grid? _gridContainer;
        private Line? _horizontalLineGeometry;
        private Image? _image;
        private bool _isImageScaledByMouseWheel;
        private bool _isMouseFirstClick = true;
        private Point? _lastMouseDownPoint;


        private double _localScale;
        private Point? _mousePos;
        //private TextBlock? _textBlock;

        private Line? _verticalLineGeometry;

        #endregion

        #region Propeties

        public Brush CrossLineColor
        {
            get => (Brush)GetValue(CrossLineColorProperty);
            set => SetValue(CrossLineColorProperty, value);
        }


        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public double LocalScale
        {
            get => _localScale;
            set => SetField(ref _localScale, value);
        }


        public Point MouseDoubleClickPosition
        {
            get => (Point)GetValue(MouseDoubleClickPositionProperty);
            set => SetValue(MouseDoubleClickPositionProperty, value);
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

        public bool ShowCrossLine
        {
            get => (bool)GetValue(ShowCrossLineProperty);
            set => SetValue(ShowCrossLineProperty, value);
        }

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public double DefaultStrokeThickness
        {
            get => (double)GetValue(DefaultStrokeThicknessProperty);
            set => SetValue(DefaultStrokeThicknessProperty, value);
        }

        private Point _mousePositionToImage;
        public Point MousePositionToImage
        {
            get => _mousePositionToImage;
            set => SetField(ref _mousePositionToImage, value);
        }

        private string? _pixelValueString;

        public string? PixelValueString
        {
            get => _pixelValueString;
            set { SetField(ref _pixelValueString, value); }
        }

        #endregion

        #region Constructors

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

        #endregion

        #region Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region local methods

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region others

        private void AutoScaleImageToFit(double width, double height, double pixelWidth, double pixelHeight)
        {
            var ratio = CalculateAutoFitRatio(width, height, pixelWidth, pixelHeight);
            LocalScale = ratio;
            var matrix = new Matrix();
            matrix.ScaleAt(
                ratio,
                ratio,
                0,
                0);
            matrix.Translate((width - pixelWidth * ratio) / 2, (height - pixelHeight * ratio) / 2);
            _matrixTransform.Matrix = matrix;
            StrokeThickness = DefaultStrokeThickness;
        }

        private double CalculateAutoFitRatio(double width, double height, double pixelWidth, double pixelHeight)
        {
            return Math.Min(width / pixelWidth, height / pixelHeight);
        }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            _containerCanvas ??= GetTemplateChild("containerCanvas") as Canvas;
            _image = GetTemplateChild("ImageViewer") as Image;

            _gridContainer ??= GetTemplateChild("GridContainer") as Grid;
            //_textBlock ??= GetTemplateChild("TbMousePosition") as TextBlock;
            _fitButton ??= GetTemplateChild("BtnFit") as Button;
            _borderContainer ??= GetTemplateChild("BorderContainer") as Border;
            _horizontalLineGeometry ??= GetTemplateChild("HorizontalLine") as Line;
            _verticalLineGeometry ??= GetTemplateChild("VerticalLine") as Line;

            _transformGroup.Children.Add(_matrixTransform);
            _transformGroup.Children.Add(_scaleTransform);

            if (_borderContainer != null)
            {
                _gridContainer.SizeChanged += (s, e) =>
                {
                    if (_verticalLineGeometry != null && _horizontalLineGeometry != null)
                    {
                        _verticalLineGeometry.X1 = _gridContainer.ActualWidth / 2;
                        _verticalLineGeometry.Y1 = 0;

                        _verticalLineGeometry.X2 = _gridContainer.ActualWidth / 2;
                        _verticalLineGeometry.Y2 = _gridContainer.ActualHeight;

                        _horizontalLineGeometry.X1 = 0;
                        _horizontalLineGeometry.Y1 = _gridContainer.ActualHeight / 2;

                        _horizontalLineGeometry.X2 = _gridContainer.ActualWidth;
                        _horizontalLineGeometry.Y2 = _gridContainer.ActualHeight / 2;
                    }

                    if (PixelHeight != 0 && PixelWidth != 0)
                    {
                        AutoScaleImageToFit(_borderContainer.ActualWidth, _borderContainer.ActualHeight, PixelWidth, PixelHeight);
                    }
                };

                _borderContainer.SizeChanged += (s, e) =>
                {
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

                if (_gridContainer != null)
                {
                    _gridContainer.RenderTransform = _transformGroup;
                }
            }
        }

        private static void OnImageSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageViewer = (ImageViewerBasic)d;
            double pixelWidth = 0;
            double pixelHeight = 0;

            if (e.NewValue is BitmapSource source)
            {
                pixelWidth = source.PixelWidth;
                pixelHeight = source.PixelHeight;
            }

            if (e.NewValue is DrawingImage drawingImage)
            {
                pixelWidth = drawingImage.Width;
                pixelHeight = drawingImage.Height;
            }

            if (imageViewer._borderContainer != null && (Math.Abs(imageViewer.PixelWidth - pixelWidth) > double.Epsilon
                                                         || Math.Abs(imageViewer.PixelHeight - pixelHeight) >
                                                         double.Epsilon))
            {
                Console.WriteLine("auto fit in image source change");
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
            MouseDoubleClickPosition = e.GetPosition(_image);
            //Console.WriteLine($"mouse dbc pos: {MouseDoubleClickPosition}");
        }


        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MousePositionToImage = e.GetPosition(_image);
            var mousePositionRelativeToCanvas = e.GetPosition(_containerCanvas);

            if (ImageSource is BitmapSource image)
            {
                if (image.PixelHeight > (int)MousePositionToImage.Y && image.PixelWidth > (int)MousePositionToImage.X && MousePositionToImage.Y >= 0 && MousePositionToImage.X >= 0)
                {
                    PixelValueString = GetPixelValue(image, (int)MousePositionToImage.X, (int)MousePositionToImage.Y);
                    //_textBlock.Text = $"X:{p.X:f0}, Y:{p.Y:f0}, {pixelValue}";
                }
            }


            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
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

        private static string? GetPixelValue(BitmapSource bitmap, int x, int y)
        {
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel];
            var rect = new Int32Rect(x, y, 1, 1);

            bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

            if (bytes.Length >= 3)
            {
                return $"[{bytes[2]:000}, {bytes[1]:000}, {bytes[0]:000}]";
            }

            return bytes.Length==1 ? $"{bytes[0]:000}" : string.Empty;

            //return string.Join(',', bytes);
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
            LocalScale = matrix.M11;
            //Debug.WriteLine($"x scale factor: {matrix.M11}");
            _matrixTransform.Matrix = matrix;
            StrokeThickness /= scaleDelta;
        }

        #endregion
    }
}