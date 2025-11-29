#nullable enable
using System.Windows;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Visitor
{
    /// <summary>
    /// Visitor that calculates the combined bounds of all visited shapes.
    /// Example implementation of the Visitor pattern.
    /// </summary>
    public class ShapeBoundsVisitor : IShapeVisitor
    {
        private Rect _combinedBounds = Rect.Empty;

        /// <summary>
        /// The combined bounding rectangle of all visited shapes
        /// </summary>
        public Rect CombinedBounds => _combinedBounds;

        public void Visit(Rectangle rectangle)
        {
            UnionBounds(rectangle.BoundsRect);
        }

        public void Visit(Circle circle)
        {
            UnionBounds(circle.BoundsRect);
        }

        public void Visit(Ellipse ellipse)
        {
            UnionBounds(ellipse.BoundsRect);
        }

        public void Visit(Line line)
        {
            UnionBounds(line.BoundsRect);
        }

        public void Visit(Cross cross)
        {
            UnionBounds(cross.BoundsRect);
        }

        public void VisitDefault(ShapeVisualBase shape)
        {
            UnionBounds(shape.BoundsRect);
        }

        private void UnionBounds(Rect bounds)
        {
            if (_combinedBounds.IsEmpty)
            {
                _combinedBounds = bounds;
            }
            else
            {
                _combinedBounds.Union(bounds);
            }
        }

        /// <summary>
        /// Reset the combined bounds
        /// </summary>
        public void Reset()
        {
            _combinedBounds = Rect.Empty;
        }
    }
}
