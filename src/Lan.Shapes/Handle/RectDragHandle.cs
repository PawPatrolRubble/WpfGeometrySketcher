using System;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;

namespace Lan.Shapes.Handle
{
    public class RectDragHandle : DragHandle
    {
        private readonly RectangleGeometry _rectangleGeometry = new RectangleGeometry();

        public RectDragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id) :
            base(handleSize, geometryCenter, detectionRange, id)
        {
            // Base constructor already sets GeometryCenter which triggers SetCenter
        }

        public RectDragHandle(double widthAndHeight, Point center, int id) : this(new Size(widthAndHeight, widthAndHeight), center, 10, id)
        {

        }


        public override Geometry HandleGeometry => _rectangleGeometry;

        protected override void SetCenter(Point center)
        {
            _rectangleGeometry.Rect = new Rect(center - new Vector(HandleSize.Width / 2, HandleSize.Height / 2), HandleSize);
        }

        public override bool FillContains(Point checkPoint)
        {
            // Inflate the rectangle by DetectionRange for easier hit testing
            var hitRect = _rectangleGeometry.Rect;
            hitRect.Inflate(DetectionRange, DetectionRange);
            return hitRect.Contains(checkPoint);
        }

        public static RectDragHandle CreateRectDragHandleFromStyler(IShapeStyler shapeStyler, Point center, int id)
        {
            return new RectDragHandle(shapeStyler.DragHandleSize, center, id);
        }
        public static RectDragHandle CreateRectDragHandleFromStyler(IShapeStyler shapeStyler, Point center, DragLocation id)
        {
            return new RectDragHandle(shapeStyler.DragHandleSize, center, (int)id);
        }

    }
}