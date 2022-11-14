using System.Windows;

namespace Lan.Shapes.Handle
{
    public class CircleDragHandle : DragHandle
    {
        public CircleDragHandle(double size, Point location, int id) : this(new Size(size, size), location, 15, id)
        {
        }

        public CircleDragHandle(Size handleSize, Point location, double detectionRange, int id) : base(handleSize,
            location,
            detectionRange, id)
        {
        }
    }
}