using Lan.Shapes.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;


namespace Lan.Shapes.Custom
{
    public class StrokeWidenedRectangle : ShapeVisualBase
    {

        #region fields

        private readonly RectangleGeometry _outerRectangleGeometry = new RectangleGeometry();
        private readonly RectangleGeometry _middleRectangleGeometry = new RectangleGeometry();
        private readonly RectangleGeometry _innerRectangleGeometry = new RectangleGeometry();

        private readonly Pen _middleGeometryPen = new Pen(Brushes.Red, 1);
        private readonly CombinedGeometry _combinedGeometry;
        private Vector _offsetVector;
        private Dictionary<DragLocation, DragHandle> _dragHandleDict;

        private readonly DragHandle _distanceResizeHandle = new RectDragHandle(new Size(10, 10), new Point(), 10, 99);

        private GeometryCombineMode _combinationMode = GeometryCombineMode.Xor;

        #endregion


        #region properties

        private Point _topLeft;

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                _topLeft = value;
                if (_topLeft != default)
                {
                    if (!IsGeometryRendered)
                    {
                        CreateGeometry(_topLeft);
                    }
                    else
                    {
                        ResizeByCornerPoint(DragLocation.TopLeft, _topLeft);
                    }

                    UpdateVisual();
                }
            }
        }



        private Point _bottomRight;

        public Point BottomRight
        {
            get => _bottomRight;
            set
            {
                _bottomRight = value;
                if (_bottomRight != default)
                {
                    ResizeByCornerPoint(DragLocation.BottomRight, _bottomRight);
                    UpdateVisual();
                }
            }
        }


        private double _distance = 50;
        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                _offsetVector.X = _distance / 2;
                _offsetVector.Y = _distance / 2;

                ChangeDistance();
                UpdateVisual();
            }
        }



        #endregion


        #region constructor



        public StrokeWidenedRectangle() : this(GeometryCombineMode.Exclude)
        {

        }

        public StrokeWidenedRectangle(GeometryCombineMode mode)
        {
            _combinationMode = mode;
            _combinedGeometry =
                new CombinedGeometry(_combinationMode, _outerRectangleGeometry, _innerRectangleGeometry);

            RenderGeometryGroup.Children.Add(_combinedGeometry);

            Distance = 30;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
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
                TopLeft = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
                OldPointForTranslate = mousePoint;
            }

            MouseDownPoint = mousePoint;
        }


        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (!IsGeometryRendered)
            {
                if (buttonState == MouseButtonState.Pressed)
                {
                    BottomRight = point;
                }
            }
            else
            {
                if (buttonState == MouseButtonState.Pressed)
                {

                    IsBeingDraggedOrPanMoving = true;

                    if (SelectedDragHandle != null)
                    {

                        if (SelectedDragHandle.Id == 99)
                        {
                            if (OldPointForTranslate != null)
                            {
                                Distance += (point - OldPointForTranslate.Value).Y;
                                Console.WriteLine($"distance: {Distance}");
                                OldPointForTranslate = point;
                            }
                        }
                        else
                        {
                            switch ((DragLocation)SelectedDragHandle.Id)
                            {
                                case DragLocation.TopLeft:
                                    TopLeft = point;

                                    break;
                                case DragLocation.TopMiddle:
                                    break;
                                case DragLocation.TopRight:
                                    BottomRight = new Point(point.X, BottomRight.Y);
                                    TopLeft = new Point(TopLeft.X, point.Y);
                                    break;
                                case DragLocation.RightMiddle:
                                    break;
                                case DragLocation.BottomRight:
                                    BottomRight = point;
                                    break;
                                case DragLocation.BottomMiddle:
                                    break;
                                case DragLocation.BottomLeft:
                                    TopLeft = new Point(point.X, TopLeft.Y);
                                    BottomRight = new Point(BottomRight.X, point.Y);

                                    break;
                                case DragLocation.LeftMiddle:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                        }
                    }
                    else
                    {

                        //handle translate
                        if (OldPointForTranslate != null)
                        {
                            TopLeft += point - OldPointForTranslate.Value;
                            BottomRight += point - OldPointForTranslate.Value;
                            OldPointForTranslate = point;
                        }
                    }
                }
            }
        }



        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
        }

        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
        }

        public override void FindSelectedHandle(Point p)
        {
            if (_distanceResizeHandle.FillContains(p))
            {
                SelectedDragHandle = _distanceResizeHandle;
                return;
            }
            base.FindSelectedHandle(p);
        }

        private void CreateGeometry(Point topLeft)
        {
            _middleRectangleGeometry.Rect = new Rect(topLeft, new Size());
            _innerRectangleGeometry.Rect = new Rect(topLeft - _offsetVector, new Size());
            _outerRectangleGeometry.Rect = new Rect(topLeft + _offsetVector, new Size());
        }


        /// <summary>
        /// it will affect the topleft and bottom right on both inner and outer geometry
        /// </summary>
        private void ChangeDistance()
        {
            ResizeByCornerPoint(DragLocation.TopLeft, TopLeft);
            ResizeByCornerPoint(DragLocation.BottomRight, BottomRight);
        }


        private void ResizeByCornerPoint(DragLocation location, Point point)
        {
            switch (location)
            {
                case DragLocation.TopLeft:
                    _middleRectangleGeometry.Rect = new Rect(point, BottomRight);
                    _outerRectangleGeometry.Rect = new Rect(point - _offsetVector, BottomRight + _offsetVector);
                    _innerRectangleGeometry.Rect = new Rect(point + _offsetVector, BottomRight - _offsetVector);
                    break;
                case DragLocation.TopMiddle:
                    break;
                case DragLocation.TopRight:
                    break;
                case DragLocation.RightMiddle:
                    break;
                case DragLocation.BottomRight:
                    _middleRectangleGeometry.Rect = new Rect(TopLeft, point);
                    _outerRectangleGeometry.Rect = new Rect(TopLeft - _offsetVector, point + _offsetVector);
                    _innerRectangleGeometry.Rect = new Rect(TopLeft + _offsetVector, point - _offsetVector);
                    break;
                case DragLocation.BottomMiddle:
                    break;
                case DragLocation.BottomLeft:
                    break;
                case DragLocation.LeftMiddle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }

            if (Handles.Count == 0)
            {
                CreateHandles();
            }

            //update location of handle
            _distanceResizeHandle.GeometryCenter = _outerRectangleGeometry.Rect.Location + new Vector(_outerRectangleGeometry.Rect.Width / 2, 0);

            _dragHandleDict[DragLocation.TopLeft].GeometryCenter = TopLeft;
            _dragHandleDict[DragLocation.TopRight].GeometryCenter = new Point(BottomRight.X, TopLeft.Y);
            _dragHandleDict[DragLocation.BottomRight].GeometryCenter = BottomRight;
            _dragHandleDict[DragLocation.BottomLeft].GeometryCenter = new Point(TopLeft.X, BottomRight.Y);
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
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.TopLeft));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.TopRight));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.BottomLeft));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.BottomRight));

            _dragHandleDict = Handles.ToDictionary(x => (DragLocation)x.Id);
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }

        #region local field

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _distanceResizeHandle.HandleGeometry);
            }

            renderContext.DrawGeometry(Brushes.Transparent, _middleGeometryPen, _middleRectangleGeometry);

            foreach (var dragHandle in Handles)
            {
                renderContext.DrawGeometry(Brushes.Transparent, _middleGeometryPen, dragHandle.HandleGeometry);
            }

            renderContext.Close();
        }

        #endregion
    }
}
