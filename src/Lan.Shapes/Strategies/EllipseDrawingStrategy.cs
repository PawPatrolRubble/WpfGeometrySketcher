#nullable enable
using System;
using System.Windows;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Drawing strategy for ellipse shapes.
    /// Draws from center with radii determined by horizontal and vertical distance.
    /// </summary>
    public class EllipseDrawingStrategy : IDrawingStrategy
    {
        public void OnDrawStart(ShapeVisualBase shape, Point startPoint)
        {
            if (shape is Ellipse ellipse)
            {
                ellipse.Center = startPoint;
                ellipse.RadiusX = 0;
                ellipse.RadiusY = 0;
            }
        }

        public void OnDrawing(ShapeVisualBase shape, Point startPoint, Point currentPoint)
        {
            if (shape is Ellipse ellipse)
            {
                ellipse.RadiusX = Math.Abs(currentPoint.X - startPoint.X) / 2;
                ellipse.RadiusY = Math.Abs(currentPoint.Y - startPoint.Y) / 2;
            }
        }

        public void OnDrawEnd(ShapeVisualBase shape, Point endPoint)
        {
            // Drawing complete - shape is already updated
        }
    }
}
