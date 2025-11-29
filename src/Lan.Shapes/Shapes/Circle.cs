using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    public class Circle : ShapeVisualBase, IDataExport<EllipseData>
    {
        #region constructor

        private DragHandle _dragHandle;

        #region Constructors

        public Circle(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            _dragHandle = new RectDragHandle(DragHandleSize, default, 1);
            RenderGeometryGroup.Children.Add(_ellipseGeometry);
        }

        #endregion

        #endregion

        #region interface implementation

        public void FromData(EllipseData data)
        {
            X = data.Center.X; 
            Y = data.Center.Y;
            Radius = data.RadiusX;
            IsGeometryRendered = true;
        }

        public EllipseData GetMetaData()
        {
            return new EllipseData
            {
                Center = Center,
                RadiusX = Radius
            };
        }

        #endregion

        #region fields

        private readonly EllipseGeometry _ellipseGeometry = new EllipseGeometry(default);
        private readonly LineGeometry _verticalLine = new LineGeometry();
        private readonly LineGeometry _horizontalLine = new LineGeometry();

        /// <summary>
        /// Default size for the center cross indicator in pixels
        /// </summary>
        private const int DefaultCrossSize = 40;
        private Point _center;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public override Rect BoundsRect
        {
            get { return _ellipseGeometry.Bounds; }
        }

        private double _x;

        public double X
        {
            get => _x;
            set
            {
                SetField(ref _x, value);

                Center = new Point(_x, Center.Y);
            }
        }

        private double _y;

        public double Y
        {
            get => _y;
            set
            {
                SetField(ref _y, value); 
                Center = new Point(Center.X, _y);
            }
        }



        public Point Center
        {
            get { return _center; }
            set
            {
                SetField(ref _center, value);
                UpdateGeometryGroup();
            }
        }


        #region Overrides of Object

        public override string ToString()
        {
            return $"Circle: {Center.X:f0}, {Center.Y:f0}, Radius: {Radius}";
        }

        #endregion


        private double _radius;

        public double Radius
        {
            get { return _radius; }
            set
            {
                SetField(ref _radius, value);
                UpdateGeometryGroup();
            }
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {

        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            Radius = (newPoint.X - oldPoint.X) / 2;
        }

        protected override void HandleResizing(Point point)
        {
            if (MouseDownPoint.HasValue)
            {
                switch (SelectedDragHandle!.Id)
                {
                    case 2:
                        Radius += MouseDownPoint.Value.Y - point.Y;
                        break;

                    case 1:
                        Radius += point.X - MouseDownPoint.Value.X;
                        break;
                }
            }

            MouseDownPoint = point;
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (!OldPointForTranslate.HasValue)
            {
                return;
            }

            var matrix = new Matrix();
            matrix.Translate(newPoint.X - OldPointForTranslate.Value.X, newPoint.Y - OldPointForTranslate.Value.Y);
            var transformedPoint = matrix.Transform(Center);
            X = transformedPoint.X;
            Y = transformedPoint.Y;
            OldPointForTranslate = newPoint;
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
                    Radius = point.X - OldPointForTranslate.Value.X;
                    Radius = point.Y - OldPointForTranslate.Value.Y;
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

            }
        }


        /// <summary>
        ///     add geometries to group
        /// </summary>
        protected void UpdateGeometryGroup([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(Center):
                    _ellipseGeometry.Center = Center;
                    _dragHandle.GeometryCenter = Center + new Vector(Radius, 0);
                    _verticalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(0, -DefaultCrossSize / 2.0);
                    _verticalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(0, DefaultCrossSize / 2.0);

                    _horizontalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(-DefaultCrossSize / 2.0, 0);
                    _horizontalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(DefaultCrossSize / 2.0, 0);
                    UpdateVisual();

                    break;

                case nameof(Radius):

                    if (Radius > 0)
                    {
                        _ellipseGeometry.RadiusX = Radius;
                        _ellipseGeometry.RadiusY = Radius;
                        _dragHandle.GeometryCenter = _ellipseGeometry.Center + new Vector(Radius, 0);
                    }
                    UpdateVisual();
                    break;

            }

        }

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _verticalLine);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _horizontalLine);
                if (_dragHandle?.HandleGeometry != null)
                {
                    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _dragHandle.HandleGeometry);
                }

                AddTagText(renderContext, Center);
            }

            renderContext.Close();
        }

        #endregion
    }
}