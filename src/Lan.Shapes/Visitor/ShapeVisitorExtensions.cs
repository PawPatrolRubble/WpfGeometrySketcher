#nullable enable
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Visitor
{
    /// <summary>
    /// Extension methods for applying visitors to shapes.
    /// </summary>
    public static class ShapeVisitorExtensions
    {
        /// <summary>
        /// Accept a visitor on this shape (double dispatch).
        /// </summary>
        public static void Accept(this ShapeVisualBase shape, IShapeVisitor visitor)
        {
            switch (shape)
            {
                case Rectangle rectangle:
                    visitor.Visit(rectangle);
                    break;
                case Circle circle:
                    visitor.Visit(circle);
                    break;
                case Ellipse ellipse:
                    visitor.Visit(ellipse);
                    break;
                case Line line:
                    visitor.Visit(line);
                    break;
                case Cross cross:
                    visitor.Visit(cross);
                    break;
                default:
                    visitor.VisitDefault(shape);
                    break;
            }
        }

        /// <summary>
        /// Accept a visitor that returns a result.
        /// </summary>
        public static TResult Accept<TResult>(this ShapeVisualBase shape, IShapeVisitor<TResult> visitor)
        {
            return shape switch
            {
                Rectangle rectangle => visitor.Visit(rectangle),
                Circle circle => visitor.Visit(circle),
                Ellipse ellipse => visitor.Visit(ellipse),
                Line line => visitor.Visit(line),
                Cross cross => visitor.Visit(cross),
                _ => visitor.VisitDefault(shape)
            };
        }
    }
}
