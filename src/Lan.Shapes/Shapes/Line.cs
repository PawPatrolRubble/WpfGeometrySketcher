﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes.Handle;

namespace Lan.Shapes.Shapes
{
    public class Line : ShapeVisualBase
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

            UpdateVisual();
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
                UpdateVisual();
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
    }
}