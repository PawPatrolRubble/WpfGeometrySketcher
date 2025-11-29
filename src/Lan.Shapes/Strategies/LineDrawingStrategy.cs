#nullable enable
using System.Windows;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Drawing strategy for line shapes.
    /// Draws from start point to end point.
    /// </summary>
    public class LineDrawingStrategy : IDrawingStrategy
    {
        public void OnDrawStart(ShapeVisualBase shape, Point startPoint)
        {
            if (shape is Line line)
            {
                line.Start = startPoint;
                line.End = startPoint;
            }
        }

        public void OnDrawing(ShapeVisualBase shape, Point startPoint, Point currentPoint)
        {
            if (shape is Line line)
            {
                line.End = currentPoint;
            }
        }

        public void OnDrawEnd(ShapeVisualBase shape, Point endPoint)
        {
            // Drawing complete - shape is already updated
        }
    }
}
