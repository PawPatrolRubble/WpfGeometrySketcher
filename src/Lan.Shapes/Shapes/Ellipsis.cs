using Lan.Shapes.Handle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Shapes
{
    public class Ellipsis : ShapeVisual
    {

        #region fields

        private EllipseGeometry _ellipseGeometry = new EllipseGeometry();
        private GeometryGroup _geometryGroup = new GeometryGroup();
        private RectangleGeometry _rectangleGeometry = new RectangleGeometry();
        private GeometryGroup _handleGeometryGroup = new GeometryGroup();

        #endregion



        public override Geometry RenderGeometry { get; }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonDown(Point newPoint)
        {
            
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
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

        public override void CreateHandles()
        {
            Handles.Clear();

            Handles.Add(new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                GetMiddlePoint(BoundsRect.TopLeft, BoundsRect.TopRight),
                (int)DragLocation.TopMiddle));

            Handles.Add(
                new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                GetMiddlePoint(BoundsRect.BottomRight, BoundsRect.TopRight),
                (int)DragLocation.RightMiddle));

            Handles.Add(new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                GetMiddlePoint(BoundsRect.BottomLeft, BoundsRect.BottomRight),
                (int)DragLocation.BottomMiddle));

            Handles.Add(new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                GetMiddlePoint(BoundsRect.TopLeft, BoundsRect.BottomLeft),
                (int)DragLocation.LeftMiddle));


        }


        private Point GetMiddlePoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }


    }
}
