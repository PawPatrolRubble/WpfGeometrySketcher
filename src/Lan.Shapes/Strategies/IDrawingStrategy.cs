#nullable enable
using System.Windows;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Strategy Pattern: Interface for shape drawing behavior.
    /// Allows different drawing behaviors to be swapped at runtime.
    /// </summary>
    public interface IDrawingStrategy
    {
        /// <summary>
        /// Handle the initial mouse down when starting to draw
        /// </summary>
        /// <param name="shape">The shape being drawn</param>
        /// <param name="startPoint">The starting point</param>
        void OnDrawStart(ShapeVisualBase shape, Point startPoint);

        /// <summary>
        /// Handle mouse movement during drawing
        /// </summary>
        /// <param name="shape">The shape being drawn</param>
        /// <param name="startPoint">The original starting point</param>
        /// <param name="currentPoint">The current mouse position</param>
        void OnDrawing(ShapeVisualBase shape, Point startPoint, Point currentPoint);

        /// <summary>
        /// Handle mouse up to complete drawing
        /// </summary>
        /// <param name="shape">The shape being drawn</param>
        /// <param name="endPoint">The ending point</param>
        void OnDrawEnd(ShapeVisualBase shape, Point endPoint);
    }
}
