#nullable enable

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

namespace Lan.Shapes.Shapes
{
    //public class Line
    //{
    //    #region fields

    //    private Point _end;

    //    private Point _start;

    //    #endregion

    //    #region Propeties

    //    public Point End
    //    {
    //        get => _end;
    //        set => _end = value;
    //    }

    //    public Geometry Geometry
    //    {
    //        get => new LineGeometry(Start, End);
    //    }

    //    public double Length
    //    {
    //        get => Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
    //    }

    //    public Point MiddlePoint
    //    {
    //        get => new Point((End.X + Start.X) / 2, (End.Y + Start.Y) / 2);
    //    }

    //    public Point Start
    //    {
    //        get => _start;
    //        set => _start = value;
    //    }

    //    #endregion

    //    #region others

    //    public bool IsHit(Point p)
    //    {
    //        return false;
    //    }

    //    public void Transform(Matrix matrix)
    //    {
    //        Start = matrix.Transform(Start);
    //        End = matrix.Transform(End);
    //    }

    //    #endregion
    //}

    //public enum EdgeType
    //{
    //    Upper = 1,
    //    Left,
    //    Right,
    //    Bottom
    //}

    //public class RectangleEdge : Line
    //{
    //    #region Propeties

    //    public EdgeType EdgeType { get; set; }

    //    #endregion

    //    #region Constructors

    //    public RectangleEdge(EdgeType edgeType)
    //    {
    //        EdgeType = edgeType;
    //    }

    //    #endregion
    //}


    //public class Rectangle : ShapeVisualBase
    //{
    //    #region Propeties

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public override Rect BoundsRect
    //    {
    //        get => RenderGeometry.Bounds;
    //    }

    //    private Point _topLeft;

    //    public Point TopLeft
    //    {
    //        get => _topLeft;
    //        set
    //        {
    //            SetField(ref _topLeft, value);
    //        }
    //    }

    //    private Point _bottomRight;

    //    public Point BottomRight
    //    {
    //        get => _bottomRight;
    //        set { SetField(ref _bottomRight, value); }
    //    }

    //    #endregion

    //    #region Constructors

    //    #region constructor

    //    public Rectangle()
    //    {
    //        _edges = new List<RectangleEdge>
    //        {
    //            new RectangleEdge(EdgeType.Upper),
    //            new RectangleEdge(EdgeType.Bottom),
    //            new RectangleEdge(EdgeType.Left),
    //            new RectangleEdge(EdgeType.Right)
    //        };

    //        _edgeDict = _edges.ToDictionary(x => x.EdgeType);
    //    }

    //    #endregion

    //    #endregion

    //    #region edges

    //    private readonly List<RectangleEdge> _edges;
    //    private readonly Dictionary<EdgeType, RectangleEdge> _edgeDict;

    //    private RectangleEdge? _selectedEdge;

    //    #endregion


    //    #region mouse events handler

    //    /// <summary>
    //    /// move with pan
    //    /// </summary>
    //    /// <param name="newPoint"></param>
    //    protected override void HandleTranslate(Point newPoint)
    //    {
    //        IsBeingDraggedOrPanMoving = true;
    //        if (OldPointForTranslate.HasValue)
    //        {
    //            var matrix = new Matrix();
    //            matrix.Translate(newPoint.X - OldPointForTranslate.Value.X, newPoint.Y - OldPointForTranslate.Value.Y);
    //            _edges.ForEach(x => x.Transform(matrix));
    //        }

    //        OldPointForTranslate = newPoint;
    //    }

    //    /// <summary>
    //    /// 未选择状态
    //    /// </summary>
    //    public override void OnDeselected()
    //    {
    //        throw new NotImplementedException();
    //    }


    //    /// <summary>
    //    /// drag one edge to move extend an shape, it can be extended in 4 directions
    //    /// </summary>
    //    /// <param name="point"></param>
    //    private void HandleDragToScale(Point point)
    //    {
    //        if (_selectedEdge != null && OldPointForTranslate.HasValue)
    //        {
    //            switch (_selectedEdge.EdgeType)
    //            {
    //                case EdgeType.Upper:

    //                    var matrix = new Matrix();
    //                    matrix.Translate(0, point.Y - OldPointForTranslate.Value.Y);
    //                    //update upper edge
    //                    _edgeDict[EdgeType.Upper].Transform(matrix);

    //                    //update left edge
    //                    _edgeDict[EdgeType.Left].Transform(matrix);

    //                    //update right edge
    //                    _edgeDict[EdgeType.Right].Transform(matrix);

    //                    //render

    //                    break;
    //                case EdgeType.Left:
    //                    break;
    //                case EdgeType.Right:
    //                    break;
    //                case EdgeType.Bottom:
    //                    break;
    //                default:
    //                    throw new ArgumentOutOfRangeException();
    //            }

    //            UpdateVisual();
    //        }
    //    }


    //    protected override void HandleResizing(Point point)
    //    {
    //        switch ((DragLocation)SelectedDragHandle!.Id)
    //        {
    //            case DragLocation.TopLeft:
    //                //upper
    //                _edgeDict[EdgeType.Upper].Start = point;
    //                _edgeDict[EdgeType.Upper].End = new Point(_edgeDict[EdgeType.Upper].End.X, point.Y);

    //                //right
    //                _edgeDict[EdgeType.Right].Start = _edgeDict[EdgeType.Upper].End;

    //                //left
    //                _edgeDict[EdgeType.Left].Start = point;
    //                _edgeDict[EdgeType.Left].End = new Point(point.X, _edgeDict[EdgeType.Left].End.Y);

    //                //bottom
    //                _edgeDict[EdgeType.Bottom].Start = _edgeDict[EdgeType.Left].End;
    //                break;
    //            case DragLocation.TopMiddle:
    //                break;
    //            case DragLocation.TopRight:
    //                //right
    //                _edgeDict[EdgeType.Right].Start = point;
    //                _edgeDict[EdgeType.Right].End = new Point(point.X, _edgeDict[EdgeType.Right].End.Y);

    //                //upper
    //                _edgeDict[EdgeType.Upper].End = point;
    //                _edgeDict[EdgeType.Upper].Start = new Point(_edgeDict[EdgeType.Upper].Start.X, point.Y);

    //                //bottom
    //                _edgeDict[EdgeType.Bottom].End = _edgeDict[EdgeType.Right].End;

    //                //left
    //                _edgeDict[EdgeType.Left].Start = _edgeDict[EdgeType.Upper].Start;
    //                break;
    //            case DragLocation.RightMiddle:
    //                break;
    //            case DragLocation.BottomRight:

    //                _edgeDict[EdgeType.Upper].End = new Point(point.X, _edgeDict[EdgeType.Upper].End.Y);

    //                _edgeDict[EdgeType.Right].Start = _edgeDict[EdgeType.Upper].End;
    //                _edgeDict[EdgeType.Right].End = point;

    //                _edgeDict[EdgeType.Bottom].Start = new Point(_edgeDict[EdgeType.Bottom].Start.X, point.Y);
    //                _edgeDict[EdgeType.Bottom].End = point;


    //                _edgeDict[EdgeType.Left].End = _edgeDict[EdgeType.Bottom].Start;

    //                break;
    //            case DragLocation.BottomMiddle:
    //                break;
    //            case DragLocation.BottomLeft:
    //                //upper start
    //                _edgeDict[EdgeType.Upper].Start = new Point(point.X, _edgeDict[EdgeType.Upper].Start.Y);

    //                //left
    //                _edgeDict[EdgeType.Left].Start = _edgeDict[EdgeType.Upper].Start;
    //                _edgeDict[EdgeType.Left].End = point;

    //                //bottom
    //                _edgeDict[EdgeType.Bottom].Start = point;
    //                _edgeDict[EdgeType.Bottom].End = new Point(_edgeDict[EdgeType.Bottom].End.X, point.Y);

    //                //right
    //                _edgeDict[EdgeType.Right].End = _edgeDict[EdgeType.Bottom].End;

    //                break;
    //            case DragLocation.LeftMiddle:
    //                break;
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }


    //        //CreateHandles();
    //        //UpdateGeometryGroup();
    //        //UpdateVisual();
    //    }

    //    protected override void DrawGeometryInMouseMove(Point oldPoint, Point point)
    //    {
    //        _edgeDict[EdgeType.Upper].Start = new Point(oldPoint.X, oldPoint.Y);
    //        _edgeDict[EdgeType.Upper].End = new Point(point.X, oldPoint.Y);

    //        _edgeDict[EdgeType.Left].Start = new Point(oldPoint.X, oldPoint.Y);
    //        _edgeDict[EdgeType.Left].End = new Point(oldPoint.X, point.Y);

    //        _edgeDict[EdgeType.Right].Start = new Point(point.X, oldPoint.Y);
    //        _edgeDict[EdgeType.Right].End = new Point(point.X, point.Y);

    //        _edgeDict[EdgeType.Bottom].Start = new Point(oldPoint.X, point.Y);
    //        _edgeDict[EdgeType.Bottom].End = new Point(point.X, point.Y);
    //    }

    //    /// <summary>
    //    /// 选择时
    //    /// </summary>
    //    public override void OnSelected()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void UpdateGeometryGroup()
    //    {
    //        RenderGeometryGroup.Children.Clear();

    //        RenderGeometryGroup.Children.Add(new RectangleGeometry(GenerateRect()));
    //        RenderGeometryGroup.Children.Add(HandleGeometryGroup);
    //    }

    //    protected override void CreateHandles()
    //    {
    //        Handles.Clear();

    //        Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Left].Start,
    //            (int)DragLocation.TopLeft));
    //        Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Left].End,
    //            (int)DragLocation.BottomLeft));
    //        Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Right].Start,
    //            (int)DragLocation.TopRight));
    //        Handles.Add(new CircleDragHandle(ShapeStyler.DragHandleSize, _edgeDict[EdgeType.Right].End,
    //            (int)DragLocation.BottomRight));

    //        HandleGeometryGroup ??= new GeometryGroup();
    //        HandleGeometryGroup.Children.Clear();
    //        HandleGeometryGroup.Children.AddRange(Handles.Select(x => x.HandleGeometry));

    //        PanSensitiveArea =
    //            new CombinedGeometry(GeometryCombineMode.Exclude, RenderGeometryGroup, HandleGeometryGroup);
    //    }

    //    private Rect GenerateRect()
    //    {
    //        return new Rect(_edgeDict[EdgeType.Upper].Start,
    //            new Size(_edgeDict[EdgeType.Upper].Length, _edgeDict[EdgeType.Left].Length));
    //    }


    //    public override void UpdateVisual()
    //    {
    //        base.UpdateVisual();
    //        TopLeft = _edgeDict[EdgeType.Left].Start;
    //        BottomRight = _edgeDict[EdgeType.Bottom].End;

    //    }

    //    #endregion
    //}


    public class Rectangle : ShapeVisualBase,IDataExport<PointsData>
    {
        #region fields

        private readonly RectangleGeometry _rectangleGeometry = new RectangleGeometry(default);
        private Point _bottomRight;
        private Point _topLeft;
        #endregion

        #region Propeties

        public Point BottomRight
        {
            get => _bottomRight;
            set
            {
                SetField(ref _bottomRight, value);
                _rectangleGeometry.Rect = new Rect(TopLeft, value);
                UpdateHandleLocation();
                UpdateVisual();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                SetField(ref _topLeft, value);
                _rectangleGeometry.Rect = new Rect(value, BottomRight);
                UpdateHandleLocation();
                UpdateVisual();
            }
        }

        #endregion

        #region Constructors

        public Rectangle()
        {
            RenderGeometryGroup.Children.Add(_rectangleGeometry);
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {
            Handles.AddRange(Enumerable.Range(1, 4).Select(x => new RectDragHandle(10, default, x)));
        }

        protected override void HandleResizing(Point point)
        {
            if (SelectedDragHandle != null)
                switch (SelectedDragHandle.Id)
                {
                    case 1:
                        TopLeft = point;
                        break;
                    case 2:
                        TopLeft = new Point(TopLeft.X, point.Y);
                        BottomRight = new Point(point.X, BottomRight.Y);
                        break;
                    case 3:
                        BottomRight = point;
                        break;
                    case 4:
                        TopLeft = new Point(point.X, TopLeft.Y);
                        BottomRight = new Point(BottomRight.X, point.Y);
                        break;
                }
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                TopLeft += newPoint - OldPointForTranslate.Value;
                BottomRight += newPoint - OldPointForTranslate.Value;
                OldPointForTranslate = newPoint;
            }
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
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
                CreateHandles();
                TopLeft = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
        }

        private void UpdateHandleLocation()
        {
            for (int i = 0; i < Handles.Count+1; i++)
            {
                switch (i)
                {
                    case 1:
                        Handles[i - 1].GeometryCenter = TopLeft;
                        break;
                    case 2:
                        Handles[i - 1].GeometryCenter = new Point(BottomRight.X, TopLeft.Y);
                        break;
                    case 3:
                        Handles[i - 1].GeometryCenter = BottomRight;
                        break;
                    case 4:
                        Handles[i - 1].GeometryCenter = new Point(TopLeft.X, BottomRight.Y);
                        break;
                }
            }
        }


        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    BottomRight = point;
                }
                else if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleResizing(point);
                }
                else
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleTranslate(point);
                }
            }
        }

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _rectangleGeometry);
                foreach (var dragHandle in Handles)
                {
                    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, dragHandle.HandleGeometry);
                }
            }
            renderContext.Close();
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        #endregion

        public PointsData GetMetaData()
        {
            return new PointsData(0, new List<Point>() { TopLeft, BottomRight });
        }
    }
}