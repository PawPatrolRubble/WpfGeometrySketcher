using System.Windows.Media;

namespace Lan.Shapes
{
    public class ShapeLayerParameter
    {
        public int LayerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Brush StylerFillColor { get; set; }
        public Brush StylerStrokeColor { get; set; }
        public double StylerStrokeThickness { get; set; }
        public DashStyle StylerDashStyle { get; set; }
        public Brush TextForeground { get; set; }
        public Brush BorderBackground { get; set; }

    }
}