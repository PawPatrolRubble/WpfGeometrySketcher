using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    public class Line : ShapeVisualBase, IDataExport<PointsData>
    {
        #region constructor

        public Line(ShapeLayer layer) : base(layer)
        {
            _leftDragHandle = new RectDragHandle(DragHandleSize, default, 1);
            _rightDragHandle = new RectDragHandle(DragHandleSize, default, 2);
            _panHandle = new RectDragHandle(DragHandleSize, default, 2);

            RenderGeometryGroup.Children.Add(_lineGeometry);
            RenderGeometryGroup.Children.Add(_leftDragHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_rightDragHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_panHandle.HandleGeometry);

        }

        #endregion

        #region private fields

        private readonly DragHandle _panHandle;
        private readonly DragHandle _leftDragHandle; //= new RectDragHandle(10, default, 1);
        private readonly LineGeometry _lineGeometry = new LineGeometry();
        private readonly DragHandle _rightDragHandle; // = new RectDragHandle(10, default, 2);

        private Point _end;
        private Point _start;

        #endregion

        #region properties

        public Point Start
        {
            get { return _start; }
            set
            {
                SetField(ref _start, value);
                UpdateGeometry();
            }
        }

        public Point End
        {
            get { return _end; }
            set
            {
                SetField(ref _end, value);
                UpdateGeometry();
            }
        }


        public override Rect BoundsRect
        {
            get { return RenderGeometryGroup.Bounds; }
        }

        #endregion

        #region other members

        private void UpdateGeometry()
        {
            _lineGeometry.StartPoint = Start;
            _lineGeometry.EndPoint = End;

            _leftDragHandle.GeometryCenter = Start;
            _rightDragHandle.GeometryCenter = End;
            _panHandle.GeometryCenter = new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);

            RenderVisualWithLength();
        }

        protected override void CreateHandles()
        {

        }

        protected override void HandleResizing(Point point)
        {

        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                Start += newPoint - OldPointForTranslate.Value;
                End += newPoint - OldPointForTranslate.Value;
                RenderVisualWithLength();
                OldPointForTranslate = newPoint;
            }
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
            base.OnMouseLeftButtonDown(mousePoint);
            if (!IsGeometryRendered)
            {
                Start = mousePoint;
                End = mousePoint + new Vector(10, 10);
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
        }

        public override void FindSelectedHandle(Point p)
        {
            switch (p)
            {
                case var point when _leftDragHandle.FillContains(point):
                    SelectedDragHandle = _leftDragHandle;
                    break;
                case var point when _rightDragHandle.FillContains(point):
                    SelectedDragHandle = _rightDragHandle;
                    break;
                case var point when _panHandle.FillContains(point):
                    SelectedDragHandle = _panHandle;
                    break;
            }
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            base.OnMouseMove(point, buttonState);

            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    End = point;
                }
                else // handle reallocation of start and end points of line
                {

                    if (SelectedDragHandle != null)
                    {
                        switch (SelectedDragHandle)
                        {
                            case var handle when handle == _leftDragHandle:
                                Start = point;
                                break;
                            case var handle when handle == _rightDragHandle:
                                End = point;
                                break;
                            default:
                                HandleTranslate(point);
                                break;
                        }
                    }
                }
            }
        }

        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            base.OnMouseLeftButtonUp(newPoint);
            SelectedDragHandle = null;
        }

        #endregion

        private void RenderVisualWithLength()
        {
            if (ShapeStyler == null)
            {
                return;
            }

            var renderContext = RenderOpen();
            
            // Draw the line geometry
            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
            
            DrawLengthText(renderContext);
            renderContext.Close();
        }

        private void DrawLengthText(DrawingContext renderContext)
        {
            // Draw the length text
            var length = Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
            
            var formattedText = new FormattedText(
                length.ToString("f4"),
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            renderContext.DrawText(formattedText, new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2));
        }

        // Kept for backward compatibility but marked as obsolete
        [Obsolete("Use RenderVisualWithLength instead")]
        private void ShowLineLength()
        {
            // This method is kept for backward compatibility
            // but should not be used anymore
            RenderVisualWithLength();
        }

        #region Overrides of ShapeVisualBase

        protected override void UpdateVisualOnLocked()
        {
            //base.UpdateVisualOnLocked();
            var renderContext = RenderOpen();
            // Draw the line geometry
            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _lineGeometry);
            DrawLengthText(renderContext);
            renderContext.Close();

        }

        #endregion


        public void FromData(PointsData data)
        {
            if (data.DataPoints.Count != 2)
            {
                throw new Exception($"{nameof(PointsData)} must have 2 elements in  DataPoints");
            }

            // Set the points
            Start = data.DataPoints[0];
            End = data.DataPoints[1];

            // Since we can't modify the drag handle size directly (immutable after creation),
            // we need to recreate the geometry group with properly sized handles
            RenderGeometryGroup.Children.Clear();

            // Recreate the geometry group with the current drag handle size
            RenderGeometryGroup.Children.Add(_lineGeometry);
            RenderGeometryGroup.Children.Add(_leftDragHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_rightDragHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_panHandle.HandleGeometry);

            // Force update of geometry to ensure everything is properly positioned
            UpdateGeometry();
            IsGeometryRendered = true;
        }

        public PointsData GetMetaData()
        {
            return new PointsData(1, new List<Point>()
            {
                Start,
                End
            });
        }
    }
}