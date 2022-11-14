using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    public class DragHandle:FrameworkElement
    {
        public int Id { get; }
        public DragHandle(Size handleSize, Point location, double detectionRange, int id)
        {
            HandleSize = handleSize;
            Location = location;
            DetectionRange = detectionRange;
            Id = id;
        }



        public  double DetectionRange { get; }
        public Size HandleSize { get; }
        public Point Location { get; }
        
        public virtual Geometry HandleGeometry => new EllipseGeometry(Location, HandleSize.Width,HandleSize.Height);
        
        public bool HitTest(Point p)
        {
            return HandleGeometry.FillContains(p, DetectionRange, ToleranceType.Relative);
        }
    }
}