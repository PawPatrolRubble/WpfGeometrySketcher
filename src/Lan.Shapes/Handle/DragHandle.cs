using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    public class DragHandle : DrawingVisual
    {
        public int Id { get; }
        public DragHandle(Size handleSize, Point location, double detectionRange, int id)
        {
            HandleSize = handleSize;
            Location = location;
            DetectionRange = detectionRange;
            Id = id;
            HandleGeometry = new EllipseGeometry(Location, HandleSize.Width, HandleSize.Height);
        }

        public double DetectionRange { get; }
        public Size HandleSize { get; }
        public Point Location { get; }

        public virtual Geometry HandleGeometry { get; }
    }
}