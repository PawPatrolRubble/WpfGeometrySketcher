#nullable enable

namespace Lan.Shapes.Commands
{
    /// <summary>
    /// Command Pattern: Interface for shape operations that can be undone/redone.
    /// Enables undo/redo functionality for shape manipulations.
    /// </summary>
    public interface IShapeCommand
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute();

        /// <summary>
        /// Undo the command (reverse the operation)
        /// </summary>
        void Undo();

        /// <summary>
        /// Description of the command for UI display
        /// </summary>
        string Description { get; }
    }
}
