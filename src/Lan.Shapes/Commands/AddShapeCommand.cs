#nullable enable
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Commands
{
    /// <summary>
    /// Command for adding a shape to the collection.
    /// </summary>
    public class AddShapeCommand : IShapeCommand
    {
        private readonly IShapeCollection _collection;
        private readonly ShapeVisualBase _shape;

        public string Description => $"Add {_shape.GetType().Name}";

        public AddShapeCommand(IShapeCollection collection, ShapeVisualBase shape)
        {
            _collection = collection;
            _shape = shape;
        }

        public void Execute()
        {
            _collection.AddShape(_shape);
        }

        public void Undo()
        {
            _collection.RemoveShape(_shape);
        }
    }
}
