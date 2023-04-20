using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Custom
{
    public class StrokeWidenedCross : ShapeVisualBase
    {

        #region fields

        private Dictionary<int, Point> _points = new Dictionary<int, Point>();
        private readonly PathGeometry _pathGeometry;
        private readonly PathFigureCollection _pathFigureCollection;
        private readonly PathFigure _pathFigure = new PathFigure();
        private const double Tolerance = 30;

        #endregion

        #region ctor

        public StrokeWidenedCross()
        {
            _pathGeometry = new PathGeometry();
            _pathFigureCollection = _pathGeometry.Figures;
        }

        #endregion

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {

        }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                if (_points.Count > 0)
                {
                    //horizontal extension
                    if (_points.Count % 2 == 0)
                    {
                        _pathFigure.Segments.Add(new LineSegment(new Point(mousePoint.X, _points.Last().Value.Y), true));
                    }
                    else
                    {
                        _pathFigure.Segments.Add(new LineSegment(new Point(_points.Last().Value.X, mousePoint.Y), true));
                    }
                }
                else
                {
                    _pathFigure.StartPoint = mousePoint;
                    _pathFigureCollection.Add(_pathFigure);
                    RenderGeometryGroup.Children.Add(_pathGeometry);
                }
                _points.Add(_points.Count, mousePoint);
            }
            UpdateVisual();
        }


        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
        }

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
