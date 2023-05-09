#region

#nullable enable
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

#endregion

namespace Lan.Shapes.Shapes
{
    public class Ellipse : ShapeVisualBase, IDataExport<EllipseData>
    {
        #region fields

        private readonly EllipseGeometry _ellipseGeometry = new EllipseGeometry(default);

        private readonly DragHandle _rightDragHandle = new RectDragHandle(10, default, 1);
        private readonly DragHandle _topDragHandle = new RectDragHandle(10, default, 2);

        private Point _center;
        private double _radiusX;
        private double _radiusY;

        #endregion

        #region Propeties

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        public Point Center
        {
            get => _center;
            set
            {
                SetField(ref _center, value);
                _ellipseGeometry.Center = value;
                _rightDragHandle.GeometryCenter = value + new Vector(RadiusX, 0);
                _topDragHandle.GeometryCenter = value + new Vector(0, -RadiusY);
            }
        }

        public double RadiusX
        {
            get => _radiusX;
            set
            {
                SetField(ref _radiusX, value);
                _ellipseGeometry.RadiusX = value;
                _rightDragHandle.GeometryCenter = Center + new Vector(value, 0);
            }
        }

        public double RadiusY
        {
            get => _radiusY;
            set
            {
                SetField(ref _radiusY, value);
                _ellipseGeometry.RadiusY = value;
                _topDragHandle.GeometryCenter = Center + new Vector(0, -value);
            }
        }

        #endregion

        #region Constructors

        public Ellipse()
        {
            RenderGeometryGroup.Children.Add(_ellipseGeometry);
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {
            //Handles.Clear();

            //Handles.Add(new CircleDragHandle(
            //    ShapeStyler.DragHandleSize,
            //    new Point(_ellipseGeometry.Center.X,
            //        _ellipseGeometry.Center.Y - _ellipseGeometry.RadiusY),
            //    (int)DragLocation.TopMiddle));

            //Handles.Add(
            //    new CircleDragHandle(
            //        ShapeStyler.DragHandleSize,
            //        new Point(
            //            _ellipseGeometry.Center.X + _ellipseGeometry.RadiusX,
            //            _ellipseGeometry.Center.Y),
            //        (int)DragLocation.RightMiddle));

            //Handles.Add(new CircleDragHandle(
            //    ShapeStyler.DragHandleSize,
            //    new Point(_ellipseGeometry.Center.X,
            //        _ellipseGeometry.Center.Y + _ellipseGeometry.RadiusY),
            //    (int)DragLocation.BottomMiddle));

            //Handles.Add(
            //    new CircleDragHandle(
            //        ShapeStyler.DragHandleSize,
            //        new Point(
            //            _ellipseGeometry.Center.X - _ellipseGeometry.RadiusX,
            //            _ellipseGeometry.Center.Y),
            //        (int)DragLocation.LeftMiddle));

            //HandleGeometryGroup ??= new GeometryGroup();
            //HandleGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));

            //PanSensitiveArea =
            //    new CombinedGeometry(GeometryCombineMode.Exclude, RenderGeometryGroup, HandleGeometryGroup);
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            RadiusX = (newPoint.X - oldPoint.X) / 2;
            RadiusY = (newPoint.Y - oldPoint.Y) / 2;
        }

        protected override void HandleResizing(Point point)
        {
            if (MouseDownPoint != null && OldPointForTranslate != null)
            {
                switch (SelectedDragHandle!.Id)
                {
                    case 2:
                        RadiusY += OldPointForTranslate.Value.Y - point.Y;
                        break;

                    case 1:
                        RadiusX += point.X - OldPointForTranslate.Value.X;
                        break;
                }
            }

            OldPointForTranslate = point;
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (!MouseDownPoint.HasValue) return;

            var matrix = new Matrix();
            matrix.Translate(newPoint.X - MouseDownPoint.Value.X, newPoint.Y - MouseDownPoint.Value.Y);
            Center = matrix.Transform(Center);
            MouseDownPoint = newPoint;
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                Center = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
            MouseDownPoint ??= mousePoint;
        }

        public override void FindSelectedHandle(Point p)
        {
            if (_rightDragHandle.FillContains(p))
            {
                SelectedDragHandle = _rightDragHandle;
            }

            if (_topDragHandle.FillContains(p))
            {
                SelectedDragHandle = _topDragHandle;
            }
        }


        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered && OldPointForTranslate.HasValue)
                {
                    RadiusX = (point.X - OldPointForTranslate.Value.X) / 2;
                    RadiusY = (point.Y - OldPointForTranslate.Value.Y) / 2;
                }
                else if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleResizing(point);
                }
                else
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleTranslate(point);
                }

                UpdateVisual();
            }

        }


        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
        }

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen,
                    _rightDragHandle.HandleGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _topDragHandle.HandleGeometry);
            }

            renderContext.Close();
        }

        #endregion

        public void FromData(EllipseData data)
        {
            throw new NotImplementedException();
        }

        public EllipseData GetMetaData()
        {

            return new EllipseData()
            {
                Center = Center,
                RadiusX = RadiusX,
                RadiusY = RadiusY,
            };
        }
    }
}