#nullable enable
using System.Collections.Generic;

namespace Lan.Shapes.Commands
{
    /// <summary>
    /// Manages command history for undo/redo operations.
    /// Implements the Command Pattern with history tracking.
    /// </summary>
    public class CommandHistory
    {
        private readonly Stack<IShapeCommand> _undoStack = new();
        private readonly Stack<IShapeCommand> _redoStack = new();
        private readonly int _maxHistorySize;

        /// <summary>
        /// Creates a new command history with optional max size.
        /// </summary>
        /// <param name="maxHistorySize">Maximum number of commands to keep (0 = unlimited)</param>
        public CommandHistory(int maxHistorySize = 100)
        {
            _maxHistorySize = maxHistorySize;
        }

        /// <summary>
        /// Whether there are commands that can be undone
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Whether there are commands that can be redone
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Number of commands in undo history
        /// </summary>
        public int UndoCount => _undoStack.Count;

        /// <summary>
        /// Number of commands in redo history
        /// </summary>
        public int RedoCount => _redoStack.Count;

        /// <summary>
        /// Execute a command and add it to history
        /// </summary>
        public void ExecuteCommand(IShapeCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Clear redo stack when new command is executed

            // Trim history if needed
            TrimHistory();
        }

        /// <summary>
        /// Undo the last command
        /// </summary>
        /// <returns>True if a command was undone</returns>
        public bool Undo()
        {
            if (!CanUndo) return false;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            return true;
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        /// <returns>True if a command was redone</returns>
        public bool Redo()
        {
            if (!CanRedo) return false;

            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            return true;
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        /// <summary>
        /// Get description of the next command to undo
        /// </summary>
        public string? GetUndoDescription()
        {
            return CanUndo ? _undoStack.Peek().Description : null;
        }

        /// <summary>
        /// Get description of the next command to redo
        /// </summary>
        public string? GetRedoDescription()
        {
            return CanRedo ? _redoStack.Peek().Description : null;
        }

        private void TrimHistory()
        {
            if (_maxHistorySize <= 0) return;

            // Convert to array, trim, and rebuild stack if needed
            if (_undoStack.Count > _maxHistorySize)
            {
                var items = _undoStack.ToArray();
                _undoStack.Clear();
                for (int i = _maxHistorySize - 1; i >= 0; i--)
                {
                    _undoStack.Push(items[i]);
                }
            }
        }
    }
}
