using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Custom
{
    public class StrokeWidenedCross : ShapeVisualBase
    {

        #region fields

        private readonly RectangleGeometry _verticalMiddleRectangleGeometry =new RectangleGeometry(new Rect(new Size()));
        private readonly RectangleGeometry _verticalInnerRectangleGeometry =new RectangleGeometry(new Rect(new Size()));
        private readonly RectangleGeometry _verticalOuterRectangleGeometry =new RectangleGeometry(new Rect(new Size()));
        
        private readonly RectangleGeometry _horizontalMiddleRectangleGeometry= new RectangleGeometry(new Rect(new Size()));
        private readonly RectangleGeometry _horizontalInnerRectangleGeometry= new RectangleGeometry(new Rect(new Size()));
        private readonly RectangleGeometry _horizontalOuterRectangleGeometry= new RectangleGeometry(new Rect(new Size()));
        
        private readonly CombinedGeometry _combinedGeometry;

        #endregion


        #region properties

        private Point VerticalTopLeft
        {
            get => _topLeft;
            set
            {
                _topLeft = value;
                UpdateTopLeftLocationForGeometry( _topLeft);
                UpdateVisual();
            }
        }

        public Point VerticalBottomRight { get; set; }



        private Point _bottomRight;
        private Point _topLeft;

        public Point HorizontalBottomRight
        {
            get => _bottomRight;
            set
            {
                _bottomRight = value;
                UpdateBottomRightForGeometry( _bottomRight);
            }
        }

        public Point HorizontalTopLeft { get; set; }



        private void UpdateTopLeftLocationForGeometry(Point topLeft) {
            _verticalRectangleGeometry.Rect = new Rect(topLeft, new Size());
            _horizontalRectangleGeometry.Rect = new Rect(topLeft, new Size());
        }

        private void UpdateBottomRightForGeometry(Point bottomRight)
        {
            _verticalRectangleGeometry.Rect = new Rect(VerticalTopLeft, new Size());
            _horizontalRectangleGeometry.Rect = new Rect(topl, new Size());
        }

        #endregion

        #region ctor

        public StrokeWidenedCross()
        {
            _combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, _verticalRectangleGeometry,_horizontalRectangleGeometry);
            RenderGeometryGroup.Children.Add(_combinedGeometry);
        }

        #endregion

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {

        }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
               
            }
            UpdateVisual();
        }


        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
            throw new NotImplementedException();
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            throw new NotImplementedException();
        }

        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }

        protected override void CreateHandles()
        {
            throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}
