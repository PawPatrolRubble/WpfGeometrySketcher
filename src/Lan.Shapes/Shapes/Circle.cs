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
            DragHandleSize = ShapeStyler.DragHandleSize;
            _dragHandle = new RectDragHandle(DragHandleSize, default, 1);
            RenderGeometryGroup.Children.Add(_dragHandle.HandleGeometry);
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

        private readonly int crossSize = 40;
        private Point _center;

        #endregion

        #region Propeties

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

        private void AddRadiusText(DrawingContext renderContext)
        {
            var lengthInMm = 0.0;
            if (ShapeLayer.UnitsPerMillimeter != 0 && ShapeLayer.PixelPerUnit != 0)
            {
                lengthInMm = Radius * ShapeLayer.UnitsPerMillimeter / ShapeLayer.PixelPerUnit;
            }

            var formattedText = new FormattedText(
                $"{lengthInMm:f4} {ShapeLayer.UnitName}, {Radius:f4} px",
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            renderContext.DrawText(formattedText, new Point(Center.X, Center.Y));
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
        ///     未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDragHandleSizeChanges(double dragHandleSize)
        {
            if (_dragHandle != null)
            {
                _dragHandle.HandleSize = new Size(dragHandleSize, dragHandleSize);
            }
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
            MouseDownPoint = mousePoint;
        }

        public override void FindSelectedHandle(Point p)
        {
            SelectedDragHandle = _dragHandle.FillContains(p) ? _dragHandle : null;
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
                    // Calculate radius as distance from center to current point
                    var dx = point.X - OldPointForTranslate.Value.X;
                    var dy = point.Y - OldPointForTranslate.Value.Y;
                    Radius = Math.Sqrt(dx * dx + dy * dy);
                    // Don't update OldPointForTranslate during drawing - we need the center point
                }
                else if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleResizing(point);
                    // HandleResizing updates OldPointForTranslate internally
                }
                else if (IsGeometryRendered)
                {
                    // If already dragging, continue. Otherwise check if mouse down was inside the circle
                    if (IsBeingDraggedOrPanMoving || (MouseDownPoint.HasValue && _ellipseGeometry.FillContains(MouseDownPoint.Value)))
                    {
                        IsBeingDraggedOrPanMoving = true;
                        HandleTranslate(point);
                        // HandleTranslate updates OldPointForTranslate internally
                    }
                }
            }
        }

        /// \u003csummary\u003e
        /// Handle mouse left button up - clean up state
        /// \u003c/summary\u003e
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            base.OnMouseLeftButtonUp(newPoint);
            // Clear mouse tracking points to prevent stale state
            OldPointForTranslate = null;
            MouseDownPoint = null;
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
        protected void UpdateGeometryGroup([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(Center):
                    _ellipseGeometry.Center = Center;
                    _dragHandle.GeometryCenter = Center + new Vector(Radius, 0);
                    _verticalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(0, -crossSize * 1.0 / 2);
                    _verticalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(0, crossSize * 1.0 / 2);

                    _horizontalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(-crossSize * 1.0 / 2, 0);
                    _horizontalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(crossSize * 1.0 / 2, 0);
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
                AddRadiusText(renderContext);
            }

            renderContext.Close();
        }

        #endregion
    }
}