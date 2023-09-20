using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    public class CircleDragHandle : DragHandle
    {
        private readonly EllipseGeometry _ellipseGeometry;
        public CircleDragHandle(double size, Point geometryCenter, int id) : this(new Size(size, size), geometryCenter, 1, id)
        {
            _ellipseGeometry = new EllipseGeometry(geometryCenter, size, size);
        }

        public CircleDragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id) : base(handleSize,
            geometryCenter,
            detectionRange, id)
        {
        }

        public override Geometry HandleGeometry { get => _ellipseGeometry; }

        protected override void SetCenter(Point center)
        {
            _ellipseGeometry.Center = center;
        }
    }
}