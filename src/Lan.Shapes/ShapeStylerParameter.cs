using System.Windows.Media;

namespace Lan.Shapes
{
    public class ShapeStylerParameter
    {
        public Brush FillColor { get; set; }
        public Brush StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public DashStyle DashStyle { get; set; }
    }
}