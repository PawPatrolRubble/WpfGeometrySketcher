using System.Windows.Media;

namespace Lan.Shapes.Styler
{
    public class ShapeStyler : IShapeStyler
    {
        private Pen _sketchPen = new Pen();
        public Pen SketchPen => _sketchPen;
        public Brush FillColor { get; set; }
        public string Name { get; set; }

        public IShapeStyler Clone()
        {
            return new ShapeStyler(FillColor, _sketchPen.Brush, _sketchPen.DashStyle);
        }

        public void SetFillColor(Brush color)
        {
            FillColor = color;
        }

        public void SetStrokeColor(Brush color)
        {
            _sketchPen.Brush = color;
        }

        public void SetStrokeThickness(double thickness)
        {
            _sketchPen.Thickness = thickness;
        }

        public void SetPenDashStyle(DashStyle dashStyle)
        {
            _sketchPen.DashStyle = dashStyle;
        }

        public ShapeStyler()
        {
        }

        public ShapeStyler(Brush fillColor, Brush strokeColor, DashStyle dashStyle)
        {
            this.FillColor = fillColor;
            _sketchPen.Thickness = 1;
            _sketchPen.Brush = strokeColor;
            _sketchPen.DashStyle = dashStyle;
        }
    }
}