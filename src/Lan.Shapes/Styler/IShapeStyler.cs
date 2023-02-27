using System.Windows.Media;

namespace Lan.Shapes.Styler
{
    public interface IShapeStyler
    {
        void SetStrokeColor(Brush color);
        void SetFillColor(Brush color);
        void SetStrokeThickness(double strokeThickness);
        void SetPenDashStyle(DashStyle dashStyle);

        double DragHandleSize { get; }
        Pen SketchPen { get; }
        Brush FillColor { get; }
        string Name { get; set; }
        IShapeStyler Clone();
    }
}