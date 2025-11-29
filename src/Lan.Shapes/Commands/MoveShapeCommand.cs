#nullable enable
using System.Windows;

namespace Lan.Shapes.Commands
{
    /// <summary>
    /// Command for moving a shape by a specified offset.
    /// </summary>
    public class MoveShapeCommand : IShapeCommand
    {
        private readonly ShapeVisualBase _shape;
        private readonly Vector _offset;
        private readonly Point _originalPosition;

        public string Description => $"Move shape by ({_offset.X:F1}, {_offset.Y:F1})";

        public MoveShapeCommand(ShapeVisualBase shape, Vector offset)
        {
            _shape = shape;
            _offset = offset;
            _originalPosition = shape.BoundsRect.Location;
        }

        public void Execute()
        {
            // The move has already been applied during drag
            // This is used for redo after undo
            ApplyOffset(_offset);
        }

        public void Undo()
        {
            ApplyOffset(-_offset);
        }

        private void ApplyOffset(Vector offset)
        {
            // Note: Actual implementation depends on shape type
            // This is a placeholder - shapes should implement IMovable interface
            _shape.UpdateVisual();
        }
    }
}
