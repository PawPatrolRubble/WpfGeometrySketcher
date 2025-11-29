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

        #region Properties

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
                var oldState = _state;
                _state = value;
                if (oldState != value)
                {
                    UpdateVisualOnStateChanged();
                    //update handle size
                    if (ShapeLayer != null)
                    {
                        DragHandleSize = ShapeStyler?.DragHandleSize ?? 0;
                        OnDragHandleSizeChanges(DragHandleSize);
                    }
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

        protected virtual void UpdateVisualOnStateChanged()
        {
            switch (State)
            {
                case ShapeVisualState.Selected:
                case ShapeVisualState.MouseOver:
                case ShapeVisualState.Normal:
                    UpdateVisual();
                    break;
                case ShapeVisualState.Locked:
                    UpdateVisualOnLocked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void UpdateVisualOnLocked()
        {
            //do nothing, but child classes can override to remove some geometries when locked
            UpdateVisual();
        }


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

        #region Factory Methods for Handle Creation

        /// <summary>
        /// Factory Method: Create a rectangular drag handle at the specified location.
        /// </summary>
        /// <param name="location">Center position of the handle</param>
        /// <param name="dragLocation">The drag location enum value (used as ID)</param>
        /// <returns>A new RectDragHandle instance</returns>
        protected DragHandle CreateHandle(DragLocation dragLocation, Point location = default)
        {
            var size = GetHandleSize();
            return new RectDragHandle(size, location, 10, (int)dragLocation);
        }

        /// <summary>
        /// Factory Method: Create a rectangular drag handle with a custom ID.
        /// </summary>
        protected DragHandle CreateRectDragHandle(Point location, int id)
        {
            var size = GetHandleSize();
            return new RectDragHandle(size, location, 10, id);
        }

        /// <summary>
        /// Factory Method: Create multiple handles for common corner positions.
        /// </summary>
        protected IEnumerable<DragHandle> CreateCornerHandles()
        {
            yield return CreateHandle(DragLocation.TopLeft);
            yield return CreateHandle(DragLocation.TopRight);
            yield return CreateHandle(DragLocation.BottomLeft);
            yield return CreateHandle(DragLocation.BottomRight);
        }

        /// <summary>
        /// Factory Method: Create handles for all 8 positions (corners + midpoints).
        /// </summary>
        protected IEnumerable<DragHandle> CreateAllHandles()
        {
            yield return CreateHandle(DragLocation.TopLeft);
            yield return CreateHandle(DragLocation.TopMiddle);
            yield return CreateHandle(DragLocation.TopRight);
            yield return CreateHandle(DragLocation.RightMiddle);
            yield return CreateHandle(DragLocation.BottomRight);
            yield return CreateHandle(DragLocation.BottomMiddle);
            yield return CreateHandle(DragLocation.BottomLeft);
            yield return CreateHandle(DragLocation.LeftMiddle);
        }

        /// <summary>
        /// Get the current handle size based on styler settings.
        /// </summary>
        private Size GetHandleSize()
        {
            var handleSize = ShapeStyler?.DragHandleSize ?? 10;
            return new Size(handleSize, handleSize);
        }

        #endregion

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
        /// Called when the shape is deselected
        /// </summary>
        public virtual void OnDeselected()
        {
            State = ShapeVisualState.Normal;
        }

        /// <summary>
        /// Left mouse button down event
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
        /// When mouse left button up
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
        /// Handles mouse move events for shape interaction
        /// </summary>
        public virtual void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Released)
            {
                HandleMouseHover(point);
            }
            else
            {
                HandleMouseDrag(point);
            }

            OldPointForTranslate = point;
        }

        /// <summary>
        /// Handles mouse hover behavior (cursor updates, state changes)
        /// </summary>
        protected virtual void HandleMouseHover(Point point)
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

        /// <summary>
        /// Handles mouse drag behavior (drawing, resizing, translating)
        /// </summary>
        protected virtual void HandleMouseDrag(Point point)
        {
            if (IsGeometryRendered)
            {
                HandleRenderedShapeDrag(point);
            }
            else
            {
                HandleDrawingDrag(point);
            }
        }

        /// <summary>
        /// Handles drag operations on an already rendered shape
        /// </summary>
        private void HandleRenderedShapeDrag(Point point)
        {
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

        /// <summary>
        /// Handles drag operations while initially drawing a shape
        /// </summary>
        protected virtual void HandleDrawingDrag(Point point)
        {
            if (MouseDownPoint != null)
            {
                DrawGeometryInMouseMove(MouseDownPoint.Value, point);
            }

            CreateHandles();
            UpdateGeometryGroup();
            UpdateVisual();
        }

        public virtual void OnMouseRightButtonUp(Point mousePosition)
        {
            IsGeometryRendered = true;
            State = ShapeVisualState.Normal;
        }

        public virtual void OnMouseLeftButtonDoubleClick(Point mouseDoubleClickPoint)
        {
            // Override in derived classes to handle double-click
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the shape is selected
        /// </summary>
        public virtual void OnSelected()
        {
            State = ShapeVisualState.Selected;
        }

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
        /// Add geometries to group
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

        /// <summary>
        /// Template Method for rendering the shape.
        /// Override the hook methods (DrawMainGeometry, DrawHandles, DrawDecorations) 
        /// to customize rendering behavior.
        /// </summary>
        public virtual void UpdateVisual()
        {
            if (ShapeStyler == null)
            {
                return;
            }

            var renderContext = RenderOpen();
            try
            {
                DrawMainGeometry(renderContext);
                DrawHandles(renderContext);
                DrawDecorations(renderContext);
            }
            finally
            {
                renderContext.Close();
            }
        }

        /// <summary>
        /// Hook method: Draw the main geometry of the shape.
        /// Override to customize main geometry rendering.
        /// </summary>
        protected virtual void DrawMainGeometry(DrawingContext context)
        {
            context.DrawGeometry(ShapeStyler!.FillColor, ShapeStyler.SketchPen, RenderGeometry);
        }

        /// <summary>
        /// Hook method: Draw drag handles.
        /// Override to customize handle rendering.
        /// </summary>
        protected virtual void DrawHandles(DrawingContext context)
        {
            foreach (var handle in Handles)
            {
                if (handle.HandleGeometry != null)
                {
                    context.DrawGeometry(ShapeStyler!.FillColor, ShapeStyler.SketchPen, handle.HandleGeometry);
                }
            }
        }

        /// <summary>
        /// Hook method: Draw additional decorations (text, indicators, etc.).
        /// Override to add custom decorations.
        /// </summary>
        protected virtual void DrawDecorations(DrawingContext context)
        {
            // Default: no decorations. Override in derived classes.
        }

        #endregion

        #region Helper Methods

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
                    Brushes.Red,
                    40);

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
                    Brushes.Lime,
                    96);

                renderContext.DrawText(formattedText, location);
                renderContext.Pop();
            }
        }


        protected List<(Point Location, string Content)> _textGeometries = new List<(Point Location, string Content)>();
        public virtual void AddText(string content, Point? location = null)
        {
            if (string.IsNullOrEmpty(content) || location == null)
            {
                return;
            }

            var renderContext = RenderOpen();
            var formattedText = new FormattedText(
                content,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            renderContext.DrawText(formattedText, location.Value);
            renderContext.Close();

            _textGeometries.Add((location.Value, content));
        }


        protected void DrawText()
        {
            foreach (var textGeometry in _textGeometries)
            {
                var renderContext = RenderOpen();
                var formattedText = new FormattedText(
                    textGeometry.Content,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    ShapeLayer.TagFontSize,
                    Brushes.Red,
                    96);

                renderContext.DrawText(formattedText, textGeometry.Location);
                renderContext.Close();
            }
        }
    }
}
#endregion