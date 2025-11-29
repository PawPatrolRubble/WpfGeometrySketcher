#nullable enable
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Selection
{
    /// <summary>
    /// A visual element that displays a selection rectangle (marquee) during drag selection.
    /// Similar to AutoCAD's window/crossing selection.
    /// </summary>
    public class SelectionRectangle : DrawingVisual
    {
        private Point _startPoint;
        private Point _endPoint;
        private bool _isActive;

        /// <summary>
        /// The starting point of the selection rectangle
        /// </summary>
        public Point StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;
                UpdateVisual();
            }
        }

        /// <summary>
        /// The current end point of the selection rectangle
        /// </summary>
        public Point EndPoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                UpdateVisual();
            }
        }

        /// <summary>
        /// Whether the selection rectangle is currently being drawn
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                if (!_isActive)
                {
                    Clear();
                }
            }
        }

        /// <summary>
        /// Gets the current selection bounds as a Rect
        /// </summary>
        public Rect SelectionBounds => new Rect(_startPoint, _endPoint);

        /// <summary>
        /// Determines if this is a "crossing" selection (right-to-left drag in AutoCAD style).
        /// Crossing selection selects shapes that intersect the rectangle.
        /// Window selection (left-to-right) selects only shapes fully contained.
        /// </summary>
        public bool IsCrossingSelection => _endPoint.X < _startPoint.X;

        /// <summary>
        /// Pen for window selection (left-to-right, solid line)
        /// </summary>
        public Pen WindowSelectionPen { get; set; } = new Pen(Brushes.DodgerBlue, 1)
        {
            DashStyle = DashStyles.Solid
        };

        /// <summary>
        /// Pen for crossing selection (right-to-left, dashed line)
        /// </summary>
        public Pen CrossingSelectionPen { get; set; } = new Pen(Brushes.LimeGreen, 1)
        {
            DashStyle = DashStyles.Dash
        };

        /// <summary>
        /// Fill brush for window selection
        /// </summary>
        public Brush WindowSelectionFill { get; set; } = new SolidColorBrush(Color.FromArgb(30, 30, 144, 255));

        /// <summary>
        /// Fill brush for crossing selection
        /// </summary>
        public Brush CrossingSelectionFill { get; set; } = new SolidColorBrush(Color.FromArgb(30, 50, 205, 50));

        /// <summary>
        /// Begin a new selection rectangle
        /// </summary>
        public void Begin(Point startPoint)
        {
            _startPoint = startPoint;
            _endPoint = startPoint;
            _isActive = true;
            UpdateVisual();
        }

        /// <summary>
        /// Update the selection rectangle during drag
        /// </summary>
        public void Update(Point currentPoint)
        {
            if (!_isActive) return;
            _endPoint = currentPoint;
            UpdateVisual();
        }

        /// <summary>
        /// End the selection and return the final bounds
        /// </summary>
        public Rect End()
        {
            var bounds = SelectionBounds;
            _isActive = false;
            Clear();
            return bounds;
        }

        /// <summary>
        /// Cancel the selection
        /// </summary>
        public void Cancel()
        {
            _isActive = false;
            Clear();
        }

        private void UpdateVisual()
        {
            using var dc = RenderOpen();
            
            if (!_isActive) return;

            var rect = new Rect(_startPoint, _endPoint);
            
            // Use different styles for window vs crossing selection (AutoCAD style)
            var pen = IsCrossingSelection ? CrossingSelectionPen : WindowSelectionPen;
            var fill = IsCrossingSelection ? CrossingSelectionFill : WindowSelectionFill;

            dc.DrawRectangle(fill, pen, rect);
        }

        private void Clear()
        {
            using var dc = RenderOpen();
            // Drawing nothing clears the visual
        }
    }
}
