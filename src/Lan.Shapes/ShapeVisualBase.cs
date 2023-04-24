#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    public abstract class ShapeVisualBase : DrawingVisual
    {
        #region fields

        private ShapeVisualState _state;

        public ShapeVisualState State
        {
            get => _state;
            set
            {
                _state = value; 
                UpdateVisual();
            }
        }


        /// <summary>
        /// this is used to ensure that during resizing or pan moving, the mouse will always
        /// focus on the same shape, instead of moved to another one
        /// </summary>
        public bool IsBeingDraggedOrPanMoving { get; protected set; }

        /// <summary>
        /// in this area translation of the shape will be allowed
        /// </summary>
        protected CombinedGeometry PanSensitiveArea = new CombinedGeometry();

        /// <summary>
        /// 
        /// </summary>
        protected GeometryGroup? HandleGeometryGroup;

        protected Point? MouseDownPoint;

        /// <summary>
        /// the first point 
        /// </summary>
        protected Point? OldPointForTranslate;

        /// <summary>
        /// geometries will be rendered for the final shape
        /// </summary>
        protected readonly GeometryGroup RenderGeometryGroup = new GeometryGroup();

        /// <summary>
        /// list of handles for drag and resizing
        /// </summary>
        protected List<DragHandle> Handles = new List<DragHandle>();



        private bool _canMoveWithHand;

        #endregion

        
        public ShapeLayer? ShapeLayer { get; set; }

        /// <summary>
        /// set it to be true, if geometry is first Rendered
        /// </summary>
        public bool IsGeometryRendered { get; protected set; }
        
        /// <summary>
        /// the current valid styler should be given from layer base on the shape State
        /// </summary>
        public IShapeStyler? ShapeStyler => ShapeLayer?.GetStyler(State);

        public virtual Geometry RenderGeometry { get => RenderGeometryGroup; }

        /// <summary>
        /// 
        /// </summary>
        public abstract Rect BoundsRect { get; }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public virtual void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (HandleGeometryGroup?.FillContains(mousePoint) ?? false)
            {
                FindSelectedHandle(mousePoint);
            }
            else
            {
                SelectedDragHandle = null;
            }

            OldPointForTranslate = mousePoint;
            MouseDownPoint ??= mousePoint;
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public virtual void OnMouseLeftButtonUp(Point newPoint)
        {
            if (!IsGeometryRendered && RenderGeometryGroup.Children.Count > 0)
            {
                IsGeometryRendered = true;
            }

            SelectedDragHandle = null;
            IsBeingDraggedOrPanMoving = false;
        }


        protected double GetDistanceBetweenTwoPoint(Point p1, Point p2)
        {
            return (p2 - p1).Length;
        }


        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public virtual void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Released)
            {
                State = ShapeVisualState.MouseOver;

                if ((HandleGeometryGroup?.FillContains(point) ?? false))
                {
                    var handle = FindDragHandleMouseOver(point);
                    if (handle != null) UpdateMouseCursor((DragLocation)handle.Id);
                }


                if (PanSensitiveArea.FillContains(point))
                {
                    Mouse.SetCursor(Cursors.Hand);
                    _canMoveWithHand = true;
                }
                else
                {
                    _canMoveWithHand = false;
                }
            }
            else //when the mouse left button is pressed
            {
                if (IsGeometryRendered)
                {
                    //scale operation
                    if (SelectedDragHandle != null)
                    {
                        IsBeingDraggedOrPanMoving = true;
                        UpdateMouseCursor((DragLocation)SelectedDragHandle.Id);
                        HandleResizing(point);
                        CreateHandles();
                        UpdateGeometryGroup();
                        UpdateVisual();
                        return;
                    }

                    if (_canMoveWithHand)
                    {
                        HandleTranslate(point);
                        CreateHandles();
                        UpdateGeometryGroup();
                        UpdateVisual();
                    }
                }
                else
                {
                    if (MouseDownPoint != null) DrawGeometryInMouseMove(MouseDownPoint.Value, point);
                    CreateHandles();
                    UpdateGeometryGroup();
                    UpdateVisual();
                }

            }


            OldPointForTranslate = point;
        }


        public virtual void OnMouseRightButtonUp(Point mousePosition)
        {
            IsGeometryRendered = true;
            State = ShapeVisualState.Normal;
        }


        /// <summary>
        /// add geometries to group
        /// </summary>
        protected abstract void UpdateGeometryGroup();

        protected abstract void DrawGeometryInMouseMove(Point oldPoint, Point newPoint);

        protected abstract void HandleResizing(Point point);

        /// <summary>
        /// 选择时
        /// </summary>
        public abstract void OnSelected();

        /// <summary>
        /// 未选择状态
        /// </summary>
        public abstract void OnDeselected();


        public virtual void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
            renderContext.Close();
        }



        public DragHandle? FindDragHandleMouseOver(Point p)
        {
            foreach (var handle in Handles)
            {
                if (handle.FillContains(p))
                {
                    return handle;
                }
            }

            return null;
        }

        public virtual void FindSelectedHandle(Point p)
        {
            SelectedDragHandle = FindDragHandleMouseOver(p);
        }

        protected DragHandle CreateRectDragHandle(Point location, int id)
        {
            if (ShapeStyler == null)
            {
                throw new Exception("Style cannot be null");
            }
            return new RectDragHandle(new Size(ShapeStyler.DragHandleSize, ShapeStyler.DragHandleSize), location, 10, id);
        }

        protected DragHandle? SelectedDragHandle { get; set; }

        protected abstract void CreateHandles();


        protected abstract void HandleTranslate(Point newPoint);

        public void UpdateMouseCursor(DragLocation dragLocation)
        {
            switch (dragLocation)
            {
                case DragLocation.TopLeft:
                    Mouse.SetCursor(Cursors.SizeNWSE);

                    break;
                case DragLocation.TopMiddle:
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case DragLocation.TopRight:
                    Mouse.SetCursor(Cursors.SizeNESW);

                    break;
                case DragLocation.RightMiddle:
                    Mouse.SetCursor(Cursors.SizeWE);

                    break;
                case DragLocation.BottomRight:
                    Mouse.SetCursor(Cursors.SizeNWSE);
                    break;
                case DragLocation.BottomMiddle:
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case DragLocation.BottomLeft:
                    Mouse.SetCursor(Cursors.SizeNESW);

                    break;
                case DragLocation.LeftMiddle:
                    Mouse.SetCursor(Cursors.SizeWE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dragLocation), dragLocation, null);
            }
        }
    }
}