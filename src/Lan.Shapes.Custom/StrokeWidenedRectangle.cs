using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Custom
{
    public class StrokeWidenedRectangle : ShapeVisualBase
    {

        #region fields

        private double _distance = 50;
        private RectangleGeometry _outerRectangleGeometry = new RectangleGeometry();
        private RectangleGeometry _middleRectangleGeometry = new RectangleGeometry();
        private RectangleGeometry _innerRectangleGeometry = new RectangleGeometry();



        #endregion


        #region constructor

        public StrokeWidenedRectangle()
        {
            RenderGeometryGroup.Children.Add(_outerRectangleGeometry);
            RenderGeometryGroup.Children.Add(_middleRectangleGeometry);
            RenderGeometryGroup.Children.Add(_innerRectangleGeometry);
        }
        #endregion

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



        private Point _rectStartPosition;
        private Point OuterStartPosition => _rectStartPosition - new Vector(_distance / 2, _distance / 2);
        private Point InnerStartPosition => _rectStartPosition + new Vector(_distance / 2, _distance / 2);

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                _rectStartPosition = mousePoint;
                _middleRectangleGeometry.Rect = new Rect(mousePoint,new Size(50,50));
                _outerRectangleGeometry.Rect = new Rect(OuterStartPosition,new Size(10+_distance,10+_distance));
                _innerRectangleGeometry.Rect = new Rect(InnerStartPosition,new Size(10,10));

                UpdateVisual();
            }
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (IsGeometryRendered)
            {
            }
            else
            {
                _middleRectangleGeometry.Rect = new Rect(_rectStartPosition, point - _rectStartPosition);
                _innerRectangleGeometry.Rect = new Rect(InnerStartPosition, point - InnerStartPosition-new Vector(_distance/2, _distance/2));
                _outerRectangleGeometry.Rect = new Rect(OuterStartPosition, point - OuterStartPosition+ new Vector(_distance/2,_distance/2));
                UpdateVisual();
            }
        }



        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
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
            
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}
