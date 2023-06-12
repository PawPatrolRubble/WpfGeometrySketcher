#region

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

#endregion

namespace Lan.Shapes.Custom
{
    public class FixedCenterCircle : ShapeVisualBase, IDataExport<EllipseData>
    {
        #region fields

        private readonly EllipseGeometry _baseEllipseGeometry;

        private readonly Rect _boundsRect;
        private readonly DragHandle _rightDragHandle; //= new RectDragHandle(10, default, 1);
        private readonly DragHandle _topDragHandle; // = new RectDragHandle(10, default, 2);


        private Point _center;

        private double _radius;

        #endregion

        #region Propeties

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => _boundsRect;
        }

        public Point Center
        {
            get => _center;
            set
            {
                _center = value;
                OnCenterChanges(_center);
            }
        }

        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                OnRadiusChanged(_radius);
            }
        }

        #endregion

        #region Constructors

        public FixedCenterCircle(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            _baseEllipseGeometry = new EllipseGeometry(new Point(), 100, 100);
            RenderGeometryGroup.Children.Add(_baseEllipseGeometry);
            _rightDragHandle = new RectDragHandle(DragHandleSize, default, 1);
            _topDragHandle = new RectDragHandle(DragHandleSize, default, 2);
        }

        #endregion

        #region Implementations

        public void FromData(EllipseData data)
        {
            throw new NotImplementedException();
        }

        public EllipseData GetMetaData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region local methods

        protected override void CreateHandles()
        {
            throw new NotImplementedException();
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

        protected override void HandleResizing(Point point)
        {
            if (MouseDownPoint != null && OldPointForTranslate != null)
            {
                switch (SelectedDragHandle!.Id)
                {
                    case 2:
                        Radius += OldPointForTranslate.Value.Y - point.Y;
                        break;

                    case 1:
                        Radius += point.X - OldPointForTranslate.Value.X;
                        break;
                }
            }

            OldPointForTranslate = point;
        }

        protected override void HandleTranslate(Point newPoint)
        {
            //translation not allowed
        }

        private void OnCenterChanges(Point center)
        {
            _baseEllipseGeometry.Center = center;
            UpdateHandlePosition(center, Radius);
            UpdateVisual();
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
                Radius = 100;
                //Center = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
            MouseDownPoint ??= mousePoint;
        }


        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed && SelectedDragHandle != null)
            {
                IsBeingDraggedOrPanMoving = true;
                HandleResizing(point);
                UpdateVisual();
            }
        }

        private void OnRadiusChanged(double radius)
        {
            _baseEllipseGeometry.RadiusX = radius;
            _baseEllipseGeometry.RadiusY = radius;
            UpdateHandlePosition(Center, Radius);
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        private void UpdateHandlePosition(Point center, double radius)
        {
            _topDragHandle.GeometryCenter = center + new Vector(0, -radius);
            _rightDragHandle.GeometryCenter = center + new Vector(radius, 0);
        }


        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null && _rightDragHandle != null && _topDragHandle != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen,
                    _rightDragHandle.HandleGeometry);

                AddTagText(renderContext, Center);

                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _topDragHandle.HandleGeometry);
            }

            renderContext.Close();
        }

        #endregion
    }
}