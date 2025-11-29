#nullable enable

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Lan.Shapes;
using Lan.Shapes.Enums;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Selection;

#endregion

namespace Lan.SketchBoard
{
    public class SketchBoard : Canvas, ISketchBoard
    {
        #region fields

        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            "SketchBoardDataManager", typeof(ISketchBoardDataManager), typeof(SketchBoard),
            new PropertyMetadata(default(ISketchBoardDataManager), OnSketchBoardDataManagerChangedCallBack));


        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(SketchBoard), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// Marquee selection service for multi-shape selection
        /// </summary>
        private readonly MarqueeSelectionService _marqueeSelection = new();

        /// <summary>
        /// Whether marquee selection mode is active (no shape type selected)
        /// </summary>
        private bool _isMarqueeSelectionMode;

        /// <summary>
        /// Whether we are currently moving multiple selected shapes
        /// </summary>
        private bool _isMovingMultipleShapes;

        /// <summary>
        /// Starting point for potential marquee selection
        /// </summary>
        private Point _mouseDownPoint;

        /// <summary>
        /// Last mouse position for calculating move offset
        /// </summary>
        private Point _lastMousePosition;

        #endregion

        #region Coordinate Transformation

        /// <summary>
        /// Transform a point from screen coordinates to local (shape) coordinates.
        /// This accounts for any RenderTransform applied to parent containers (like zoom).
        /// </summary>
        private Point TransformToLocalCoordinates(Point screenPoint)
        {
            // Check parent's transform (GridContainer in ImageViewer) first
            if (this.Parent is FrameworkElement parent)
            {
                var parentTransform = parent.RenderTransform;
                if (parentTransform != null && parentTransform != Transform.Identity)
                {
                    var inverse = parentTransform.Inverse;
                    if (inverse != null)
                    {
                        Debug.WriteLine($"[TransformToLocalCoordinates] Using parent transform inverse. Input={screenPoint}");
                        var result = inverse.Transform(screenPoint);
                        Debug.WriteLine($"[TransformToLocalCoordinates] Output={result}");
                        return result;
                    }
                }
            }
            
            // If there's a RenderTransform on this element
            var transform = this.RenderTransform;
            if (transform != null && transform != Transform.Identity)
            {
                var inverse = transform.Inverse;
                if (inverse != null)
                {
                    return inverse.Transform(screenPoint);
                }
            }
            
            return screenPoint;
        }

        /// <summary>
        /// Transform a rectangle from screen coordinates to local (shape) coordinates.
        /// </summary>
        private Rect TransformRectToLocalCoordinates(Rect screenRect)
        {
            var topLeft = TransformToLocalCoordinates(screenRect.TopLeft);
            var bottomRight = TransformToLocalCoordinates(screenRect.BottomRight);
            return new Rect(topLeft, bottomRight);
        }

        /// <summary>
        /// Find all shapes within the specified screen bounds using visual hit testing.
        /// </summary>
        /// <param name="screenBounds">The selection bounds in screen coordinates</param>
        /// <param name="isCrossing">If true, select shapes that intersect. If false, select only fully contained shapes.</param>
        private List<ShapeVisualBase> FindShapesInBounds(Rect screenBounds, bool isCrossing)
        {
            var result = new List<ShapeVisualBase>();
            
            if (SketchBoardDataManager == null) return result;
            
            // Use visual hit testing with a geometry
            var selectionGeometry = new RectangleGeometry(screenBounds);
            
            VisualTreeHelper.HitTest(
                this,
                null,
                hitResult =>
                {
                    if (hitResult.VisualHit is ShapeVisualBase shape && !shape.IsLocked)
                    {
                        if (!result.Contains(shape))
                        {
                            // For crossing selection, any hit is enough
                            // For window selection, we need to check if fully contained
                            if (isCrossing)
                            {
                                result.Add(shape);
                                Debug.WriteLine($"[FindShapesInBounds] HitTest found (crossing): {shape.GetType().Name}");
                            }
                            else
                            {
                                // Check if shape is fully contained by getting its visual bounds
                                var shapeBounds = VisualTreeHelper.GetDescendantBounds(shape);
                                var shapeScreenBounds = shape.TransformToAncestor(this).TransformBounds(shapeBounds);
                                
                                if (screenBounds.Contains(shapeScreenBounds))
                                {
                                    result.Add(shape);
                                    Debug.WriteLine($"[FindShapesInBounds] HitTest found (window): {shape.GetType().Name}");
                                }
                            }
                        }
                    }
                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(selectionGeometry));
            
            Debug.WriteLine($"[FindShapesInBounds] Found {result.Count} shapes via hit testing");
            return result;
        }

        #endregion

        #region Properties

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public ISketchBoardDataManager? SketchBoardDataManager
        {
            get => (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty);
            set => SetValue(SketchBoardDataManagerProperty, value);
        }

        /// <summary>
        /// Access to the marquee selection service
        /// </summary>
        public MarqueeSelectionService MarqueeSelection => _marqueeSelection;

        #endregion


        public SketchBoard()
        {
            SizeChanged += SketchBoard_SizeChanged;
        }

        /// <summary>Invoked when an unhandled KeyDown attached event reaches an element in its route that is derived from this class.</summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Debug.WriteLine($"[OnKeyDown] Key={e.Key}, SelectedShapes.Count={SketchBoardDataManager?.SelectedShapes.Count ?? 0}");
            
            if (e.Key == Key.Delete && SketchBoardDataManager != null)
            {
                Debug.WriteLine($"[OnKeyDown] Delete pressed. SelectedShapes={SketchBoardDataManager.SelectedShapes.Count}, SelectedGeometry={SketchBoardDataManager.SelectedGeometry?.GetType().Name ?? "null"}");
                
                // Delete all selected shapes (multi-selection support)
                if (SketchBoardDataManager.SelectedShapes.Count > 0)
                {
                    Debug.WriteLine("[OnKeyDown] Calling DeleteSelectedShapes()");
                    SketchBoardDataManager.DeleteSelectedShapes();
                }
                else if (SketchBoardDataManager.SelectedGeometry != null)
                {
                    Debug.WriteLine("[OnKeyDown] Calling RemoveShape() for single selection");
                    SketchBoardDataManager.RemoveShape(SketchBoardDataManager.SelectedGeometry);
                }
            }
        }

        private void SketchBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SketchBoardDataManager == null) return;
            
            var scaleFactor = Lan.Shapes.Scaling.ViewportScalingService.CalculateStrokeThicknessFromViewportSize(ActualWidth, ActualHeight);
            var stylers = SketchBoardDataManager.CurrentShapeLayer?.Stylers;

            if (stylers != null)
            {
                foreach (var shapeStyler in stylers)
                {
                    shapeStyler.Value.SketchPen.Thickness = 2 * scaleFactor;
                    shapeStyler.Value.DragHandleSize = 10 * scaleFactor;
                }
            }
        }

        #region others

        private static void OnSketchBoardDataManagerChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is SketchBoard sketchBoard && e.NewValue is ISketchBoardDataManager dataManager)
            {
                var oldShapes = dataManager.Shapes;

                dataManager.InitializeVisualCollection(sketchBoard);
                if (oldShapes != null)
                {
                    foreach (var shape in oldShapes)
                    {
                        dataManager.AddShape(shape);
                    }
                }
            }
        }

        #endregion


        #region overrides

        protected override int VisualChildrenCount
        {
            get => SketchBoardDataManager?.VisualCollection.Count ?? 0;
        }

        protected override Visual GetVisualChild(int index)
        {
            return SketchBoardDataManager?.VisualCollection[index] ?? throw new InvalidOperationException();
        }

        #endregion


        #region events handling

        /// <summary>
        /// Right click the mouse means ending the drawing of current shape
        /// </summary>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            SketchBoardDataManager?.CurrentGeometryInEdit?.OnMouseRightButtonUp(e.GetPosition(this));
            SketchBoardDataManager?.UnselectGeometry();

            base.OnMouseRightButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                Focus();
                if (SketchBoardDataManager == null) return;

                var mousePosition = e.GetPosition(this);
                _mouseDownPoint = mousePosition;
                _lastMousePosition = mousePosition;

                // Find the shape clicked by mouse
                var hitShape = GetHitTestShape(mousePosition);
                
                Debug.WriteLine($"[OnMouseLeftButtonDown] hitShape={hitShape?.GetType().Name ?? "null"}, SelectedShapes.Count={SketchBoardDataManager.SelectedShapes.Count}");

                // IMPORTANT: Check multi-selection move FIRST, before any other logic
                // This prevents the selection from being cleared when clicking on a selected shape
                if (hitShape != null && SketchBoardDataManager.SelectedShapes.Count > 0 
                    && SketchBoardDataManager.SelectedShapes.Contains(hitShape))
                {
                    Debug.WriteLine("[OnMouseLeftButtonDown] Starting multi-shape move");
                    _isMovingMultipleShapes = true;
                    return;
                }

                // Check if we should start marquee selection mode BEFORE clearing selection
                // (no shape hit, no shape being drawn, and no geometry type selected)
                var hasGeometryType = SketchBoardDataManager.CurrentGeometryInEdit != null;
                
                if (!hasGeometryType && hitShape == null)
                {
                    // Try to create a new geometry - if it returns null, no geometry type is selected
                    var newGeometry = SketchBoardDataManager.CreateNewGeometry(mousePosition);
                    if (newGeometry == null)
                    {
                        // No geometry type selected, start marquee selection
                        // DON'T clear selection here - let the marquee selection handle it
                        Debug.WriteLine("[OnMouseLeftButtonDown] Starting marquee selection");
                        _isMarqueeSelectionMode = true;
                        _marqueeSelection.BeginSelection(mousePosition);
                        
                        // Add selection rectangle to visual collection
                        if (!SketchBoardDataManager.VisualCollection.Contains(_marqueeSelection.SelectionRectangle))
                        {
                            SketchBoardDataManager.VisualCollection.Add(_marqueeSelection.SelectionRectangle);
                        }
                        return;
                    }
                    else
                    {
                        // New geometry was created - clear selection first
                        Debug.WriteLine("[OnMouseLeftButtonDown] New geometry created, clearing selection");
                        SketchBoardDataManager.ClearSelection();
                        SketchBoardDataManager.SelectedGeometry = newGeometry;
                        SketchBoardDataManager.SelectedGeometry.OnMouseLeftButtonDown(mousePosition);
                        return;
                    }
                }

                // If clicking on an unselected shape, clear multi-selection
                if (SketchBoardDataManager.SelectedShapes.Count > 0)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                    {
                        Debug.WriteLine("[OnMouseLeftButtonDown] Clicking on unselected shape, clearing selection");
                        SketchBoardDataManager.ClearSelection();
                    }
                }

                // Normal shape selection/creation logic
                SketchBoardDataManager.SelectedGeometry = hitShape;

                if (SketchBoardDataManager.SelectedGeometry == null)
                {
                    SketchBoardDataManager.SelectedGeometry = SketchBoardDataManager.CurrentGeometryInEdit;
                }

                if (e.ClickCount == 2)
                {
                    SketchBoardDataManager.SelectedGeometry?.OnMouseLeftButtonDoubleClick(mousePosition);
                }
                else
                {
                    SketchBoardDataManager.SelectedGeometry?.OnMouseLeftButtonDown(mousePosition);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private ShapeVisualBase? GetHitTestShape(Point mousePosition)
        {
            if ((SketchBoardDataManager?.SelectedGeometry?.IsBeingDraggedOrPanMoving ?? false)
                && !SketchBoardDataManager.SelectedGeometry.IsLocked)
            {
                return SketchBoardDataManager.SelectedGeometry;
            }

            ShapeVisualBase? shape = null;

            var hitTestResult = VisualTreeHelper.HitTest(this, mousePosition);

            if (hitTestResult != null) shape = hitTestResult.VisualHit as ShapeVisualBase;

            return shape?.IsLocked ?? true ? null : shape;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var mousePosition = e.GetPosition(this);

                    // Handle marquee selection mode
                    if (_isMarqueeSelectionMode)
                    {
                        _marqueeSelection.UpdateSelection(mousePosition);
                        return;
                    }

                    // Handle moving multiple selected shapes
                    if (_isMovingMultipleShapes && SketchBoardDataManager != null)
                    {
                        var offset = mousePosition - _lastMousePosition;
                        SketchBoardDataManager.MoveSelectedShapes(offset);
                        _lastMousePosition = mousePosition;
                        return;
                    }

                    if (SketchBoardDataManager?.CurrentGeometryInEdit != null)
                    {
                        SketchBoardDataManager.CurrentGeometryInEdit.OnMouseMove(mousePosition, e.LeftButton);
                    }
                    else
                    {
                        SketchBoardDataManager?.SelectedGeometry?.OnMouseMove(mousePosition, e.LeftButton);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            try
            {
                if (SketchBoardDataManager == null) return;

                var position = e.GetPosition(this);

                // Complete marquee selection if active
                if (_isMarqueeSelectionMode)
                {
                    var addToExisting = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                    
                    // Get the selection bounds in screen coordinates
                    var screenBounds = _marqueeSelection.SelectionRectangle.SelectionBounds;
                    
                    Debug.WriteLine($"[OnMouseLeftButtonUp] Screen bounds: {screenBounds}");
                    
                    // Find shapes using visual hit testing (works in screen coordinates)
                    var isCrossing = _marqueeSelection.SelectionRectangle.IsCrossingSelection;
                    var selectedShapes = FindShapesInBounds(screenBounds, isCrossing);
                    
                    // End the selection rectangle visual
                    _marqueeSelection.SelectionRectangle.IsActive = false;
                    
                    // Apply selection to data manager
                    SketchBoardDataManager.SelectShapes(selectedShapes, addToExisting);
                    
                    _isMarqueeSelectionMode = false;
                    return;
                }

                // End multi-shape move operation
                if (_isMovingMultipleShapes)
                {
                    _isMovingMultipleShapes = false;
                    return;
                }

                var geometry = SketchBoardDataManager.SelectedGeometry;
                if (geometry == null) return;

                if (!geometry.IsGeometryRendered)
                {
                    SketchBoardDataManager.NewShapeSketched?.Invoke(geometry);
                }

                geometry.OnMouseLeftButtonUp(position);

                if (geometry.IsGeometryRendered)
                {
                    SketchBoardDataManager.UnselectGeometry();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
