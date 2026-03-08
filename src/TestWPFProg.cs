using System;
using System.Windows;
using System.Windows.Media;

class Program {
    [STAThread]
    static void Main() {
        var geo = new PathGeometry();
        var fig = new PathFigure { StartPoint = new Point(2356, 2372) };
        fig.Segments.Add(new ArcSegment(new Point(2556, 2372), new Size(100,100), 0, false, SweepDirection.Clockwise, true));
        geo.Figures.Add(fig);
        var flat = geo.GetFlattenedPathGeometry(0.01, ToleranceType.Absolute);
        Console.WriteLine("Flat figures: " + flat.Figures.Count);
        var flat2 = geo.GetFlattenedPathGeometry();
        Console.WriteLine("Flat2 figures: " + flat2.Figures.Count);
    }
}
