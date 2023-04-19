using System.Windows.Media;

namespace Lan.Shapes.Styler
{
    public interface IShapeStyler
    {
        string Name { get; set; }
        void SetStrokeColor(Brush color);
        void SetFillColor(Brush color);
        void SetStrokeThickness(double strokeThickness);
        void SetPenDashStyle(DashStyle dashStyle);

        double DragHandleSize { get; }
        Pen SketchPen { get; }
        Brush FillColor { get; }
        IShapeStyler Clone();
        ShapeStylerParameter ToStylerParameter();
    }
    
}