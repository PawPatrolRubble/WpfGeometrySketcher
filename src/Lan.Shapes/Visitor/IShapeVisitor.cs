#nullable enable
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Visitor
{
    /// <summary>
    /// Visitor Pattern: Interface for operations that can be performed on shapes.
    /// Allows adding new operations without modifying shape classes.
    /// </summary>
    public interface IShapeVisitor
    {
        /// <summary>
        /// Visit a Rectangle shape
        /// </summary>
        void Visit(Rectangle rectangle);

        /// <summary>
        /// Visit a Circle shape
        /// </summary>
        void Visit(Circle circle);

        /// <summary>
        /// Visit an Ellipse shape
        /// </summary>
        void Visit(Ellipse ellipse);

        /// <summary>
        /// Visit a Line shape
        /// </summary>
        void Visit(Line line);

        /// <summary>
        /// Visit a Cross shape
        /// </summary>
        void Visit(Cross cross);

        /// <summary>
        /// Visit any other shape (fallback)
        /// </summary>
        void VisitDefault(ShapeVisualBase shape);
    }

    /// <summary>
    /// Generic visitor interface for operations that return a value.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the visitor</typeparam>
    public interface IShapeVisitor<TResult>
    {
        TResult Visit(Rectangle rectangle);
        TResult Visit(Circle circle);
        TResult Visit(Ellipse ellipse);
        TResult Visit(Line line);
        TResult Visit(Cross cross);
        TResult VisitDefault(ShapeVisualBase shape);
    }
}
