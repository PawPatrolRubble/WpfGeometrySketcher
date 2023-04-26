#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Shapes;

#endregion

namespace Lan.Shapes.Custom
{
    public class ThickenedRectangle : CustomGeometryBase,IDataExport<PointsData>
    {
        #region fields


        private readonly RectangleGeometry _middleRectangleGeometry = new RectangleGeometry();

        private Point _bottomRight;

        private Dictionary<DragLocation, DragHandle> _dragHandleDict;

        private Vector _offsetVector;


        private double _strokeThickness = 30;


        private Point _topLeft;

        #endregion

        #region Propeties

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

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                _strokeThickness = value;
                _strokeThickness = Math.Min(MaxStrokeThickness, _strokeThickness);
                _strokeThickness = Math.Max(0, _strokeThickness);

                _offsetVector.X = _strokeThickness / 2;
                _offsetVector.Y = _strokeThickness / 2;

                ChangeDistance();
                UpdateVisual();
            }
        }

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                _topLeft = value;
                if (_topLeft != default)
                {
                    if (!IsGeometryRendered)
                        CreateGeometry(_topLeft);
                    else
                        ResizeByCornerPoint(DragLocation.TopLeft, _topLeft);

                    UpdateVisual();
                }
            }
        }

        #endregion

        #region Constructors

        public ThickenedRectangle()
        {
            StrokeThickness = 15;
            RenderGeometryGroup.Children.Add(_middleRectangleGeometry);
        }

        #endregion

        #region others

        /// <summary>
        /// it will affect the topleft and bottom right on both inner and outer geometry
        /// </summary>
        private void ChangeDistance()
        {
            ResizeByCornerPoint(DragLocation.TopLeft, TopLeft);
            ResizeByCornerPoint(DragLocation.BottomRight, BottomRight);
            if (Pen != null) Pen.Thickness = StrokeThickness;
        }

        private void CreateGeometry(Point topLeft)
        {
            _middleRectangleGeometry.Rect = new Rect(topLeft, new Size());
        }

        protected override void CreateHandles()
        {
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.TopLeft));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.TopRight));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.BottomLeft));
            Handles.Add(new RectDragHandle(new Size(10, 10), default, 10, (int)DragLocation.BottomRight));

            _dragHandleDict = Handles.ToDictionary(x => (DragLocation)x.Id);
        }


        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
        }

        public override void FindSelectedHandle(Point p)
        {
            if (DistanceResizeHandle.FillContains(p))
            {
                SelectedDragHandle = DistanceResizeHandle;
                return;
            }

            base.FindSelectedHandle(p);
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
                if (buttonState == MouseButtonState.Pressed) BottomRight = point;
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
                                StrokeThickness += -(point - OldPointForTranslate.Value).Y;
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


        private void ResizeByCornerPoint(DragLocation location, Point point)
        {
            switch (location)
            {
                case DragLocation.TopLeft:
                    _middleRectangleGeometry.Rect = new Rect(point, BottomRight);

                    break;
                case DragLocation.TopMiddle:
                    break;
                case DragLocation.TopRight:
                    break;
                case DragLocation.RightMiddle:
                    break;
                case DragLocation.BottomRight:
                    _middleRectangleGeometry.Rect = new Rect(TopLeft, point);

                    break;
                case DragLocation.BottomMiddle:
                    break;
                case DragLocation.BottomLeft:
                    break;
                case DragLocation.LeftMiddle:
                    break;
                case DragLocation.HorizontalTopLeft:

                    break;
                case DragLocation.HorizontalBottomRight:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }

            if (Handles.Count == 0) CreateHandles();

            //update location of handle
            DistanceResizeHandle.GeometryCenter = _middleRectangleGeometry.Rect.Location +
                                                   new Vector(_middleRectangleGeometry.Rect.Width / 2,
                                                       -StrokeThickness / 2);

            _dragHandleDict[DragLocation.TopLeft].GeometryCenter = TopLeft;
            _dragHandleDict[DragLocation.TopRight].GeometryCenter = new Point(BottomRight.X, TopLeft.Y);
            _dragHandleDict[DragLocation.BottomRight].GeometryCenter = BottomRight;
            _dragHandleDict[DragLocation.BottomLeft].GeometryCenter = new Point(TopLeft.X, BottomRight.Y);
        }


        #region local field

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            Pen ??= ShapeStyler?.SketchPen.CloneCurrentValue();

            if (ShapeStyler != null && Pen != null)
            {
                Pen.Brush.Opacity = 0.5;
                renderContext.DrawGeometry(ShapeStyler.FillColor, Pen, RenderGeometry);
            }

            renderContext.DrawGeometry(Brushes.Aquamarine, DragHandlePen, DistanceResizeHandle.HandleGeometry);

            foreach (var dragHandle in Handles)
                renderContext.DrawGeometry(Brushes.Aquamarine, DragHandlePen, dragHandle.HandleGeometry);

            renderContext.Close();
        }

        #endregion

        #endregion

        public PointsData GetMetaData()
        {
            throw new NotImplementedException();
        }
    }
}