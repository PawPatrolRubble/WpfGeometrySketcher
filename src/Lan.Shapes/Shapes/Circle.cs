using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    public class Circle : ShapeVisualBase, IDataExport<EllipseData>
    {
        #region constructor

        #region Constructors

        public Circle(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            RenderGeometryGroup.Children.Add(_ellipseGeometry);
        }

        #endregion

        #endregion

        #region interface implementation

        public void FromData(EllipseData data)
        {
            Center = data.Center;
            RadiusX = data.RadiusX;
            RadiusY = data.RadiusY;
            IsGeometryRendered = true;
        }

        public EllipseData GetMetaData()
        {
            return new EllipseData
            {
                Center = Center,
                RadiusX = RadiusX,
                RadiusY = RadiusY
            };
        }

        #endregion

        #region fields

        private readonly EllipseGeometry _ellipseGeometry = new EllipseGeometry(default);
        private readonly LineGeometry _verticalLine = new LineGeometry();
        private readonly LineGeometry _horizontalLine = new LineGeometry();

        private readonly int crossSize = 80;
        private Point _center;
        private double _radiusX;
        private double _radiusY;

        #endregion

        #region Propeties

        /// <summary>
        /// </summary>
        public override Rect BoundsRect
        {
            get { return _ellipseGeometry.Bounds; }
        }

        public Point Center
        {
            get { return _center; }
            set
            {
                SetField(ref _center, value);
                _ellipseGeometry.Center = value;
                _verticalLine.StartPoint = new Point(value.X, value.Y) + new Vector(0, -crossSize * 1.0 / 2);
                _verticalLine.EndPoint = new Point(value.X, value.Y) + new Vector(0, crossSize * 1.0 / 2);

                _horizontalLine.StartPoint = new Point(value.X, value.Y) + new Vector(-crossSize * 1.0 / 2, 0);
                _horizontalLine.EndPoint = new Point(value.X, value.Y) + new Vector(crossSize * 1.0 / 2, 0);
            }
        }

        public double RadiusX
        {
            get { return _radiusX; }
            set
            {
                SetField(ref _radiusX, value);
                _ellipseGeometry.RadiusX = value;
            }
        }

        public double RadiusY
        {
            get { return _radiusY; }
            set
            {
                SetField(ref _radiusY, value);
                _ellipseGeometry.RadiusY = value;
            }
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {
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
            if (!MouseDownPoint.HasValue)
            {
                return;
            }

            var matrix = new Matrix();
            matrix.Translate(newPoint.X - MouseDownPoint.Value.X, newPoint.Y - MouseDownPoint.Value.Y);
            Center = matrix.Transform(Center);
            MouseDownPoint = newPoint;
        }

        /// <summary>
        ///     未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     left mouse button down event
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
        }


        /// <summary>
        ///     鼠标点击移动
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
                //else if (SelectedDragHandle != null)
                //{
                //    IsBeingDraggedOrPanMoving = true;
                //    HandleResizing(point);
                //}
                //else
                //{
                //    IsBeingDraggedOrPanMoving = true;
                //    HandleTranslate(point);
                //}

                UpdateVisual();
            }
        }


        /// <summary>
        ///     选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     add geometries to group
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
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _verticalLine);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _horizontalLine);

                AddTagText(renderContext, Center);
            }

            renderContext.Close();
        }

        #endregion
    }
}