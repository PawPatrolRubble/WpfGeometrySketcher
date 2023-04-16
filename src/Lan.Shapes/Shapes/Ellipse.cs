using Lan.Shapes.Handle;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace Lan.Shapes.Shapes
{
    public class Ellipse : ShapeVisualBase
    {

        #region fields

        private EllipseGeometry _ellipseGeometry;


        private double _mouseDownRadiusX;
        private double _mouseDownRadiusY;


        #endregion




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
            base.OnMouseLeftButtonDown(newPoint);

            if (IsGeometryRendered)
            {
                _mouseDownRadiusX = _ellipseGeometry.RadiusX;
                _mouseDownRadiusY = _ellipseGeometry.RadiusY;
            }

            if (!IsGeometryRendered)
            {
                if (MouseDownPoint != null)
                    _ellipseGeometry = new EllipseGeometry(new Rect(MouseDownPoint.Value, newPoint));
            }

        }


        /// <summary>
        /// 鼠标点击
        /// </summary>
        //public override void OnMouseMove(Point point, MouseButtonState buttonState)
        //{

        //    if ((_handleGeometry?.FillContains(point) ?? false) && buttonState == MouseButtonState.Released)
        //    {
        //        var handle = FindDragHandleMouseOver(point);
        //        if (handle != null) UpdateMouseCursor((DragLocation)handle.Id);
        //    }


        //    if (_panSensitiveArea.FillContains(point))
        //    {
        //        Mouse.SetCursor(Cursors.Hand);
        //        _canMoveWithHand = true;
        //    }
        //    else
        //    {
        //        _canMoveWithHand = false;
        //    }


        //    if (buttonState == MouseButtonState.Pressed)
        //    {
        //        if (IsGeometryInitialized)
        //        {
        //            if (SelectedDragHandle != null)
        //            {
        //                HandleGeometryScaling(point);
        //                CreateHandles();
        //                AddGeometriesToRender();
        //                UpdateVisual();
        //            }

        //            if (_canMoveWithHand)
        //            {
        //                HandleTranslate(point);
        //                CreateHandles();
        //                AddGeometriesToRender();
        //                UpdateVisual();
        //            }
        //        }
        //        else
        //        {
        //            DrawGeometry(point);
        //            CreateHandles();
        //            AddGeometriesToRender();
        //            UpdateVisual();
        //        }

        //    }

        //    _oldPoint = point;
        //}

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
            RenderGeometryGroup.Children.Clear();
            RenderGeometryGroup.Children.Add(_ellipseGeometry);

            RenderGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            _ellipseGeometry.RadiusX = (newPoint.X - oldPoint.X) / 2;
            _ellipseGeometry.RadiusY = (newPoint.Y - oldPoint.Y) / 2;
        }

        protected override void HandleResizing(Point point)
        {
            switch ((DragLocation)SelectedDragHandle!.Id)
            {
                case DragLocation.TopLeft:

                    break;
                case DragLocation.TopMiddle:
                    if (MouseDownPoint != null)
                        _ellipseGeometry.RadiusY = _mouseDownRadiusY + OldPointForTranslate.Value.Y - point.Y;
                    break;
                case DragLocation.TopRight:
                    break;
                case DragLocation.RightMiddle:
                    if (MouseDownPoint != null)
                        _ellipseGeometry.RadiusX = _mouseDownRadiusX +   point.X - OldPointForTranslate.Value.X;
                    break;
                case DragLocation.BottomRight:
                    break;
                case DragLocation.BottomMiddle:
                    if (MouseDownPoint != null)
                        _ellipseGeometry.RadiusY = _mouseDownRadiusY + point.Y - OldPointForTranslate.Value.Y;
                    break;
                case DragLocation.BottomLeft:
                    break;
                case DragLocation.LeftMiddle:
                    if (MouseDownPoint != null)
                        _ellipseGeometry.RadiusX = _mouseDownRadiusX + OldPointForTranslate.Value.X - point.X;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (MouseDownPoint.HasValue)
            {
                var matrix = new Matrix();
                matrix.Translate(newPoint.X - MouseDownPoint.Value.X, newPoint.Y - MouseDownPoint.Value.Y);
               _ellipseGeometry.Center = matrix.Transform(_ellipseGeometry.Center);
            }

            MouseDownPoint = newPoint;
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
            Handles.Clear();

            Handles.Add(new CircleDragHandle(
                ShapeStyler.DragHandleSize,
               new Point(_ellipseGeometry.Center.X,
                   _ellipseGeometry.Center.Y - _ellipseGeometry.RadiusY),
                (int)DragLocation.TopMiddle));

            Handles.Add(
                new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                new Point(
                    _ellipseGeometry.Center.X + _ellipseGeometry.RadiusX,
                    _ellipseGeometry.Center.Y),
                (int)DragLocation.RightMiddle));

            Handles.Add(new CircleDragHandle(
                ShapeStyler.DragHandleSize,
                new Point(_ellipseGeometry.Center.X,
                    _ellipseGeometry.Center.Y + _ellipseGeometry.RadiusY),
                (int)DragLocation.BottomMiddle));

            Handles.Add(
                new CircleDragHandle(
                    ShapeStyler.DragHandleSize,
                    new Point(
                        _ellipseGeometry.Center.X - _ellipseGeometry.RadiusX,
                        _ellipseGeometry.Center.Y),
                    (int)DragLocation.LeftMiddle));

            HandleGeometryGroup ??= new GeometryGroup();
            HandleGeometryGroup.Children.Clear();
            HandleGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));

            PanSensitiveArea = new CombinedGeometry(GeometryCombineMode.Exclude, RenderGeometryGroup, HandleGeometryGroup);


        }



    }
}
