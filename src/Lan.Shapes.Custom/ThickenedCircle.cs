#region

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;

#endregion

namespace Lan.Shapes.Custom
{
    public class ThickenedCircle : CustomGeometryBase, IDataExport<EllipseData>
    {
        #region fields

        private readonly Rect _boundsRect;
        private readonly DragHandle _resizeHandle;

        private Point _center;
        private double _strokeThickness;

        private readonly EllipseGeometry _middleEllipseGeometry = new EllipseGeometry();
        private readonly Pen _middleGeometrySketchPen = new Pen(Brushes.Red, 1);
        private double _radius;

        #endregion

        #region Constructors

        #region constructor

        public ThickenedCircle()
        {
            StrokeThickness = 30;
            RenderGeometryGroup.Children.Add(_middleEllipseGeometry);

            _resizeHandle = new RectDragHandle(10, Center + new Vector(Radius, 0), 1);

            _middleGeometrySketchPen.DashStyle = DashStyles.Dash;
        }

        #endregion

        #endregion

        #region properties

        private double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                _strokeThickness = value;
                _strokeThickness = Math.Min(MaxStrokeThickness, _strokeThickness);
                _strokeThickness = Math.Max(0, _strokeThickness);
                if (Radius > 0)
                {
                    ResizeGeometry(Radius);
                    UpdateVisual();
                }
            }
        }


        private Point Center
        {
            get => _center;
            set
            {
                _center = value;
                CreateOrTranslateGeometry(_center);
                UpdateVisual();
            }
        }

        private void CreateOrTranslateGeometry(Point center)
        {
            _middleEllipseGeometry.Center = center;
        }

        private double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                ResizeGeometry(_radius);
                UpdateVisual();
            }
        }

        private void ResizeGeometry(double radius)
        {
            _middleEllipseGeometry.RadiusX = radius;
            _middleEllipseGeometry.RadiusY = radius;

            _resizeHandle.GeometryCenter = Center + new Vector(Radius, 0);
            DistanceResizeHandle.GeometryCenter = Center + new Vector(Radius + StrokeThickness / 2, 0);
            if (Pen != null)
            {
                Pen.Thickness = StrokeThickness;
            }
        }

        #endregion

        #region implementation

        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                Center = mousePoint;
                MouseDownPoint = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    if (MouseDownPoint != null)
                        Radius = GetDistanceBetweenTwoPoint(point, MouseDownPoint.Value);
                }
                else if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;
                    switch (SelectedDragHandle.Id)
                    {
                        case 2:

                            if (OldPointForTranslate != null)
                            {
                                StrokeThickness += (point - OldPointForTranslate).Value.X;
                                OldPointForTranslate = point;
                            }

                            break;
                        case 1:
                            if (OldPointForTranslate != null)
                            {
                                Radius += (point - OldPointForTranslate).Value.X;
                                OldPointForTranslate = point;
                            }

                            break;
                    }
                }
                else
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleTranslate(point);
                }
            }
        }


        public override void FindSelectedHandle(Point p)
        {
            if (_resizeHandle.HandleGeometry?.FillContains(p) ?? false) SelectedDragHandle = _resizeHandle;

            if (DistanceResizeHandle.HandleGeometry?.FillContains(p) ?? false)
                SelectedDragHandle = DistanceResizeHandle;
        }


        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => _boundsRect;
        }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            throw new NotImplementedException();
        }

        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            Pen ??= ShapeStyler?.SketchPen.CloneCurrentValue();

            if (ShapeStyler != null && Pen != null)
            {
                Pen.Brush.Opacity = 0.5;
                renderContext.DrawGeometry(ShapeStyler.FillColor, Pen, RenderGeometry);
            }


            renderContext.DrawGeometry(Brushes.Aquamarine, _middleGeometrySketchPen, _resizeHandle.HandleGeometry);
            renderContext.DrawGeometry(Brushes.Aquamarine, _middleGeometrySketchPen,
                DistanceResizeHandle.HandleGeometry);

            renderContext.Close();
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
            if (OldPointForTranslate.HasValue)
            {
                Center += newPoint - OldPointForTranslate.Value;
                _resizeHandle.GeometryCenter += newPoint - OldPointForTranslate.Value;
                DistanceResizeHandle.GeometryCenter += newPoint - OldPointForTranslate.Value;
                OldPointForTranslate = newPoint;
            }
        }

        public EllipseData GetMetaData()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}