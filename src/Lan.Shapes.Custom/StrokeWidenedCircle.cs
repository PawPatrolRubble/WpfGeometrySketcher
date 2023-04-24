using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;

namespace Lan.Shapes.Custom {
    public class StrokeWidenedCircle : ShapeVisualBase {
        #region fields

        private Pen _middleGeometrySketchPen = new Pen(Brushes.Red, 1);
        private EllipseGeometry _innerGeometry = new EllipseGeometry();
        private EllipseGeometry _outerGeometry = new EllipseGeometry();
        private EllipseGeometry _middleEllipseGeometry = new EllipseGeometry();
        private Point _center;
        private double _radius;
        private readonly Rect _boundsRect;
        private double _distance;
        private readonly CircleDragHandle _resizeHandle;
        private readonly CircleDragHandle _distanceAdjustmentHandle;



        #endregion

        #region properties

        private double Distance {
            get => _distance;
            set {
                _distance = value;
                if (Radius > 0) {

                    ResizeGeometry(Radius);
                    UpdateVisual();
                }
            }
        }


        private Point Center {
            get => _center;
            set {
                _center = value;
                CreateOrTranslateGeometry(_center);
                UpdateVisual();
            }
        }

        private void CreateOrTranslateGeometry(Point center) {
            _outerGeometry.Center = center;
            _innerGeometry.Center = center;
            _middleEllipseGeometry.Center = center;
        }

        private double Radius {
            get => _radius;
            set {
                _radius = value;
                ResizeGeometry(_radius);
                UpdateVisual();
            }
        }

        private void ResizeGeometry(double radius) {
            _innerGeometry.RadiusX = radius - Distance / 2;
            _innerGeometry.RadiusY = radius - Distance / 2;
            _middleEllipseGeometry.RadiusX = radius;
            _middleEllipseGeometry.RadiusY = radius;
            _outerGeometry.RadiusX = radius + Distance / 2;
            _outerGeometry.RadiusY = radius + Distance / 2;

            _resizeHandle.GeometryCenter = Center + new Vector(Radius, 0);
            _distanceAdjustmentHandle.GeometryCenter = Center + new Vector(Radius + Distance / 2, 0);
        }

        #endregion


        #region constructor
        public StrokeWidenedCircle() {
            Distance = 30;
            RenderGeometryGroup.Children.Add(_outerGeometry);
            RenderGeometryGroup.Children.Add(_innerGeometry);
            _resizeHandle = new CircleDragHandle(5, Center + new Vector(Radius, 0), 1);
            _distanceAdjustmentHandle = new CircleDragHandle(5, Center + new Vector(Radius, 0), 2);

            _middleGeometrySketchPen.DashStyle = DashStyles.Dash;
        }

        #endregion

        #region implementation

        public override void OnMouseLeftButtonDown(Point mousePoint) {
            if (!IsGeometryRendered) {
                Center = mousePoint;
                MouseDownPoint = mousePoint;
            }
            else {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState) {
            if (buttonState == MouseButtonState.Pressed) {
                if (!IsGeometryRendered) {
                    if (MouseDownPoint != null)
                        Radius = GetDistanceBetweenTwoPoint(point, MouseDownPoint.Value);
                }
                else if (SelectedDragHandle != null) {
                    IsBeingDraggedOrPanMoving = true;
                    switch (SelectedDragHandle.Id) {
                        case 2:

                            if (OldPointForTranslate != null) {
                                Distance += (point - OldPointForTranslate).Value.X;
                                OldPointForTranslate = point;
                            }
                            break;
                        case 1:
                            if (OldPointForTranslate != null) {
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


        public override void FindSelectedHandle(Point p) {
            if (_resizeHandle.HandleGeometry.FillContains(p)) {
                SelectedDragHandle = _resizeHandle;
            }

            if (_distanceAdjustmentHandle.HandleGeometry.FillContains(p)) {
                SelectedDragHandle = _distanceAdjustmentHandle;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect => _boundsRect;

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup() {
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint) {
            throw new NotImplementedException();
        }

        protected override void HandleResizing(Point point) {
            throw new NotImplementedException();
        }

        public override void UpdateVisual() {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);

            renderContext.DrawGeometry(Brushes.Aquamarine,_middleGeometrySketchPen,_resizeHandle.HandleGeometry);
            renderContext.DrawGeometry(Brushes.Aquamarine,_middleGeometrySketchPen,_distanceAdjustmentHandle.HandleGeometry);
            renderContext.DrawGeometry(Brushes.Transparent, _middleGeometrySketchPen, _middleEllipseGeometry);
            renderContext.Close();
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected() {
            throw new NotImplementedException();
        }

        protected override void CreateHandles() {

        }

        protected override void HandleTranslate(Point newPoint) {
            if (OldPointForTranslate.HasValue)
            {
                Center += (newPoint - OldPointForTranslate.Value);
                _resizeHandle.GeometryCenter += (newPoint - OldPointForTranslate.Value);
                _distanceAdjustmentHandle.GeometryCenter += (newPoint - OldPointForTranslate.Value);
                OldPointForTranslate = newPoint;
            }
        }

        #endregion
    }
}