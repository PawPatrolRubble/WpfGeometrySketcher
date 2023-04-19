using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;

namespace Lan.Shapes.Shapes
{
    public class Polygon : ShapeVisualBase
    {
        #region fields

        private Dictionary<int, Point> _points = new Dictionary<int, Point>();
        private readonly PathGeometry _pathGeometry;
        private readonly PathFigureCollection _pathFigureCollection;
        private readonly PathFigure _pathFigure = new PathFigure();
        private const double Tolerance = 30;

        #endregion

        public Polygon()
        {
            _pathGeometry = new PathGeometry();
            _pathFigureCollection = _pathGeometry.Figures;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get => _pathGeometry.Bounds; }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            OldPointForTranslate = mousePoint;
            if (!IsGeometryRendered)
            {
                //_points.Add(_points.Count, mousePoint);
                CreateNewGeometryAndRenderIt(mousePoint);
            }
            else
            {

                SelectedDragHandle = FindDragHandleMouseOver(mousePoint);
                Debug.WriteLine($"handle = {SelectedDragHandle ==null}");

            }

        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (_pathFigure.IsClosed)
            {
                IsGeometryRendered = true;
            }

            SelectedDragHandle = null;
            IsBeingDragged = false;
        }


        private void CreateNewGeometryAndRenderIt(Point newPoint)
        {
            if (_points.Count > 0)
            {
                //add new line geometry
                //RenderGeometryGroup.Children.Add(new LineGeometry(_points[^1], newPoint));

                if (IsTwoPointsAreClose(_points[0], newPoint, Tolerance))
                {
                    //newPoint = _points[0];
                    //_pathFigure.Segments.Add(new LineSegment(newPoint, true));
                    ClosePolygon();
                }
                else
                {
                    _pathFigure.Segments.Add(new LineSegment(newPoint, true));
                }

                //if last point is close to first point it will close the polygon
            }
            else
            {
                _pathFigure.StartPoint = newPoint;
                _pathFigureCollection.Add(_pathFigure);
                RenderGeometryGroup.Children.Add(_pathGeometry);
            }


            //if the geometry is closed, handle and last point will not be added
            if (!_pathFigure.IsClosed)
            {
                Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, newPoint, _points.Count));
                _points.Add(_points.Count, newPoint);
            }

            RenderGeometryGroup.Children.Add(Handles[Handles.Count - 1].HandleGeometry);
            UpdateVisual();
        }


        public override void OnMouseRightButtonUp(Point mousePosition)
        {
            ClosePolygon();
        }


        private void ClosePolygon()
        {
            _pathFigure.IsClosed = true;
            IsGeometryRendered = true;
            HandleGeometryGroup ??= new GeometryGroup();
            HandleGeometryGroup?.Children.AddRange(Handles.Select(x => x.HandleGeometry));
        }


        private bool IsTwoPointsAreClose(Point p1, Point p2, double tolerance)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)) < tolerance;
        }


        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (IsGeometryRendered)
            {
                //handle pan of geometry
                State = ShapeVisualState.MouseOver;
                if (buttonState == MouseButtonState.Pressed)
                {
                    if (SelectedDragHandle != null)
                    {
                        IsBeingDragged = true;
                        //handle resizing 
                        HandleResizing(point);
                    }
                    else
                    {
                        //handle translation
                        HandleTranslate(point);
                    }

                    UpdateVisual();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateGeometryGroup()
        {

        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {

        }

        protected override void HandleResizing(Point point)
        {
            if (SelectedDragHandle != null)
            {
                _points[SelectedDragHandle.Id] = point;

                if (SelectedDragHandle.Id > 0)
                {
                    ((LineSegment)_pathFigure.Segments[SelectedDragHandle.Id-1]).Point = point;
                }
                else
                {
                    _pathFigure.StartPoint = point;
                }

                SelectedDragHandle.Location = point;

                UpdateVisual();
            }
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
                var deltaX = newPoint.X - OldPointForTranslate.Value.X;
                var deltaY = newPoint.Y - OldPointForTranslate.Value.Y;

                var matrix = new Matrix();
                matrix.Translate(deltaX, deltaY);

                var tans = new TranslateTransform(deltaX, deltaY);

                foreach (var segment in _pathFigure.Segments)
                {
                    if (segment is LineSegment lineSegment)
                    {
                        lineSegment.Point = matrix.Transform(lineSegment.Point);
                    }
                }

                _pathFigure.StartPoint = matrix.Transform(_pathFigure.StartPoint);
                Handles.ForEach(x=>x.Location = tans.Transform(x.Location));
            }

            OldPointForTranslate = newPoint;
        }

    }
}
