using System.Collections.Generic;
using System.Windows.Media;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    public class ShapeLayerParameter
    {
        public Dictionary<ShapeState, ShapeStylerParameter> StyleSchema { get; set; }
        public int LayerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Brush TextForeground { get; set; }
        public Brush BorderBackground { get; set; }
    }
}