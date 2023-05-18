#region

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;
using Newtonsoft.Json.Linq;

#endregion

namespace Lan.Shapes.Custom
{
    public class ThickenedCircle : CustomGeometryBase, IDataExport<EllipseData>
    {
        #region fields

        private readonly DragHandle _resizeHandle;

        private Point _center;

        private readonly EllipseGeometry _middleEllipseGeometry = new EllipseGeometry();
        private double _radius;

        #endregion

        #region Constructors

        #region constructor

        public ThickenedCircle()
        {
            RenderGeometryGroup.Children.Add(_middleEllipseGeometry);
            _resizeHandle = new RectDragHandle(10, Center + new Vector(Radius, 0), 1);
        }

        #endregion

        #endregion

        #region properties


        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            //update handle position
            if (Radius > 0)
            {
                DistanceResizeHandle.GeometryCenter = Center + new Vector(0, -(Radius + StrokeThickness / 2));
                UpdateVisual();
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
            DistanceResizeHandle.GeometryCenter = Center + new Vector(0, -(Radius + StrokeThickness / 2));
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
                        case 99:

                            if (OldPointForTranslate != null)
                            {
                                StrokeThickness += -(point - OldPointForTranslate).Value.Y;
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



        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            Pen ??= ShapeStyler?.SketchPen.CloneCurrentValue();

            if (ShapeStyler != null && Pen != null)
            {
                Pen.Brush.Opacity = 0.5;
                Pen.Thickness = StrokeThickness;
                renderContext.DrawGeometry(ShapeStyler.FillColor, Pen, RenderGeometry);
            }


            renderContext.DrawGeometry(DragHandleFillColor, DragHandlePen, _resizeHandle.HandleGeometry);
            renderContext.DrawGeometry(DragHandleFillColor, DragHandlePen,
                DistanceResizeHandle.HandleGeometry);

            renderContext.Close();
        }


        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                SetMouseCursorToHand();
                Center += newPoint - OldPointForTranslate.Value;
                _resizeHandle.GeometryCenter += newPoint - OldPointForTranslate.Value;
                DistanceResizeHandle.GeometryCenter += newPoint - OldPointForTranslate.Value;
                OldPointForTranslate = newPoint;
            }
        }

        public void FromData(EllipseData data)
        {
            Center = data.Center;
            Radius = data.RadiusX;
            StrokeThickness = data.StrokeThickness;
            IsGeometryRendered = true;
        }

        public EllipseData GetMetaData()
        {
            return new EllipseData()
            {
                Center = Center,
                RadiusX = Radius,
                RadiusY = Radius,
                StrokeThickness = StrokeThickness
            };
        }


        #endregion
    }
}