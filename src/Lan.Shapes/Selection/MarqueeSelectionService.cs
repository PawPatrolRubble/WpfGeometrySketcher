#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lan.Shapes.Selection
{
    /// <summary>
    /// Service for handling marquee (rubber band) selection of multiple shapes.
    /// Supports both window selection (fully contained) and crossing selection (intersecting).
    /// </summary>
    public class MarqueeSelectionService
    {
        private readonly SelectionRectangle _selectionRectangle;
        private readonly List<ShapeVisualBase> _selectedShapes = new();

        /// <summary>
        /// Event raised when the selection changes
        /// </summary>
        public event EventHandler<IReadOnlyList<ShapeVisualBase>>? SelectionChanged;

        /// <summary>
        /// The visual element for the selection rectangle
        /// </summary>
        public SelectionRectangle SelectionRectangle => _selectionRectangle;

        /// <summary>
        /// Currently selected shapes
        /// </summary>
        public IReadOnlyList<ShapeVisualBase> SelectedShapes => _selectedShapes;

        /// <summary>
        /// Whether a marquee selection is currently in progress
        /// </summary>
        public bool IsSelecting => _selectionRectangle.IsActive;

        /// <summary>
        /// Minimum drag distance (in pixels) before marquee selection activates.
        /// Prevents accidental marquee selection on simple clicks.
        /// </summary>
        public double MinimumDragDistance { get; set; } = 5.0;

        private Point _initialPoint;
        private bool _dragStarted;

        public MarqueeSelectionService()
        {
            _selectionRectangle = new SelectionRectangle();
        }

        /// <summary>
        /// Begin potential marquee selection (called on mouse down)
        /// </summary>
        public void BeginSelection(Point startPoint)
        {
            _initialPoint = startPoint;
            _dragStarted = false;
        }

        /// <summary>
        /// Update selection during drag (called on mouse move)
        /// </summary>
        /// <returns>True if marquee selection is active</returns>
        public bool UpdateSelection(Point currentPoint)
        {
            // Check if we've dragged far enough to start marquee selection
            if (!_dragStarted)
            {
                var distance = (currentPoint - _initialPoint).Length;
                if (distance >= MinimumDragDistance)
                {
                    _dragStarted = true;
                    _selectionRectangle.Begin(_initialPoint);
                }
            }

            if (_dragStarted)
            {
                _selectionRectangle.Update(currentPoint);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Complete the selection and find all shapes within the selection area
        /// </summary>
        /// <param name="allShapes">All shapes to test against</param>
        /// <param name="addToExisting">If true, add to existing selection (Shift+drag behavior)</param>
        /// <returns>List of selected shapes</returns>
        public IReadOnlyList<ShapeVisualBase> CompleteSelection(
            IEnumerable<ShapeVisualBase> allShapes, 
            bool addToExisting = false)
        {
            var shapesList = allShapes.ToList();
            System.Diagnostics.Debug.WriteLine($"[CompleteSelection] _dragStarted={_dragStarted}, allShapes.Count={shapesList.Count}");
            
            if (!_dragStarted)
            {
                // No marquee selection occurred (user just clicked without dragging)
                System.Diagnostics.Debug.WriteLine("[CompleteSelection] _dragStarted is false, returning empty");
                return _selectedShapes;
            }

            var selectionBounds = _selectionRectangle.End();
            var isCrossing = _selectionRectangle.IsCrossingSelection;
            
            System.Diagnostics.Debug.WriteLine($"[CompleteSelection] selectionBounds={selectionBounds}, isCrossing={isCrossing}");

            if (!addToExisting)
            {
                ClearSelection();
            }

            foreach (var shape in shapesList)
            {
                if (shape.IsLocked)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompleteSelection] Shape {shape.GetType().Name} is locked, skipping");
                    continue;
                }

                var shapeBounds = shape.BoundsRect;
                System.Diagnostics.Debug.WriteLine($"[CompleteSelection] Testing shape {shape.GetType().Name}, bounds={shapeBounds}");

                bool shouldSelect = isCrossing
                    ? IntersectsWithRect(shape, selectionBounds)  // Crossing: any overlap
                    : IsFullyContainedInRect(shape, selectionBounds);  // Window: fully inside

                System.Diagnostics.Debug.WriteLine($"[CompleteSelection] shouldSelect={shouldSelect}");

                if (shouldSelect && !_selectedShapes.Contains(shape))
                {
                    _selectedShapes.Add(shape);
                    System.Diagnostics.Debug.WriteLine($"[CompleteSelection] Added shape {shape.GetType().Name}");
                }
            }

            _dragStarted = false;
            OnSelectionChanged();
            System.Diagnostics.Debug.WriteLine($"[CompleteSelection] Final count: {_selectedShapes.Count}");
            return _selectedShapes;
        }

        /// <summary>
        /// Cancel the current selection operation
        /// </summary>
        public void CancelSelection()
        {
            _selectionRectangle.Cancel();
            _dragStarted = false;
        }

        /// <summary>
        /// Clear all selected shapes
        /// </summary>
        public void ClearSelection()
        {
            _selectedShapes.Clear();
            OnSelectionChanged();
        }

        /// <summary>
        /// Add a shape to the selection
        /// </summary>
        public void AddToSelection(ShapeVisualBase shape)
        {
            if (!_selectedShapes.Contains(shape))
            {
                _selectedShapes.Add(shape);
                OnSelectionChanged();
            }
        }

        /// <summary>
        /// Remove a shape from the selection
        /// </summary>
        public void RemoveFromSelection(ShapeVisualBase shape)
        {
            if (_selectedShapes.Remove(shape))
            {
                OnSelectionChanged();
            }
        }

        /// <summary>
        /// Toggle a shape's selection state
        /// </summary>
        public void ToggleSelection(ShapeVisualBase shape)
        {
            if (_selectedShapes.Contains(shape))
            {
                _selectedShapes.Remove(shape);
            }
            else
            {
                _selectedShapes.Add(shape);
            }
            OnSelectionChanged();
        }

        /// <summary>
        /// Check if a shape intersects with the selection rectangle (crossing selection)
        /// </summary>
        private static bool IntersectsWithRect(ShapeVisualBase shape, Rect selectionRect)
        {
            var shapeBounds = shape.BoundsRect;
            
            // Check if rectangles intersect
            return selectionRect.IntersectsWith(shapeBounds);
        }

        /// <summary>
        /// Check if a shape is fully contained within the selection rectangle (window selection)
        /// </summary>
        private static bool IsFullyContainedInRect(ShapeVisualBase shape, Rect selectionRect)
        {
            var shapeBounds = shape.BoundsRect;
            
            // Check if selection rect fully contains the shape bounds
            return selectionRect.Contains(shapeBounds);
        }

        /// <summary>
        /// More precise intersection test using the shape's actual geometry
        /// </summary>
        public static bool IntersectsWithGeometry(ShapeVisualBase shape, Rect selectionRect)
        {
            var shapeBounds = shape.BoundsRect;
            
            // Quick bounds check first
            if (!selectionRect.IntersectsWith(shapeBounds))
            {
                return false;
            }

            // For more precise checking, we could use the RenderGeometry
            // This is a simplified version using bounds
            var renderGeometry = shape.RenderGeometry;
            if (renderGeometry != null)
            {
                var selectionGeometry = new System.Windows.Media.RectangleGeometry(selectionRect);
                var intersection = System.Windows.Media.Geometry.Combine(
                    renderGeometry,
                    selectionGeometry,
                    System.Windows.Media.GeometryCombineMode.Intersect,
                    null);
                
                return !intersection.IsEmpty();
            }

            return true;
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, _selectedShapes);
        }
    }
}
