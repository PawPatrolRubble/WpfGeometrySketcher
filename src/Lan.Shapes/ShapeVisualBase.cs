#nullable enable

#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Enums;
using Lan.Shapes.Handle;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;

#endregion

namespace Lan.Shapes
{
    public abstract class ShapeVisualBase : DrawingVisual, INotifyPropertyChanged
    {
        #region fields

        /// <summary>
        /// geometries will be rendered for the final shape
        /// </summary>
        protected readonly GeometryGroup RenderGeometryGroup = new GeometryGroup();

        private bool _canMoveWithHand;
        private bool _isLocked;

        private ShapeVisualState _state;

        /// <summary>
        /// 
        /// </summary>
        protected GeometryGroup? HandleGeometryGroup;

        /// <summary>
        /// list of handles for drag and resizing
        /// </summary>
        protected List<DragHandle> Handles = new List<DragHandle>();

        protected Point? MouseDownPoint;

        /// <summary>
        /// the first point 
        /// </summary>
        protected Point? OldPointForTranslate;

        /// <summary>
        /// in this area translation of the shape will be allowed
        /// </summary>
        protected CombinedGeometry PanSensitiveArea = new CombinedGeometry();

        #endregion

        #region Propeties

        /// <summary>
        /// 
        /// </summary>
        public abstract Rect BoundsRect { get; }

        protected double DragHandleSize { get; set; }


        public Guid Id { get; }


        /// <summary>
        /// this is used to ensure that during resizing or pan moving, the mouse will always
        /// focus on the same shape, instead of moved to another one
        /// </summary>
        public bool IsBeingDraggedOrPanMoving { get; protected set; }

        /// <summary>
        /// set it to be true, if geometry is first Rendered
        /// </summary>
        public bool IsGeometryRendered { get; protected set; }

        public bool IsLocked
        {
            get => _isLocked;
            protected set
            {
                _isLocked = value;

                State = _isLocked ? ShapeVisualState.Locked : ShapeVisualState.Normal;

            }
        }

        public virtual Geometry RenderGeometry
        {
            get => RenderGeometryGroup;
        }

        protected DragHandle? SelectedDragHandle { get; set; }


        public ShapeLayer ShapeLayer { get; set; }

        /// <summary>
        /// the current valid styler should be given from layer base on the shape State
        /// </summary>
        public IShapeStyler? ShapeStyler
        {
            get => ShapeLayer?.GetStyler(State);
        }

        public ShapeVisualState State
        {
            get => _state;
            set
            {
                _state = value;
                UpdateVisual();
                //update handle size
                if (ShapeLayer != null)
                {
                    DragHandleSize = ShapeStyler?.DragHandleSize ?? 0;
                    OnDragHandleSizeChanges(DragHandleSize);
                }
            }
        }

        private string? _tag;

        public string? Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                UpdateVisual();
            }
        }

        #endregion

        #region Constructors

        protected ShapeVisualBase(ShapeLayer layer)
        {
            ShapeLayer = layer;
            Id = Guid.NewGuid();
            State = ShapeVisualState.Normal;
            //Tag = this.GetType().Name;
        }


        #endregion

        #region Implementations

        #region implementations

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #endregion

        #region others

        public virtual void Lock()
        {
            IsLocked = true;
        }

        public virtual void UnLock()
        {
            IsLocked = false;
        }

        protected virtual void OnDragHandleSizeChanges(double dragHandleSize)
        {
        }

        protected abstract void CreateHandles();

        protected DragHandle CreateRectDragHandle(Point location, int id)
        {
            if (ShapeStyler == null)
            {
                throw new Exception("Style cannot be null");
            }

            return new RectDragHandle(new Size(ShapeStyler.DragHandleSize, ShapeStyler.DragHandleSize), location, 10,
                id);
        }

        protected virtual void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
            //throw new NotImplementedException();
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


        protected double GetDistanceBetweenTwoPoint(Point p1, Point p2)
        {
            return (p2 - p1).Length;
        }

        protected Point GetMiddleToTwoPoints(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        protected abstract void HandleResizing(Point point);


        protected abstract void HandleTranslate(Point newPoint);

        /// <summary>
        /// 未选择状态
        /// </summary>
        public abstract void OnDeselected();

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


        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public virtual void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Released)
            {
                State = ShapeVisualState.MouseOver;

                if (HandleGeometryGroup?.FillContains(point) ?? false)
                {
                    var handle = FindDragHandleMouseOver(point);
                    if (handle != null)
                    {
                        UpdateMouseCursor((DragLocation)handle.Id);
                    }
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
                    if (MouseDownPoint != null)
                    {
                        DrawGeometryInMouseMove(MouseDownPoint.Value, point);
                    }

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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public abstract void OnSelected();

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void SetMouseCursorToHand()
        {
            Mouse.SetCursor(Cursors.Hand);
        }


        /// <summary>
        /// add geometries to group
        /// </summary>
        protected virtual void UpdateGeometryGroup()
        {
        }

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


        public virtual void UpdateVisual()
        {
            if (ShapeStyler == null)
            {
                return;
            }

            var renderContext = RenderOpen();
            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
            renderContext.Close();
        }

        #endregion

        protected double EnsureNumberWithinRange(double value, double min, double max)
        {
            value = Math.Min(value, max);
            value = Math.Max(value, min);
            return value;
        }

        /// <summary>
        /// if the point passed is out of the range defined after,
        /// it will used the maximum valid value
        /// </summary>
        /// <param name="point"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        protected Point ForcePointInRange(Point point, double minX, double maxX, double minY, double maxY)
        {
            var x = point.X;
            var y = point.Y;
            x = EnsureNumberWithinRange(x, minX, maxX);
            y = EnsureNumberWithinRange(y, minY, maxY);
            return new Point(x, y);
        }


        protected void AddTagText(DrawingContext renderContext, Point location)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                FormattedText formattedText = new FormattedText(
                    Tag,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    ShapeLayer.TagFontSize,
                    Brushes.Lime);

                renderContext.DrawText(formattedText, location);
            }
        }

        protected void AddTagText(DrawingContext renderContext, Point location, double angle)
        {
            if (!string.IsNullOrEmpty(Tag))
            {
                RotateTransform rt = new RotateTransform();

                rt.Angle = angle;
                rt.CenterX = location.X;
                rt.CenterY = location.Y;
                renderContext.PushTransform(rt);

                FormattedText formattedText = new FormattedText(
                    Tag,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    ShapeLayer.TagFontSize,
                    Brushes.Lime);

                renderContext.DrawText(formattedText, location);
                renderContext.Pop();
            }
        }
    }
}