using System;
using System.Windows;
using System.Windows.Media;

class Program
{
    [STAThread]
    static void Main()
    {
        var geo = new PathGeometry();
        var fig = new PathFigure { StartPoint = new Point(0, 0), IsFilled = true };
        fig.Segments.Add(new LineSegment(new Point(100, 100), true));
        geo.Figures.Add(fig);
        var flat = geo.GetFlattenedPathGeometry();
        Console.WriteLine("Figures count with IsFilled=true: " + flat.Figures.Count);
    }
}
