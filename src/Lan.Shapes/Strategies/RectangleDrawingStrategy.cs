#nullable enable
using System.Windows;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Drawing strategy for rectangle shapes.
    /// Draws from top-left to bottom-right corner.
    /// </summary>
    public class RectangleDrawingStrategy : IDrawingStrategy
    {
        public void OnDrawStart(ShapeVisualBase shape, Point startPoint)
        {
            if (shape is Rectangle rectangle)
            {
                rectangle.TopLeft = startPoint;
            }
        }

        public void OnDrawing(ShapeVisualBase shape, Point startPoint, Point currentPoint)
        {
            if (shape is Rectangle rectangle)
            {
                // Ensure bottom-right is always to the right and below top-left
                var minX = System.Math.Min(startPoint.X, currentPoint.X);
                var minY = System.Math.Min(startPoint.Y, currentPoint.Y);
                var maxX = System.Math.Max(startPoint.X, currentPoint.X);
                var maxY = System.Math.Max(startPoint.Y, currentPoint.Y);

                rectangle.TopLeft = new Point(minX, minY);
                rectangle.BottomRight = new Point(maxX, maxY);
            }
        }

        public void OnDrawEnd(ShapeVisualBase shape, Point endPoint)
        {
            // Drawing complete - shape is already updated
        }
    }
}
