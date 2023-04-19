using System.Windows.Media;
using System.Windows.Media;
using Newtonsoft.Json;


namespace Lan.Shapes
{
    public class ShapeStylerParameter
    {
        [JsonConverter(typeof(BrushToHexConverter))]
        public Brush FillColor { get; set; }

        [JsonConverter(typeof(BrushToHexConverter))]
        public Brush StrokeColor { get; set; }

        public double StrokeThickness { get; set; }

        public string DashStyle { get; set; }

        public double DragHandleSize { get; set; }

        public double FillOpacity { get; set; }
    }
}