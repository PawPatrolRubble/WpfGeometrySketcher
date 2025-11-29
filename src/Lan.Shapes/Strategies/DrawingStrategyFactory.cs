#nullable enable
using System;
using System.Collections.Generic;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Strategies
{
    /// <summary>
    /// Factory for creating drawing strategies based on shape type.
    /// </summary>
    public static class DrawingStrategyFactory
    {
        private static readonly Dictionary<Type, IDrawingStrategy> _strategies = new()
        {
            { typeof(Rectangle), new RectangleDrawingStrategy() },
            { typeof(Circle), new CircleDrawingStrategy() },
            { typeof(Ellipse), new EllipseDrawingStrategy() },
            { typeof(Line), new LineDrawingStrategy() }
        };

        /// <summary>
        /// Get the drawing strategy for a shape type.
        /// </summary>
        /// <typeparam name="TShape">The shape type</typeparam>
        /// <returns>The drawing strategy, or null if not found</returns>
        public static IDrawingStrategy? GetStrategy<TShape>() where TShape : ShapeVisualBase
        {
            return GetStrategy(typeof(TShape));
        }

        /// <summary>
        /// Get the drawing strategy for a shape type.
        /// </summary>
        /// <param name="shapeType">The shape type</param>
        /// <returns>The drawing strategy, or null if not found</returns>
        public static IDrawingStrategy? GetStrategy(Type shapeType)
        {
            return _strategies.TryGetValue(shapeType, out var strategy) ? strategy : null;
        }

        /// <summary>
        /// Get the drawing strategy for a shape instance.
        /// </summary>
        /// <param name="shape">The shape instance</param>
        /// <returns>The drawing strategy, or null if not found</returns>
        public static IDrawingStrategy? GetStrategy(ShapeVisualBase shape)
        {
            return GetStrategy(shape.GetType());
        }

        /// <summary>
        /// Register a custom drawing strategy for a shape type.
        /// </summary>
        /// <typeparam name="TShape">The shape type</typeparam>
        /// <param name="strategy">The drawing strategy</param>
        public static void RegisterStrategy<TShape>(IDrawingStrategy strategy) where TShape : ShapeVisualBase
        {
            _strategies[typeof(TShape)] = strategy;
        }
    }
}
