﻿using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes.Handle;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    public class Circle : ShapeVisualBase, IDataExport<EllipseData>
    {
        #region constructor

        private DragHandle _dragHandle;

        #region Constructors

        public Circle(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            _dragHandle = new RectDragHandle(DragHandleSize, default, 1);
            RenderGeometryGroup.Children.Add(_ellipseGeometry);
        }

        #endregion

        #endregion

        #region interface implementation

        public void FromData(EllipseData data)
        {
            Center = data.Center;
            Radius = data.RadiusX;
            IsGeometryRendered = true;
        }

        public EllipseData GetMetaData()
        {
            return new EllipseData
            {
                Center = Center,
                RadiusX = Radius
            };
        }

        #endregion

        #region fields

        private readonly EllipseGeometry _ellipseGeometry = new EllipseGeometry(default);
        private readonly LineGeometry _verticalLine = new LineGeometry();
        private readonly LineGeometry _horizontalLine = new LineGeometry();

        private readonly int crossSize = 40;
        private Point _center;

        #endregion

        #region Propeties

        /// <summary>
        /// </summary>
        public override Rect BoundsRect
        {
            get { return _ellipseGeometry.Bounds; }
        }

        public Point Center
        {
            get { return _center; }
            set
            {
                SetField(ref _center, value);
                UpdateGeometryGroup();
            }
        }


        private double _radius;

        public double Radius
        {
            get { return _radius; }
            set
            {
                SetField(ref _radius, value);
                UpdateGeometryGroup();
            }
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {

        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            Radius = (newPoint.X - oldPoint.X) / 2;
        }

        protected override void HandleResizing(Point point)
        {
            if (MouseDownPoint.HasValue)
            {
                switch (SelectedDragHandle!.Id)
                {
                    case 2:
                        Radius += MouseDownPoint.Value.Y - point.Y;
                        break;

                    case 1:
                        Radius += point.X - MouseDownPoint.Value.X;
                        break;
                }
            }

            MouseDownPoint = point;
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (!OldPointForTranslate.HasValue)
            {
                return;
            }

            var matrix = new Matrix();
            matrix.Translate(newPoint.X - OldPointForTranslate.Value.X, newPoint.Y - OldPointForTranslate.Value.Y);
            Center = matrix.Transform(Center);
            OldPointForTranslate = newPoint;
        }

        /// <summary>
        ///     未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                Center = mousePoint;
            }
            else
            {
                FindSelectedHandle(mousePoint);
            }

            OldPointForTranslate = mousePoint;
            MouseDownPoint = mousePoint;
        }

        public override void FindSelectedHandle(Point p)
        {
            SelectedDragHandle = _dragHandle.FillContains(p) ? _dragHandle : null;
        }


        /// <summary>
        ///     鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered && OldPointForTranslate.HasValue)
                {
                    Radius = (point.X - OldPointForTranslate.Value.X) / 2;
                    Radius = (point.Y - OldPointForTranslate.Value.Y) / 2;
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


        /// <summary>
        ///     选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     add geometries to group
        /// </summary>
        protected void UpdateGeometryGroup([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(Center):
                    _ellipseGeometry.Center = Center;
                    _dragHandle.GeometryCenter = Center + new Vector(Radius, 0);
                    _verticalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(0, -crossSize * 1.0 / 2);
                    _verticalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(0, crossSize * 1.0 / 2);

                    _horizontalLine.StartPoint = new Point(_center.X, _center.Y) + new Vector(-crossSize * 1.0 / 2, 0);
                    _horizontalLine.EndPoint = new Point(_center.X, _center.Y) + new Vector(crossSize * 1.0 / 2, 0);
                    UpdateVisual();

                    break;

                case nameof(Radius):

                    if (Radius > 0)
                    {
                        _ellipseGeometry.RadiusX = Radius;
                        _ellipseGeometry.RadiusY = Radius;
                        _dragHandle.GeometryCenter = _ellipseGeometry.Center + new Vector(Radius, 0);
                    }
                    UpdateVisual();
                    break;

            }

        }

        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _verticalLine);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _horizontalLine);
                if (_dragHandle?.HandleGeometry != null)
                {
                    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _dragHandle.HandleGeometry);
                }

                AddTagText(renderContext, Center);
            }

            renderContext.Close();
        }

        #endregion
    }
}