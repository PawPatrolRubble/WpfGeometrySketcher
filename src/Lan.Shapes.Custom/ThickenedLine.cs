#region

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Enums;
using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;
using Point = System.Windows.Point;

#endregion

namespace Lan.Shapes.Custom
{
    public class ThickenedLine : CustomGeometryBase, IDataExport<PointsData>
    {
        #region fields

        private readonly DragHandle _leftDragHandle; //= new RectDragHandle(10, default, 1);
        private readonly DragHandle _rightDragHandle;// = new RectDragHandle(10, default, 2);

        private readonly LineGeometry _lineGeometry = new LineGeometry(default, default);

        private Point _end;

        private Point _start;

        #endregion

        #region Propeties

        public Point End
        {
            get => _end;
            set
            {
                _end = value;
                OnEndPointsChanges();
            }
        }

        public Point Start
        {
            get => _start;
            set
            {
                _start = value;
                OnStartPointChanges();
            }
        }

        #endregion

        #region Constructors

        public ThickenedLine(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            RenderGeometryGroup.Children.Add(_lineGeometry);
            _leftDragHandle = new RectDragHandle(DragHandleSize, default, 1);
            _rightDragHandle = new RectDragHandle(DragHandleSize, default, 2);
        }

        #endregion

        #region others

        public override void FindSelectedHandle(Point p)
        {
            if (_rightDragHandle.FillContains(p))
            {
                SelectedDragHandle = _rightDragHandle;
                return;
            }

            if (_leftDragHandle.FillContains(p))
            {
                SelectedDragHandle = _leftDragHandle;
                return;
            }

            if (DistanceResizeHandle.FillContains(p)) SelectedDragHandle = DistanceResizeHandle;
        }


        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                Start += newPoint - OldPointForTranslate.Value;
                End += newPoint - OldPointForTranslate.Value;
                UpdateVisual();
                OldPointForTranslate = newPoint;
            }
        }

        private void OnEndPointsChanges()
        {
            _lineGeometry.EndPoint = End;
            _rightDragHandle.GeometryCenter = End;
            UpdateResizeHandleLocation();
            UpdateVisual();
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonDown(Point newPoint)
        {
            if (!IsGeometryRendered && Start == default)
            {
                Start = newPoint;
                End = newPoint + new Vector(10, 10);
            }
            else if (End == default)
                End = newPoint;
            else
                FindSelectedHandle(newPoint);
            OldPointForTranslate = newPoint;
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (End != default) base.OnMouseLeftButtonUp(newPoint);

        }

        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (!IsGeometryRendered)
            {
                End = point;
                return;
            }

            if (buttonState != MouseButtonState.Pressed) return;
            IsBeingDraggedOrPanMoving = true;
            switch (SelectedDragHandle?.Id ?? 0)
            {
                case 1:
                    Start = point;
                    break;
                case 2:
                    End = point;
                    break;
                case 99:
                    if (OldPointForTranslate.HasValue)
                    {
                        var deltaX = (point - OldPointForTranslate.Value).X;
                        var deltaY = (point - OldPointForTranslate.Value).Y;

                        if (deltaX <= 0 && deltaY <= 0)
                        {
                            StrokeThickness -= Math.Min(deltaX, deltaY);
                        }
                        else if (deltaX >= 0 && deltaY <= 0)
                        {
                            StrokeThickness += Math.Max(deltaX, deltaY);
                        }
                        else if (deltaX >= 0 && deltaY >= 0)
                        {
                            StrokeThickness -= Math.Max(deltaX, deltaY);
                        }
                        else if (deltaX <= 0 && deltaY >= 0)
                        {
                            StrokeThickness -= deltaX;
                        }



                        OldPointForTranslate = point;
                    }

                    break;
                default:
                    HandleTranslate(point);
                    break;
            }

            UpdateVisual();
        }


        private void OnStartPointChanges()
        {
            _lineGeometry.StartPoint = Start;
            _leftDragHandle.GeometryCenter = Start;

            //wait until end has value
            if (End == default) return;
            UpdateResizeHandleLocation();
            UpdateVisual();
        }


        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            UpdateResizeHandleLocation();
        }

        private void UpdateResizeHandleLocation()
        {
            DistanceResizeHandle.GeometryCenter =
                GetMiddleToTwoPoints(Start, End) + new Vector(0, -StrokeThickness / 2);
        }


        protected virtual bool CanRenderGeometry()
        {
            if (DistanceResizeHandle == null || _leftDragHandle == null || _rightDragHandle == null) {
                return false;
            }

            if (ShapeStyler == null) return false;
            return true;
        }

        public override void UpdateVisual()
        {
            if (!CanRenderGeometry())
            {
                return;
            }

            Pen ??= ShapeStyler.SketchPen.CloneCurrentValue();
            Pen.Thickness = StrokeThickness;


            var render = RenderOpen();


            switch (State)
            {
                case ShapeVisualState.Normal:
                    Pen.Brush = ShapeStyler.FillColor;
                    Pen.Brush.Opacity = 0.3;

                    break;
                case ShapeVisualState.Locked:
                    Pen.Brush = ShapeStyler.FillColor;

                    break;
                case ShapeVisualState.Selected:
                    Pen.Brush = ShapeStyler.FillColor;
                    Pen.Brush.Opacity = 0.8;
                    break;
                case ShapeVisualState.MouseOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            render.DrawGeometry(ShapeStyler.FillColor, Pen, RenderGeometry);
            render.DrawGeometry(DragHandleFillColor, DragHandlePen, DistanceResizeHandle.HandleGeometry);
            render.DrawGeometry(DragHandleFillColor, DragHandlePen, _leftDragHandle.HandleGeometry);
            render.DrawGeometry(DragHandleFillColor, DragHandlePen, _rightDragHandle.HandleGeometry);

            var angle = GetAngleBetweenPoints(Start, End);

            AddTagText(render, Start - new Vector(0, ShapeLayer.TagFontSize + StrokeThickness), angle);
            render.Close();
        }

        public static int GetAngleBetweenPoints(Point pt1, Point pt2)
        {
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;

            int deg = Convert.ToInt32(Math.Atan2(dy, dx) * (180 / Math.PI));
            if (deg < 0) { deg += 360; }

            return deg;
        }

        #endregion

        public void FromData(PointsData data)
        {
            if (data.DataPoints.Count >= 2)
            {
                Start = data.DataPoints[0];
                End = data.DataPoints[1];
                StrokeThickness = data.StrokeThickness;
            }
            else
            {
                throw new Exception($"提供的点数据不足，必须大于2个点。");
            }

        }

        public PointsData GetMetaData()
        {
            return new PointsData(StrokeThickness, new List<Point>() { Start, End });
        }
    }
}