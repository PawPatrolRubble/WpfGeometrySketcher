using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    public class CircleDragHandle : DragHandle
    {
        private readonly EllipseGeometry _ellipseGeometry = new EllipseGeometry();

        public CircleDragHandle(double size, Point geometryCenter, int id) : this(new Size(size, size), geometryCenter, 1, id)
        {
        }

        public CircleDragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id) : base(handleSize,
            geometryCenter,
            detectionRange, id)
        {
            // Base constructor already sets GeometryCenter which triggers SetCenter
        }

        public override Geometry HandleGeometry => _ellipseGeometry;

        protected override void SetCenter(Point center)
        {
            _ellipseGeometry.Center = center;
            _ellipseGeometry.RadiusX = HandleSize.Width / 2;
            _ellipseGeometry.RadiusY = HandleSize.Height / 2;
        }

        public override bool FillContains(Point checkPoint)
        {
            // Calculate distance from center for hit testing with DetectionRange
            var dx = checkPoint.X - _ellipseGeometry.Center.X;
            var dy = checkPoint.Y - _ellipseGeometry.Center.Y;
            var radiusX = _ellipseGeometry.RadiusX + DetectionRange;
            var radiusY = _ellipseGeometry.RadiusY + DetectionRange;

            // Check if point is within the inflated ellipse
            return (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY) <= 1.0;
        }
    }
}