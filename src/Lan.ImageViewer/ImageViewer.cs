using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lan.ImageViewer
{
    [TemplatePart(Type = typeof(Canvas), Name = "containerCanvas")]
    [TemplatePart(Type = typeof(Image), Name = "ImageViewer")]
    [TemplatePart(Type = typeof(Grid), Name = "GridContainer")]
    [TemplatePart(Type = typeof(TextBlock), Name = "TbMousePosition")]

    public class ImageViewer : Control
    {


        private Point? _mousePos;
        private MatrixTransform _matrixTransform = new MatrixTransform();
        private ScaleTransform _scaleTransform = new ScaleTransform();
        private Canvas _containerCanvas;
        private Image _image;
        private Grid _gridContainer;
        private TransformGroup _transformGroup = new TransformGroup();
        private TextBlock _textBlock;
        private double _totalScale;
        private bool _isMouseFirstClick = true;

        #region binding properties



        #endregion


        #region dependency properties

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata(default(ImageSource)));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
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



        private bool _isImageScaledByMouseWheel = false;
        /// <summary>Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged" /> event, using the specified information as part of the eventual event data.</summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ImageSource is BitmapSource bitmap && !_isImageScaledByMouseWheel)
            {
                var ratio = AutoScaleImageToFit(
                    sizeInfo.NewSize.Width, 
                    sizeInfo.NewSize.Height, 
                    bitmap.PixelWidth,
                    bitmap.PixelHeight);

                var matrix = new Matrix();
                matrix.ScaleAt(
                    ratio, 
                    ratio,
                    (sizeInfo.NewSize.Width- bitmap.PixelWidth * ratio)/2, 
                    (sizeInfo.NewSize.Height - bitmap.PixelHeight * ratio)/2);

                _matrixTransform.Matrix = matrix;

            }
        }


        private double AutoScaleImageToFit(double width, double height, double pixelWidth, double pixelHeight)
        {
            return Math.Min(width / pixelWidth, height / pixelHeight);
        }



        #region events handlers

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised on this element. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
                //capture the mouse, even when the mouse is not above the control, the mouse events will still be fired
                CaptureMouse();
                _mousePos = e.GetPosition(this);
            //}
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
            var pos = e.GetPosition(this);
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;

            _totalScale += scale;
            ScaleGridContainer(scale, pos);
            _isImageScaledByMouseWheel = true;
        }


        private void ScaleGridContainer(double scale, Point pos)
        {
            var matrix = _matrixTransform.Matrix;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            _matrixTransform.Matrix = matrix;
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
