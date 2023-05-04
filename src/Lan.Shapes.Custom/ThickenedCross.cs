#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

#endregion

namespace Lan.Shapes.Custom
{
    public class ThickenedCross : CustomGeometryBase, IDataExport<PointsData>
    {
        #region fields

        private readonly CombinedGeometry _combinedGeometry;
        private readonly RectangleGeometry _hRectangleGeometry = new RectangleGeometry();

        private readonly bool _isSquare;
        private readonly RectangleGeometry _vRectangleGeometry = new RectangleGeometry();
        private Point _centerPoint;


        private Dictionary<DragLocations, DragHandle> _dragHandles = new Dictionary<DragLocations, DragHandle>();
        private Point _horizontalTopLeft;
        private Point _verticalBottomRight;
        private Point _verticalTopLeft;

        #endregion

        #region Propeties


        public override Geometry RenderGeometry
        {
            get => _combinedGeometry;
        }

        #endregion

        #region implementations

        public PointsData GetMetaData()
        {
            //todo get the cross points in specific order
            return new PointsData(StrokeThickness, new List<Point>());
        }

        #endregion

        #region others

        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            _dragHandles[DragLocations.ResizeHandle].GeometryCenter = GetResizeHandleLocation();
        }

        protected override void CreateHandles()
        {
            if (Handles.Count == 0)
            {
                Handles.Add(new RectDragHandle(10, _hRectangleGeometry.Rect.Location, (int)DragLocations.HTopLeft));
                Handles.Add(new RectDragHandle(10, _hRectangleGeometry.Rect.BottomRight,
                    (int)DragLocations.HBottomRight));

                Handles.Add(new RectDragHandle(10, _vRectangleGeometry.Rect.BottomRight,
                    (int)DragLocations.VBottomRight));
                Handles.Add(new RectDragHandle(10, _vRectangleGeometry.Rect.TopLeft, (int)DragLocations.VTopLeft));

                Handles.Add(new RectDragHandle(10, GetResizeHandleLocation(), (int)DragLocations.ResizeHandle));

                _dragHandles = Handles.ToDictionary(x => (DragLocations)x.Id);

                RenderGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));
            }
        }

        private Point GetResizeHandleLocation()
        {
            return _vRectangleGeometry.Rect.Location +
                   new Vector(_vRectangleGeometry.Rect.Width / 2, -StrokeThickness / 2);
        }


        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                var offset = newPoint - OldPointForTranslate.Value;
                VerticalTopLeft += offset;
                VerticalBottomRight += offset;

                HorizontalBottomRight += offset;
                HorizontalTopLeft += offset;

                UpdateHandleLocation();
                UpdateVisual();
                OldPointForTranslate = newPoint;
            }
        }


        /// <summary>
        ///     left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
                VerticalTopLeft = mousePoint;
            else
                FindSelectedHandle(mousePoint);
            OldPointForTranslate = mousePoint;
        }


        /// <summary>
        ///     鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    VerticalBottomRight = point;
                    UpdateVisual();
                }
                else if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;

                    switch ((DragLocations)SelectedDragHandle.Id)
                    {
                        case DragLocations.VTopLeft:
                            VerticalTopLeft = point;
                            break;
                        case DragLocations.VBottomRight:
                            VerticalBottomRight = point;
                            break;
                        case DragLocations.HTopLeft:

                            HorizontalTopLeft = point;

                            break;
                        case DragLocations.HBottomRight:

                            HorizontalBottomRight = point;
                            break;
                        case DragLocations.ResizeHandle:

                            if (OldPointForTranslate != null)
                            {
                                var delta = point - OldPointForTranslate.Value;
                                StrokeThickness += -delta.Y;
                                OldPointForTranslate = point;
                            }

                            break;
                    }

                    UpdateHandleLocation();
                    UpdateVisual();
                }
                else
                {
                    Mouse.SetCursor(Cursors.Hand);
                    HandleTranslate(point);
                }
            }
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

            renderContext.DrawGeometry(DragHandleFillColor, DragHandlePen, RenderGeometryGroup);
            renderContext.Close();
        }

        private enum DragLocations
        {
            None = 0,
            VTopLeft,
            VTopRight,
            VBottomLeft,
            VBottomRight,
            HTopLeft,
            HTopRight,
            HBottomLeft,
            HBottomRight,
            ResizeHandle
        }

        #endregion


        #region properties

        private Point CenterPoint
        {
            get => _centerPoint;
            set
            {
                _centerPoint = value;
                UpdateHorizontalGeometryCornerPoints();
            }
        }


        public Point VerticalTopLeft
        {
            get => _verticalTopLeft;
            set
            {
                _verticalTopLeft = value;
                if (!IsGeometryRendered)
                {
                    CreateGeometry(_verticalTopLeft);
                }
                else
                {
                    HandleDragging(_verticalTopLeft, DragLocations.VTopLeft);
                    UpdateCenter();
                }
            }
        }


        public Point VerticalBottomRight
        {
            get => _verticalBottomRight;
            set
            {
                _verticalBottomRight = value;
                HandleDragging(_verticalBottomRight, DragLocations.VBottomRight);
                UpdateCenter();
            }
        }


        public Point HorizontalTopLeft
        {
            get => _horizontalTopLeft;
            set
            {
                _horizontalTopLeft = value;
                HandleDragging(_horizontalTopLeft, DragLocations.HTopLeft);
            }
        }


        private Point _horizontalBottomRight;

        public Point HorizontalBottomRight
        {
            get => _horizontalBottomRight;
            set
            {
                _horizontalBottomRight = value;
                HandleDragging(_horizontalBottomRight, DragLocations.HBottomRight);
            }
        }

        #endregion


        #region constructor

        public ThickenedCross(bool enableHandleGeneration, bool isSquare)
        {
            _isSquare = isSquare;
            _combinedGeometry =
                new CombinedGeometry(GeometryCombineMode.Union, _hRectangleGeometry, _vRectangleGeometry);
        }

        public ThickenedCross() : this(true, false)
        {
        }

        #endregion


        #region local methods



        private void UpdateCenter()
        {
            //update center point
            CenterPoint = new Point(
                (_vRectangleGeometry.Rect.TopLeft.X + _vRectangleGeometry.Rect.BottomRight.X) / 2,
                (_vRectangleGeometry.Rect.TopLeft.Y + _vRectangleGeometry.Rect.BottomRight.Y) / 2);
        }


        /// <summary>
        ///     update horizontal top left and bottom right when center changes
        ///     as it must be symmetric to the center
        /// </summary>
        private void UpdateHorizontalGeometryCornerPoints()
        {
            //get width and height of horizontal geometry
            var width = _hRectangleGeometry.Rect.Width / 2;
            var height = _hRectangleGeometry.Rect.Height / 2;

            if (!IsGeometryRendered)
            {
                width = _vRectangleGeometry.Rect.Height / 2;
                height = _vRectangleGeometry.Rect.Width / 2;
            }


            HorizontalTopLeft = CenterPoint -
                                new Vector(width, height);
            HorizontalBottomRight = CenterPoint +
                                    new Vector(width, height);
        }


        private void CreateGeometry(Point topLeft)
        {
            _vRectangleGeometry.Rect = new Rect(topLeft, new Size());
        }


        private void HandleDragging(Point mousePoint, DragLocations location)
        {
            switch (location)
            {
                case DragLocations.None:
                    break;
                case DragLocations.VTopLeft:

                    _vRectangleGeometry.Rect = new Rect(mousePoint, _vRectangleGeometry.Rect.BottomRight);

                    UpdateHTopLeft();
                    UpdateHBottomRight();
                    _hRectangleGeometry.Rect = new Rect(HorizontalTopLeft, HorizontalBottomRight);

                    break;
                case DragLocations.VTopRight:
                    break;
                case DragLocations.VBottomLeft:
                    break;
                case DragLocations.VBottomRight:

                    _vRectangleGeometry.Rect = new Rect(_vRectangleGeometry.Rect.Location, mousePoint);
                    UpdateCenter();

                    CreateHandles();

                    break;
                case DragLocations.HTopLeft:

                    //
                    if (SelectedDragHandle?.Id == (int)DragLocations.HTopLeft)
                    {
                        //change bottom right also
                        var vectorToCenter = mousePoint - CenterPoint;
                        HorizontalBottomRight = CenterPoint - vectorToCenter;
                    }

                    _hRectangleGeometry.Rect = new Rect(mousePoint, HorizontalBottomRight);

                    break;
                case DragLocations.HTopRight:
                    break;
                case DragLocations.HBottomLeft:
                    break;
                case DragLocations.HBottomRight:

                    if (SelectedDragHandle?.Id == (int)DragLocations.HBottomRight)
                    {
                        //change top left also
                        var vectorToCenter = mousePoint - CenterPoint;
                        HorizontalTopLeft = CenterPoint - vectorToCenter;
                    }

                    _hRectangleGeometry.Rect = new Rect(HorizontalTopLeft, mousePoint);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }

            UpdateHandleLocation();
        }

        private void UpdateHandleLocation()
        {
            if (Handles.Count > 0)
            {
                _dragHandles[DragLocations.VTopLeft].GeometryCenter = _vRectangleGeometry.Rect.Location;
                _dragHandles[DragLocations.VBottomRight].GeometryCenter = _vRectangleGeometry.Rect.BottomRight;

                _dragHandles[DragLocations.HTopLeft].GeometryCenter = _hRectangleGeometry.Rect.Location;
                _dragHandles[DragLocations.HBottomRight].GeometryCenter = _hRectangleGeometry.Rect.BottomRight;
                _dragHandles[DragLocations.ResizeHandle].GeometryCenter = GetResizeHandleLocation();
            }
        }


        private void UpdateHBottomRight()
        {
            if (_isSquare)
            {
                var width = _vRectangleGeometry.Rect.Width;
                HorizontalBottomRight = HorizontalTopLeft + new Vector(_vRectangleGeometry.Rect.Height, width);
            }

            if (!IsGeometryRendered)
                HorizontalBottomRight = VerticalBottomRight + new Vector(15, -_vRectangleGeometry.Rect.Width);
        }

        private void UpdateHTopLeft()
        {
            if (_isSquare)
            {
                var width = _vRectangleGeometry.Rect.Width;
                var height = _vRectangleGeometry.Rect.Height;
                HorizontalTopLeft = _vRectangleGeometry.Rect.Location +
                                    new Vector(-(height - width) / 2, (height - width) / 2);
            }

            //set default size of 
            if (!IsGeometryRendered)
                HorizontalTopLeft = VerticalTopLeft + new Vector(-15, _vRectangleGeometry.Rect.Width);
        }

        #endregion
    }
}