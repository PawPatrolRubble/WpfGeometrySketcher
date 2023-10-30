using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes.Enums;
using Lan.Shapes.ExtensionMethods;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    //cross based on rectangle
    //public class Cross : ShapeVisualBase, IDataExport<PointsData>
    //{
    //    #region constructor

    //    public Cross(ShapeLayer layer) : base(layer)
    //    {
    //        //RenderGeometryGroup.Children.Add(_rectangleGeometry);
    //        RenderGeometryGroup.Children.Add(_verticalLine);
    //        RenderGeometryGroup.Children.Add(_horizontalLine);
    //    }

    //    #endregion

    //    #region private fields

    //    private TagPosition _tagPosition;

    //    #endregion

    //    #region fields

    //    private readonly LineGeometry _verticalLine = new LineGeometry();
    //    private readonly LineGeometry _horizontalLine = new LineGeometry();
    //    private readonly RectangleGeometry _rectangleGeometry = new RectangleGeometry();

    //    private Point _bottomRight;
    //    private Point _topLeft;

    //    #endregion

    //    #region Propeties

    //    public Point BottomRight
    //    {
    //        get { return _bottomRight; }
    //        set
    //        {
    //            SetField(ref _bottomRight, value);

    //            if (_rectangleGeometry != null)
    //            {
    //                _rectangleGeometry.Rect = new Rect(TopLeft, value);

    //                UpdateVerticalAndHorizontalLine();

    //                //UpdateHandleLocation();
    //                UpdateVisual();
    //            }
    //        }
    //    }

    //    private void UpdateVerticalAndHorizontalLine()
    //    {

    //        _horizontalLine.StartPoint = TopLeft + new Vector(0, _rectangleGeometry.Rect.Height / 2);
    //        _horizontalLine.EndPoint = BottomRight - new Vector(0, _rectangleGeometry.Rect.Height / 2);

    //        _verticalLine.StartPoint = TopLeft + new Vector(_rectangleGeometry.Rect.Width / 2, 0);
    //        _verticalLine.EndPoint = BottomRight + new Vector(-_rectangleGeometry.Rect.Width / 2, 0);
    //    }


    //    /// <summary>
    //    /// </summary>
    //    public override Rect BoundsRect
    //    {
    //        get { return RenderGeometry.Bounds; }
    //    }

    //    public Point TopLeft
    //    {
    //        get { return _topLeft; }
    //        set
    //        {
    //            SetField(ref _topLeft, value);
    //            _rectangleGeometry.Rect = new Rect(value, BottomRight);
    //            //UpdateHandleLocation();
    //            UpdateVerticalAndHorizontalLine();
    //            UpdateVisual();
    //        }
    //    }

    //    #endregion


    //    #region Implementations

    //    public void FromData(PointsData data)
    //    {
    //        if (data.DataPoints.Count != 2)
    //        {
    //            throw new Exception($"{nameof(PointsData)} must have 2 elements in  DataPoints");
    //        }

    //        //create handles

    //        //CreateHandles();
    //        TopLeft = data.DataPoints[0];
    //        BottomRight = data.DataPoints[1];
    //        IsGeometryRendered = true;

    //        Tag = data.Tag;
    //        _tagPosition = data.TagPosition;
    //    }



    //    /// <summary>
    //    /// </summary>
    //    /// <returns></returns>
    //    public PointsData GetMetaData()
    //    {
    //        return new PointsData(0,
    //            new List<Point> { _rectangleGeometry.Rect.TopLeft, _rectangleGeometry.Rect.BottomRight });
    //    }

    //    #endregion

    //    #region others

    //    protected override void CreateHandles()
    //    {
    //        Handles.AddRange(Enumerable.Range(1, 4).Select(x => new RectDragHandle(DragHandleSize, default, x)));
    //    }

    //    protected override void HandleResizing(Point point)
    //    {
    //        if (SelectedDragHandle != null)
    //        {
    //            switch (SelectedDragHandle.Id)
    //            {
    //                case 1:
    //                    TopLeft = ForcePointInRange(point, 0, BottomRight.X, 0, BottomRight.Y);
    //                    break;
    //                case 2:

    //                    var validPointTopRight = ForcePointInRange(point, TopLeft.X, point.X, 0, BottomRight.Y);
    //                    TopLeft = new Point(TopLeft.X, validPointTopRight.Y);
    //                    BottomRight = new Point(validPointTopRight.X, BottomRight.Y);
    //                    break;
    //                case 3:
    //                    BottomRight = ForcePointInRange(point, TopLeft.X, point.X, TopLeft.Y, point.Y);
    //                    break;
    //                case 4:
    //                    var validPointBottomLeft = ForcePointInRange(point, 0, BottomRight.X, TopLeft.Y, point.Y);

    //                    TopLeft = new Point(validPointBottomLeft.X, TopLeft.Y);
    //                    BottomRight = new Point(BottomRight.X, validPointBottomLeft.Y);
    //                    break;
    //            }
    //        }
    //    }

    //    protected override void HandleTranslate(Point newPoint)
    //    {
    //        if (OldPointForTranslate.HasValue)
    //        {
    //            TopLeft += newPoint - OldPointForTranslate.Value;
    //            BottomRight += newPoint - OldPointForTranslate.Value;
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
    //            TopLeft = mousePoint;
    //            //CreateHandles();
    //        }
    //        else
    //        {
    //            //FindSelectedHandle(mousePoint);
    //        }

    //        OldPointForTranslate = mousePoint;
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
    //                BottomRight = ForcePointInRange(point, TopLeft.X, point.X, TopLeft.Y, point.Y);
    //            }
    //            else if (SelectedDragHandle != null)
    //            {
    //                IsBeingDraggedOrPanMoving = true;
    //                //HandleResizing(point);
    //            }
    //            else
    //            {
    //                IsBeingDraggedOrPanMoving = true;

    //                HandleTranslate(point);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    ///     选择时
    //    /// </summary>
    //    public override void OnSelected()
    //    {
    //        //throw new NotImplementedException();
    //    }

    //    private void UpdateHandleLocation()
    //    {
    //        for (var i = 0; i < Handles.Count + 1; i++)
    //        {
    //            switch (i)
    //            {
    //                case 1:
    //                    Handles[i - 1].GeometryCenter = TopLeft;
    //                    break;
    //                case 2:
    //                    Handles[i - 1].GeometryCenter = new Point(BottomRight.X, TopLeft.Y);
    //                    break;
    //                case 3:
    //                    Handles[i - 1].GeometryCenter = BottomRight;
    //                    break;
    //                case 4:
    //                    Handles[i - 1].GeometryCenter = new Point(TopLeft.X, BottomRight.Y);
    //                    break;
    //            }
    //        }
    //    }

    //    public override void UpdateVisual()
    //    {
    //        if (_rectangleGeometry == null)
    //        {
    //            return;
    //        }

    //        var renderContext = RenderOpen();

    //        if (ShapeStyler != null)
    //        {
    //            AddTagText(renderContext, GetTagPosition());

    //            //renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _rectangleGeometry);
    //            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometryGroup);
    //            //foreach (var dragHandle in Handles)
    //            //{
    //            //    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, dragHandle.HandleGeometry);
    //            //}
    //        }

    //        renderContext.Close();
    //    }

    //    private Point GetTagPosition()
    //    {
    //        switch (_tagPosition)
    //        {
    //            case TagPosition.Center:
    //                return TopLeft.MiddleWith(BottomRight) -
    //                       new Vector(ShapeLayer.TagFontSize / 2, ShapeLayer.TagFontSize / 2);

    //            case TagPosition.Top:
    //                return TopLeft - new Vector(0, ShapeLayer.TagFontSize);
    //            case TagPosition.Bottom:
    //                return TopLeft + new Vector(0, BottomRight.Y - TopLeft.Y + ShapeLayer.TagFontSize);
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }
    //    }

    //    #endregion
    //}


    public class Cross : ShapeVisualBase, IDataExport<CrossData>
    {


        private readonly LineGeometry _verticalLine = new LineGeometry();
        private readonly LineGeometry _horizontalLine = new LineGeometry();
        private Point _center;
        private int _height;
        private int _width;

        public Cross(ShapeLayer layer) : base(layer)
        {

        }

        public Point Center
        {
            get { return _center; }
            set
            {
                SetField(ref _center, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }


        public int Height
        {
            get { return _height; }
            set
            {
                SetField(ref _height, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                SetField(ref _width, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }


        private void UpdateVerticalAndHorizontalLine()
        {

            _horizontalLine.StartPoint = Center + new Vector(-Width * 1.0 / 2, 0);
            _horizontalLine.EndPoint = Center + new Vector(Width * 1.0 / 2, 0);

            _verticalLine.StartPoint = Center + new Vector(0, -Height * 1.0 / 2);
            _verticalLine.EndPoint = Center + new Vector(0, Height * 1.0 / 2);
        }


        public override Rect BoundsRect { get; }
        protected override void CreateHandles()
        {
            //throw new NotImplementedException();
        }

        protected override void HandleResizing(Point point)
        {
            //throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            //throw new NotImplementedException();
        }

        public override void OnDeselected()
        {
            //throw new NotImplementedException();
        }

        public override void OnSelected()
        {
            //throw new NotImplementedException();
        }


        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            //base.OnMouseLeftButtonDown(mousePoint);
            //if (!IsGeometryRendered)
            //{
                
            //}
        }
        
        public void FromData(CrossData data)
        {
            Center = data.Center;
            Width = data.Width;
            Height = data.Height;
            ShapeStyler.SetStrokeThickness(data.StrokeThickness);
        }

        public CrossData GetMetaData()
        {
            return new CrossData()
            {
                Center = Center,
                Width = Width,
                Height = Height,
                StrokeThickness = ShapeStyler.SketchPen.Thickness,
            };
        }

        public override void UpdateVisual()
        {
            //base.UpdateVisual();

            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _verticalLine);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _horizontalLine);

            }

            renderContext.Close();

        }
    }
}