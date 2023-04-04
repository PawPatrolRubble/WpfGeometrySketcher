﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;

namespace Lan.Shapes.Shapes
{
    public class Line
    {
        public double Length => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));

        private Point _start;

        public Point Start
        {
            get => _start;
            set { _start = value; }
        }

        private Point _end;

        public Point End
        {
            get => _end;
            set { _end = value; }
        }

        public Point MiddlePoint => new Point((End.X + Start.X) / 2, (End.Y + Start.Y) / 2);

        public Geometry Geometry => new LineGeometry(Start, End);

        public bool IsHit(Point p)
        {
            return false;
        }

        public void Transform(Matrix matrix)
        {
            Start = matrix.Transform(Start);
            End = matrix.Transform(End);
        }
    }

    public enum EdgeType
    {
        Upper = 1,
        Left,
        Right,
        Bottom
    }

    public class RectangleEdge : Line
    {
        public RectangleEdge(EdgeType edgeType)
        {
            EdgeType = edgeType;
        }

        public EdgeType EdgeType { get; set; }
    }


    public class Rectangle : ShapeVisualBase
    {
        #region edges

        private List<RectangleEdge> _edges;
        private Dictionary<EdgeType, RectangleEdge> _edgeDict;

        private RectangleEdge? _selectedEdge;




        #endregion


        #region constructor

        public Rectangle()
        {
            _edges = new List<RectangleEdge>()
            {
                new RectangleEdge(EdgeType.Upper),
                new RectangleEdge(EdgeType.Bottom),
                new RectangleEdge(EdgeType.Left),
                new RectangleEdge(EdgeType.Right),
            };

            _edgeDict = _edges.ToDictionary(x => x.EdgeType);
        }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => RenderGeometry.Bounds;
        }


        #region mouse events handler



        /// <summary>
        /// move with pan
        /// </summary>
        /// <param name="newPoint"></param>
        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                var matrix = new Matrix();
                matrix.Translate(newPoint.X - OldPointForTranslate.Value.X, newPoint.Y - OldPointForTranslate.Value.Y);
                _edges.ForEach(x => x.Transform(matrix));
            }

            OldPointForTranslate = newPoint;
        }


        /// <summary>
        /// drag one edge to move extend an shape, it can be extended in 4 directions
        /// </summary>
        /// <param name="point"></param>
        private void HandleDragToScale(Point point)
        {
            if (_selectedEdge != null && OldPointForTranslate.HasValue)
            {
                switch (_selectedEdge.EdgeType)
                {
                    case EdgeType.Upper:

                        var matrix = new Matrix();
                        matrix.Translate(0, point.Y - OldPointForTranslate.Value.Y);
                        //update upper edge
                        _edgeDict[EdgeType.Upper].Transform(matrix);

                        //update left edge
                        _edgeDict[EdgeType.Left].Transform(matrix);

                        //update right edge
                        _edgeDict[EdgeType.Right].Transform(matrix);

                        //render

                        break;
                    case EdgeType.Left:
                        break;
                    case EdgeType.Right:
                        break;
                    case EdgeType.Bottom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                UpdateVisual();
            }
        }


        protected override void HandleResizing(Point point)
        {
            switch ((DragLocation)SelectedDragHandle!.Id)
            {
                case DragLocation.TopLeft:
                    //upper
                    _edgeDict[EdgeType.Upper].Start = point;
                    _edgeDict[EdgeType.Upper].End = new Point(_edgeDict[EdgeType.Upper].End.X, point.Y);

                    //right
                    _edgeDict[EdgeType.Right].Start = _edgeDict[EdgeType.Upper].End;

                    //left
                    _edgeDict[EdgeType.Left].Start = point;
                    _edgeDict[EdgeType.Left].End = new Point(point.X, _edgeDict[EdgeType.Left].End.Y);

                    //bottom
                    _edgeDict[EdgeType.Bottom].Start = _edgeDict[EdgeType.Left].End;
                    break;
                case DragLocation.TopMiddle:
                    break;
                case DragLocation.TopRight:
                    //right
                    _edgeDict[EdgeType.Right].Start = point;
                    _edgeDict[EdgeType.Right].End = new Point(point.X, _edgeDict[EdgeType.Right].End.Y);

                    //upper
                    _edgeDict[EdgeType.Upper].End = point;
                    _edgeDict[EdgeType.Upper].Start = new Point(_edgeDict[EdgeType.Upper].Start.X, point.Y);

                    //bottom
                    _edgeDict[EdgeType.Bottom].End = _edgeDict[EdgeType.Right].End;

                    //left
                    _edgeDict[EdgeType.Left].Start = _edgeDict[EdgeType.Upper].Start;
                    break;
                case DragLocation.RightMiddle:
                    break;
                case DragLocation.BottomRight:

                    _edgeDict[EdgeType.Upper].End = new Point(point.X, _edgeDict[EdgeType.Upper].End.Y);

                    _edgeDict[EdgeType.Right].Start = _edgeDict[EdgeType.Upper].End;
                    _edgeDict[EdgeType.Right].End = point;

                    _edgeDict[EdgeType.Bottom].Start = new Point(_edgeDict[EdgeType.Bottom].Start.X, point.Y);
                    _edgeDict[EdgeType.Bottom].End = point;


                    _edgeDict[EdgeType.Left].End = _edgeDict[EdgeType.Bottom].Start;

                    break;
                case DragLocation.BottomMiddle:
                    break;
                case DragLocation.BottomLeft:
                    //upper start
                    _edgeDict[EdgeType.Upper].Start = new Point(point.X, _edgeDict[EdgeType.Upper].Start.Y);

                    //left
                    _edgeDict[EdgeType.Left].Start = _edgeDict[EdgeType.Upper].Start;
                    _edgeDict[EdgeType.Left].End = point;

                    //bottom
                    _edgeDict[EdgeType.Bottom].Start = point;
                    _edgeDict[EdgeType.Bottom].End = new Point(_edgeDict[EdgeType.Bottom].End.X, point.Y);

                    //right
                    _edgeDict[EdgeType.Right].End = _edgeDict[EdgeType.Bottom].End;

                    break;
                case DragLocation.LeftMiddle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //CreateHandles();
            //UpdateGeometryGroup();
            //UpdateVisual();
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point point)
        {

            _edgeDict[EdgeType.Upper].Start = new Point(oldPoint.X, oldPoint.Y);
            _edgeDict[EdgeType.Upper].End = new Point(point.X, oldPoint.Y);

            _edgeDict[EdgeType.Left].Start = new Point(oldPoint.X, oldPoint.Y);
            _edgeDict[EdgeType.Left].End = new Point(oldPoint.X, point.Y);

            _edgeDict[EdgeType.Right].Start = new Point(point.X, oldPoint.Y);
            _edgeDict[EdgeType.Right].End = new Point(point.X, point.Y);

            _edgeDict[EdgeType.Bottom].Start = new Point(oldPoint.X, point.Y);
            _edgeDict[EdgeType.Bottom].End = new Point(point.X, point.Y);
        }

        protected override void UpdateGeometryGroup()
        {
            RenderGeometryGroup.Children.Clear();

            var rect = GenerateRect();
            RenderGeometryGroup.Children.Add(new RectangleGeometry(GenerateRect()));
            RenderGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));
        }

        protected override void CreateHandles()
        {
            Handles.Clear();

            Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Left].Start, (int)DragLocation.TopLeft));
            Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Left].End, (int)DragLocation.BottomLeft));
            Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Right].Start, (int)DragLocation.TopRight));
            Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Right].End, (int)DragLocation.BottomRight));

            HandleGeometryGroup ??= new GeometryGroup();
            HandleGeometryGroup.Children.Clear();
            HandleGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));

            PanSensitiveArea = new CombinedGeometry(GeometryCombineMode.Exclude, RenderGeometryGroup, HandleGeometryGroup);
        }

        private Rect GenerateRect()
        {
            return new Rect(_edgeDict[EdgeType.Upper].Start,
                new Size(_edgeDict[EdgeType.Upper].Length, _edgeDict[EdgeType.Left].Length));
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


        #endregion
    }
}