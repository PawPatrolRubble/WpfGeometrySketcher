using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using HalconDotNet;

using Lan.Shapes.Interfaces;

namespace Lan.ImageViewer.Halcon
{
    [TemplatePart(Type = typeof(Canvas), Name = "containerCanvas")]
    [TemplatePart(Type = typeof(HSmartWindowControlWPF), Name = "ImageViewer")]
    [TemplatePart(Type = typeof(Grid), Name = "GridContainer")]
    public class ImageViewerHalcon : Control, INotifyPropertyChanged
    {
        #region private fields

        private readonly MatrixTransform _matrixTransform = new();
        private readonly ScaleTransform _scaleTransform = new();
        private readonly TransformGroup _transformGroup = new();
        private readonly TranslateTransform _translateTransform = new();
        private Border? _borderContainer;
        private Canvas? _containerCanvas;

        private bool _disablePropertyChangeCallback;
        private Button? _fitButton;
        private Grid? _gridContainer;
        private Line? _horizontalLineGeometry;
        private HSmartWindowControlWPF? _image;
        private bool _isImageScaledByMouseWheel;
        private bool _isMouseFirstClick = true;
        private Point? _lastMouseDownPoint;

        private double _localScale;

        private Point? _mousePos;

        private SketchBoard.SketchBoard? _sketchBoard;
        //private TextBlock? _textBlock;

        private Line? _verticalLineGeometry;

        #endregion

        #region properties

        public double LocalScale
        {
            get { return _localScale; }
            set { SetField(ref _localScale, value); }
        }

        #endregion

        #region interface implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region other members

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _containerCanvas ??= GetTemplateChild("containerCanvas") as Canvas;

            if (_image == null)
            {
                _image = GetTemplateChild("ImageViewer") as HSmartWindowControlWPF;
                _image.MouseMove += (s, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        ImagePart = _image.HImagePart;
                    }
                };
            }

            _gridContainer ??= GetTemplateChild("GridContainer") as Grid;

            _sketchBoard ??= GetTemplateChild("SketchBoard") as SketchBoard.SketchBoard;

        }

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


        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised
        ///     on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event
        ///     data reports that the left mouse button was pressed.
        /// </param>
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
            //MouseDoubleClickPosition = e.GetPosition(_image);
            //Console.WriteLine($"mouse dbc pos: {MouseDoubleClickPosition}");
        }


        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an
        ///     element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
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

            return bytes.Length == 1 ? $"{bytes[0]:000}" : string.Empty;

            //return string.Join(',', bytes);
        }


        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element
        ///     in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event
        ///     data reports that the mouse button was released.
        /// </param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            _mousePos = null;
            _isMouseFirstClick = false;
            base.OnMouseUp(e);
        }

        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel" /> attached event reaches an
        ///     element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
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
        }

        #endregion

        #region dependencyProperties

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items), typeof(ObservableCollection<HObject>), typeof(ImageViewerHalcon),
            new PropertyMetadata(default(ObservableCollection<HObject>)));

        public ObservableCollection<HObject> Items
        {
            get { return (ObservableCollection<HObject>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            nameof(SketchBoardDataManager), typeof(ISketchBoardDataManager), typeof(ImageViewerHalcon),
            new PropertyMetadata(default(ISketchBoardDataManager), OnSketchBoardDataManagerChanged));

        private static void OnSketchBoardDataManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ;
        }

        public ISketchBoardDataManager SketchBoardDataManager
        {
            get { return (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty); }
            set { SetValue(SketchBoardDataManagerProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource), typeof(string), typeof(ImageViewerHalcon), new PropertyMetadata(default(string), OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerHalcon imageViewerHalcon && e.NewValue is string imagePath)
            {
                HOperatorSet.ReadImage(out var image, imagePath);

                var halconImage = new HIconicDisplayObjectWPF();
                halconImage.IconicObject = image;

                imageViewerHalcon._image?.Items.Add(halconImage);
            }
        }

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImagePartProperty = DependencyProperty.Register(
            nameof(ImagePart), typeof(Rect), typeof(ImageViewerHalcon), new PropertyMetadata(default(Rect), OnImagePartChanged));

        private static void OnImagePartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ;
        }

        public Rect ImagePart
        {
            get { return (Rect)GetValue(ImagePartProperty); }
            set { SetValue(ImagePartProperty, value); }
        }


        #endregion
    }
}