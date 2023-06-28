using System.Windows;

namespace Lan.Shapes.Interfaces
{
    public class EllipseData : IGeometryMetaData
    {
        public double StrokeThickness { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public Point Center { get; set; }
    }
}