#nullable enable

#region

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

#endregion

namespace Lan.Shapes.Shapes
{
    public class Rectangle : ShapeVisualBase, IDataExport<PointsData>
    {
        #region fields

        private RectangleGeometry? _rectangleGeometry;
        private Point _bottomRight;
        private Point _topLeft;

        #endregion

        private TagPosition _tagPosition;

        #region Properties

        public Point BottomRight
        {
            get => _bottomRight;
            set
            {
                SetField(ref _bottomRight, value);

                if (_rectangleGeometry != null)
                {
                    _rectangleGeometry.Rect = new Rect(TopLeft, value);
                    UpdateHandleLocation();
                    UpdateVisual();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => RenderGeometry.Bounds;
        }

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                SetField(ref _topLeft, value);

                if (_rectangleGeometry == null)
                {
                    _rectangleGeometry = new RectangleGeometry();
                    RenderGeometryGroup.Children.Add(_rectangleGeometry);
                    _rectangleGeometry.Rect = new Rect(value, value);
                }
                else
                {
                    _rectangleGeometry.Rect = new Rect(value, BottomRight);
                }

                UpdateHandleLocation();
                UpdateVisual();
            }
        }

        #endregion


        #region Implementations

        public void FromData(PointsData data)
        {
            if (data.DataPoints.Count != 2)
            {
                throw new Exception($"{nameof(PointsData)} must have 2 elements in  DataPoints");
            }

            //create handles

            CreateHandles();
            TopLeft = data.DataPoints[0];
            BottomRight = data.DataPoints[1];
            IsGeometryRendered = true;

            Tag = data.Tag;
            _tagPosition = data.TagPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PointsData GetMetaData()
        {
            return new PointsData(0,
                new List<Point> { _rectangleGeometry.Rect.TopLeft, _rectangleGeometry.Rect.BottomRight });
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {
            Handles.Clear();
            Handles.AddRange(CreateCornerHandles());
        }

        protected override void HandleResizing(Point point)
        {
            if (SelectedDragHandle != null)
                switch ((DragLocation)SelectedDragHandle.Id)
                {
                    case DragLocation.TopLeft:
                        TopLeft = ForcePointInRange(point, 0, BottomRight.X, 0, BottomRight.Y);
                        break;
                    case DragLocation.TopRight:
                        var validPointTopRight = ForcePointInRange(point, TopLeft.X, double.MaxValue, 0, BottomRight.Y);
                        TopLeft = new Point(TopLeft.X, validPointTopRight.Y);
                        BottomRight = new Point(validPointTopRight.X, BottomRight.Y);
                        break;
                    case DragLocation.BottomRight:
                        BottomRight = ForcePointInRange(point, TopLeft.X, double.MaxValue, TopLeft.Y, double.MaxValue);
                        break;
                    case DragLocation.BottomLeft:
                        var validPointBottomLeft = ForcePointInRange(point, 0, BottomRight.X, TopLeft.Y, double.MaxValue);
                        TopLeft = new Point(validPointBottomLeft.X, TopLeft.Y);
                        BottomRight = new Point(BottomRight.X, validPointBottomLeft.Y);
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

        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                TopLeft = mousePoint;
                CreateHandles();
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    BottomRight = ForcePointInRange(point, TopLeft.X, point.X, TopLeft.Y, point.Y);
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

        private void UpdateHandleLocation()
        {
            // Handles are created in order: TopLeft, TopRight, BottomLeft, BottomRight
            foreach (var handle in Handles)
            {
                switch ((DragLocation)handle.Id)
                {
                    case DragLocation.TopLeft:
                        handle.GeometryCenter = TopLeft;
                        break;
                    case DragLocation.TopRight:
                        handle.GeometryCenter = new Point(BottomRight.X, TopLeft.Y);
                        break;
                    case DragLocation.BottomRight:
                        handle.GeometryCenter = BottomRight;
                        break;
                    case DragLocation.BottomLeft:
                        handle.GeometryCenter = new Point(TopLeft.X, BottomRight.Y);
                        break;
                }
            }
        }

        public override void UpdateVisual()
        {
            if (_rectangleGeometry == null)
            {
                return;
            }

            var renderContext = RenderOpen();

            if (ShapeStyler != null)
            {
                AddTagText(renderContext, GetTagPosition());

                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _rectangleGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometryGroup);
                foreach (var dragHandle in Handles)
                    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, dragHandle.HandleGeometry);
            }

            renderContext.Close();
        }

        private Point GetTagPosition()
        {
            switch (_tagPosition)
            {
                case TagPosition.Center:
                    return TopLeft.MiddleWith(BottomRight) -new Vector(ShapeLayer.TagFontSize/2, ShapeLayer.TagFontSize/2);

                case TagPosition.Top:
                    return TopLeft - new Vector(0, ShapeLayer.TagFontSize);

                case TagPosition.Bottom:
                    return TopLeft + new Vector(0, BottomRight.Y - TopLeft.Y + ShapeLayer.TagFontSize);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public Rectangle(ShapeLayer layer) : base(layer)
        {
        }
    }
}