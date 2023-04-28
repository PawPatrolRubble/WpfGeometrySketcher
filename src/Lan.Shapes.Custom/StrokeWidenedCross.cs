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
    /// <summary>
    /// don't use this
    ///// </summary>
    //public class StrokeWidenedCross : ShapeVisualBase
    //{
    //    #region my fields

    //    private readonly ThickenedCross _innerCross = new ThickenedCross();
    //    private readonly ThickenedCross _middleCross = new ThickenedCross();
    //    private readonly Pen _middlePen = new Pen(Brushes.Red, 1);
    //    private readonly ThickenedCross _outerCross = new ThickenedCross();
    //    private double _distance;
    //    private Dictionary<DragLocation, DragHandle> _dragHandles;
    //    private Vector _halfDistanceVector;
    //    private Point _horizontalBottomRight;

    //    private Point _horizontalTopLeft;

    //    private Point _verticalBottomRight;


    //    private Point _verticalTopLeft;

    //    #endregion

    //    #region Propeties

    //    /// <summary>
    //    /// </summary>
    //    public override Rect BoundsRect { get; }

    //    public double Distance
    //    {
    //        get => _distance;
    //        set
    //        {
    //            _distance = value;

    //            _halfDistanceVector.X = value / 2;
    //            _halfDistanceVector.Y = value / 2;

    //            if (_distance > 0)
    //            {
    //                UpdateTopLeftLocationForGeometry(_middleCross.VerticalTopLeft);
    //                UpdateBottomRightForGeometry(_middleCross.VerticalBottomRight);
    //            }
    //        }
    //    }

    //    public Point HorizontalBottomRight
    //    {
    //        get => _horizontalBottomRight;
    //        set
    //        {
    //            _horizontalBottomRight = value;
    //            UpdateGeometryOnDragLocationChanges(_horizontalBottomRight, DragLocation.HorizontalBottomRight);
    //        }
    //    }

    //    public Point HorizontalTopLeft
    //    {
    //        get => _horizontalTopLeft;
    //        set
    //        {
    //            _horizontalTopLeft = value;

    //            UpdateGeometryOnDragLocationChanges(_horizontalTopLeft, DragLocation.HorizontalTopLeft);
    //        }
    //    }

    //    public Point VerticalBottomRight
    //    {
    //        get => _verticalBottomRight;
    //        set
    //        {
    //            _verticalBottomRight = value;
    //            UpdateBottomRightForGeometry(_verticalBottomRight);
    //        }
    //    }

    //    private Point VerticalTopLeft
    //    {
    //        get => _verticalTopLeft;
    //        set
    //        {
    //            _verticalTopLeft = value;
    //            UpdateTopLeftLocationForGeometry(_verticalTopLeft);
    //        }
    //    }

    //    #endregion

    //    #region Constructors

    //    public StrokeWidenedCross()
    //    {
    //        _halfDistanceVector = new Vector();
    //        Distance = 20;
    //        _middlePen.DashStyle = DashStyles.DashDot;
    //        RenderGeometryGroup.Children.Add(new CombinedGeometry(GeometryCombineMode.Xor, _outerCross.RenderGeometry,
    //            _innerCross.RenderGeometry));
    //    }

    //    #endregion

    //    #region others

    //    protected override void CreateHandles()
    //    {
    //        if (Handles.Count == 0)
    //        {
    //            Handles.Add(new RectDragHandle(10, _middleCross.VerticalTopLeft, (int)DragLocation.TopLeft));
    //            Handles.Add(new RectDragHandle(10, _middleCross.VerticalBottomRight, (int)DragLocation.BottomRight));

    //            Handles.Add(new RectDragHandle(10, _middleCross.HorizontalTopLeft,
    //                (int)DragLocation.HorizontalTopLeft));
    //            Handles.Add(new RectDragHandle(10, _middleCross.HorizontalBottomRight,
    //                (int)DragLocation.HorizontalBottomRight));

    //            Handles.Add(new RectDragHandle(10, _outerCross.HorizontalBottomRight, (int)DragLocation.RightMiddle));

    //            _dragHandles = Handles.ToDictionary(x => (DragLocation)x.Id);
    //        }
    //        else
    //        {
    //            UpdateHandleLocations();
    //        }
    //    }

    //    protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void HandleResizing(Point point)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void HandleTranslate(Point newPoint)
    //    {
    //        if (OldPointForTranslate.HasValue)
    //        {
    //            VerticalTopLeft += newPoint - OldPointForTranslate.Value;
    //            VerticalBottomRight += newPoint - OldPointForTranslate.Value;

    //            Handles.ForEach(x => x.GeometryCenter += newPoint - OldPointForTranslate.Value);

    //            OldPointForTranslate = newPoint;
    //        }
    //    }

    //    /// <summary>
    //    ///     未选择状态
    //    /// </summary>
    //    public override void OnDeselected()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    /// <summary>
    //    ///     left mouse button down event
    //    /// </summary>
    //    /// <param name="mousePoint"></param>
    //    public override void OnMouseLeftButtonDown(Point mousePoint)
    //    {
    //        if (!IsGeometryRendered)
    //        {
    //            VerticalTopLeft = mousePoint;
    //            //_outerCross.OnMouseLeftButtonDown(mousePoint - _halfDistanceVector);
    //            //_innerCross.OnMouseLeftButtonDown(mousePoint + _halfDistanceVector);
    //            //_middleCross.OnMouseLeftButtonDown(mousePoint);
    //            //UpdateVisual();
    //        }
    //        else
    //        {
    //            OldPointForTranslate = mousePoint;
    //            FindSelectedHandle(mousePoint);
    //        }
    //    }

    //    /// <summary>
    //    ///     when mouse left button up
    //    /// </summary>
    //    /// <param name="newPoint"></param>
    //    public override void OnMouseLeftButtonUp(Point newPoint)
    //    {
    //        base.OnMouseLeftButtonUp(newPoint);
    //        _middleCross.OnMouseLeftButtonUp(newPoint);
    //        _innerCross.OnMouseLeftButtonUp(newPoint);
    //        _outerCross.OnMouseLeftButtonUp(newPoint);
    //    }


    //    /// <summary>
    //    ///     鼠标点击移动
    //    /// </summary>
    //    public override void OnMouseMove(Point point, MouseButtonState buttonState)
    //    {
    //        if (buttonState == MouseButtonState.Pressed)
    //        {
    //            if (!IsGeometryRendered)
    //            {
    //                VerticalBottomRight = point;
    //                CreateHandles();
    //                UpdateVisual();
    //            }
    //            else if (SelectedDragHandle != null) //handle resizing
    //            {
    //                IsBeingDraggedOrPanMoving = true;

    //                switch ((DragLocation)SelectedDragHandle.Id)
    //                {
    //                    case DragLocation.TopLeft:
    //                        VerticalTopLeft = point;
    //                        break;
    //                    case DragLocation.TopMiddle:
    //                        break;
    //                    case DragLocation.TopRight:
    //                        break;
    //                    case DragLocation.RightMiddle: //change size of stroke width
    //                        if (OldPointForTranslate != null)
    //                        {
    //                            Distance += (point - OldPointForTranslate.Value).X;
    //                            OldPointForTranslate = point;
    //                        }

    //                        break;
    //                    case DragLocation.BottomRight:
    //                        VerticalBottomRight = point;
    //                        break;
    //                    case DragLocation.BottomMiddle:
    //                        break;
    //                    case DragLocation.BottomLeft:
    //                        break;
    //                    case DragLocation.LeftMiddle:
    //                        break;
    //                    case DragLocation.HorizontalTopLeft:
    //                        HorizontalTopLeft = point;
    //                        break;
    //                    case DragLocation.HorizontalBottomRight:
    //                        HorizontalBottomRight = point;
    //                        break;
    //                    default:
    //                        throw new ArgumentOutOfRangeException();
    //                }

    //                UpdateHandleLocations();
    //                UpdateVisual();
    //            }
    //            else //handle translation
    //            {
    //                Mouse.SetCursor(Cursors.Hand);
    //                IsBeingDraggedOrPanMoving = true;
    //                HandleTranslate(point);
    //                UpdateVisual();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    ///     选择时
    //    /// </summary>
    //    public override void OnSelected()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    /// <summary>
    //    ///     add geometries to group
    //    /// </summary>
    //    protected override void UpdateGeometryGroup()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private void UpdateGeometryOnDragLocationChanges(Point point, DragLocation dragLocation)
    //    {
    //        switch (dragLocation)
    //        {
    //            case DragLocation.HorizontalTopLeft:
    //                _middleCross.HorizontalTopLeft = point;
    //                _outerCross.HorizontalTopLeft = point - _halfDistanceVector;
    //                _innerCross.HorizontalTopLeft = point + _halfDistanceVector;
    //                break;
    //            case DragLocation.HorizontalBottomRight:
    //                _middleCross.HorizontalBottomRight = point;
    //                _outerCross.HorizontalBottomRight = point + _halfDistanceVector;
    //                _innerCross.HorizontalBottomRight = point - _halfDistanceVector;
    //                break;
    //        }
    //    }


    //    public override void UpdateVisual()
    //    {
    //        var renderContext = RenderOpen();
    //        if (ShapeStyler != null)
    //            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometryGroup);

    //        //middle cross style
    //        renderContext.DrawGeometry(Brushes.Transparent, _middlePen, _middleCross.RenderGeometry);

    //        foreach (var dragHandle in Handles)
    //            renderContext.DrawGeometry(Brushes.Transparent, _middlePen, dragHandle.HandleGeometry);

    //        renderContext.Close();
    //    }

    //    #endregion


    //    #region local methods

    //    private void UpdateHandleLocations()
    //    {
    //        _dragHandles[DragLocation.TopLeft].GeometryCenter = _middleCross.VerticalTopLeft;
    //        _dragHandles[DragLocation.BottomRight].GeometryCenter = _middleCross.VerticalBottomRight;
    //        _dragHandles[DragLocation.RightMiddle].GeometryCenter = _outerCross.HorizontalBottomRight;
    //        _dragHandles[DragLocation.HorizontalBottomRight].GeometryCenter = _middleCross.HorizontalBottomRight;
    //        _dragHandles[DragLocation.HorizontalTopLeft].GeometryCenter = _middleCross.HorizontalTopLeft;
    //    }


    //    private void UpdateTopLeftLocationForGeometry(Point topLeft)
    //    {
    //        //_middleCross.OnMouseLeftButtonDown();
    //        _middleCross.VerticalTopLeft = topLeft;
    //        _outerCross.VerticalTopLeft = topLeft - _halfDistanceVector;
    //        _innerCross.VerticalTopLeft = topLeft + _halfDistanceVector;
    //    }

    //    private void UpdateBottomRightForGeometry(Point bottomRight)
    //    {
    //        //_middleCross.OnMouseMove(point, buttonState);
    //        //_outerCross.OnMouseMove(point + _halfDistanceVector, buttonState);
    //        _middleCross.VerticalBottomRight = bottomRight;
    //        _outerCross.VerticalBottomRight = bottomRight + _halfDistanceVector;

    //        if ((bottomRight - _innerCross.VerticalTopLeft).X > Distance &&
    //            (bottomRight - _innerCross.VerticalTopLeft).Y > Distance)
    //            //_innerCross.OnMouseMove(bottomRight - _halfDistanceVector, buttonState);
    //            _innerCross.VerticalBottomRight = bottomRight - _halfDistanceVector;
    //    }

    //    #endregion
    //}
}