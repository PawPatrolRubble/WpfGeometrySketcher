#nullable enable
using System.Windows.Input;

namespace Lan.Shapes.Interfaces
{
    public interface ISketchBoardMouseHandler
    {
        // Basic mouse events
        public event MouseButtonEventHandler MouseLeftButtonUp;
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event System.Windows.Input.MouseEventHandler MouseMove;
    }
}