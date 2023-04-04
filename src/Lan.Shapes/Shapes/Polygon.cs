using System;
using System.Collections.Generic;
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
        private readonly List<Point> _points = new List<Point>();
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
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonDown(Point newPoint)
        {

        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (!IsGeometryInitialized)
            {

                if (_points.Count > 0)
                {
                    //add new line geometry
                    //RenderGeometryGroup.Children.Add(new LineGeometry(_points[^1], newPoint));

                    if (IsTwoPointsAreClose(_points[0], newPoint, Tolerance))
                    {
                        newPoint = _points[0];
                        _pathFigure.Segments.Add(new LineSegment(newPoint, true));

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


                Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, newPoint, _points.Count));
                RenderGeometryGroup.Children.Add(Handles[Handles.Count - 1].HandleGeometry);
                _points.Add(newPoint);
                UpdateVisual();
            }
        }



        private void ClosePolygon()
        {

            _pathFigure.IsClosed = true;
            IsGeometryInitialized = true;

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
            throw new NotImplementedException();
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            throw new NotImplementedException();
        }

        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}
