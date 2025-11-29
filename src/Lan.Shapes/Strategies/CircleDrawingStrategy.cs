#nullable enable
using System;
using System.Windows;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Drawing strategy for circle shapes.
    /// Draws from center with radius determined by distance to current point.
    /// </summary>
    public class CircleDrawingStrategy : IDrawingStrategy
    {
        public void OnDrawStart(ShapeVisualBase shape, Point startPoint)
        {
            if (shape is Circle circle)
            {
                circle.X = startPoint.X;
                circle.Y = startPoint.Y;
                circle.Radius = 0;
            }
        }

        public void OnDrawing(ShapeVisualBase shape, Point startPoint, Point currentPoint)
        {
            if (shape is Circle circle)
            {
                // Calculate radius as distance from center to current point
                var dx = currentPoint.X - startPoint.X;
                var dy = currentPoint.Y - startPoint.Y;
                circle.Radius = Math.Sqrt(dx * dx + dy * dy);
            }
        }

        public void OnDrawEnd(ShapeVisualBase shape, Point endPoint)
        {
            // Drawing complete - shape is already updated
        }
    }
}
