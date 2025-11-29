#nullable enable
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Commands
{
    /// <summary>
    /// Command for deleting a shape from the collection.
    /// </summary>
    public class DeleteShapeCommand : IShapeCommand
    {
        private readonly IShapeCollection _collection;
        private readonly ShapeVisualBase _shape;
        private int _originalIndex;

        public string Description => $"Delete {_shape.GetType().Name}";

        public DeleteShapeCommand(IShapeCollection collection, ShapeVisualBase shape)
        {
            _collection = collection;
            _shape = shape;
            _originalIndex = -1;
        }

        public void Execute()
        {
            // Store the original index for undo
            _originalIndex = _collection.Shapes.IndexOf(_shape);
            _collection.RemoveShape(_shape);
        }

        public void Undo()
        {
            if (_originalIndex >= 0 && _originalIndex <= _collection.Shapes.Count)
            {
                _collection.AddShape(_shape, _originalIndex);
            }
            else
            {
                _collection.AddShape(_shape);
            }
        }
    }
}
