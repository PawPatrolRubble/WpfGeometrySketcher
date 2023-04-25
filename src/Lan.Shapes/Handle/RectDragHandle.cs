using System;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    public class RectDragHandle : DragHandle
    {
        private readonly RectangleGeometry _rectangleGeometry = new RectangleGeometry();

        public RectDragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id) :
            base(handleSize, geometryCenter, detectionRange, id)
        {
            GeometryCenter = geometryCenter;
        }

        public RectDragHandle(double widthAndHeight, Point center, int id) : this(new Size(widthAndHeight, widthAndHeight), center, 10, id)
        {

        }


        public override Geometry HandleGeometry
        {
            get => _rectangleGeometry;

        }

        protected override void SetCenter(Point center)
        {
            _rectangleGeometry.Rect = new Rect(center - new Vector(HandleSize.Width / 2, HandleSize.Height / 2), HandleSize);
        }
    }
}