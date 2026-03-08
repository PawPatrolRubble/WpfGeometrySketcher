using netDxf;
using netDxf.Entities;

using System;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Shapes;
using Circle = netDxf.Entities.Circle;
using Line = netDxf.Entities.Line;
using Point = System.Windows.Point;

public static class DxfRenderer
{
    /// <summary>
    /// Converts all supported DXF entities into a single PathGeometry.
    /// </summary>
    public static PathGeometry BuildGeometry(DxfDocument doc, double scale = 1.0, Point offset = default)
    {
        var geometry = new PathGeometry();
        geometry.FillRule = FillRule.EvenOdd;

        foreach (var entity in doc.Entities.All)
        {
            switch (entity)
            {
                case Line line:
                    geometry.Figures.Add(DrawLine(line, scale, offset));
                    break;
                case Arc arc:
                    geometry.Figures.Add(DrawArc(arc, scale, offset));
                    break;
                case Circle circle:
                    geometry.Figures.Add(DrawCircle(circle, scale, offset));
                    break;
                case Polyline2D poly:
                    geometry.Figures.Add(DrawLwPolyline(poly, scale, offset));
                    break;
                case Polyline3D poly3d:
                    geometry.Figures.Add(DrawPolyline3D(poly3d, scale, offset));
                    break;
                case Spline spline:
                    var splineFigure = DrawSpline(spline, scale, offset);
                    if (splineFigure != null)
                        geometry.Figures.Add(splineFigure);
                    break;
            }
        }

        geometry.Freeze(); // Freeze for performance
        return geometry;
    }


    static PathFigure DrawLwPolylineWithBulge(Polyline2D poly, double scale, Point offset)
    {
        var verts = poly.Vertexes;
        if (verts.Count == 0) return null;

        var figure = new PathFigure
        {
            StartPoint = ToPoint(verts[0].Position, scale, offset),
            IsClosed = poly.IsClosed,
            IsFilled = false
        };

        for (int i = 0; i < verts.Count - 1; i++)
        {
            var v0 = verts[i];
            var v1 = verts[i + 1];

            if (Math.Abs(v0.Bulge) < 1e-10)
            {
                figure.Segments.Add(new LineSegment(ToPoint(v1.Position, scale, offset), true));
            }
            else
            {
                // Convert bulge to arc parameters
                double bulge = v0.Bulge;
                double angle = 4 * Math.Atan(Math.Abs(bulge)); // included angle
                double d = Math.Sqrt(Math.Pow(v1.Position.X - v0.Position.X, 2) +
                                     Math.Pow(v1.Position.Y - v0.Position.Y, 2));
                double r = d / (2 * Math.Sin(angle / 2)) * scale;

                bool largeArc = angle > Math.PI;
                var sweep = bulge > 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

                figure.Segments.Add(new ArcSegment(ToPoint(v1.Position, scale, offset), new Size(r, r),
                    0, largeArc, sweep, true));
            }
        }
        return figure;
    }


    // --- Helpers ---

    static Point ToPoint(netDxf.Vector2 v, double scale, Point offset) =>
        new Point(v.X * scale + offset.X, -v.Y * scale + offset.Y); // Flip Y for screen coords

    static Point ToPoint(netDxf.Vector3 v, double scale, Point offset) =>
        new Point(v.X * scale + offset.X, -v.Y * scale + offset.Y);

    static PathFigure DrawLine(Line line, double scale, Point offset)
    {
        var figure = new PathFigure
        {
            StartPoint = ToPoint(line.StartPoint, scale, offset),
            IsClosed = false,
            IsFilled = false
        };
        figure.Segments.Add(new LineSegment(ToPoint(line.EndPoint, scale, offset), true));
        return figure;
    }

    static PathFigure DrawArc(Arc arc, double scale, Point offset)
    {
        double startRad = arc.StartAngle * Math.PI / 180.0;
        double endRad = arc.EndAngle * Math.PI / 180.0;
        double r = arc.Radius * scale;

        Point center = ToPoint(arc.Center, scale, offset);

        // WPF Y-axis is flipped, so negate angles
        Point startPt = new Point(
            center.X + r * Math.Cos(startRad),
            center.Y - r * Math.Sin(startRad));

        Point endPt = new Point(
            center.X + r * Math.Cos(endRad),
            center.Y - r * Math.Sin(endRad));

        double sweep = arc.EndAngle - arc.StartAngle;
        if (sweep < 0) sweep += 360;
        bool isLargeArc = sweep > 180;

        var figure = new PathFigure
        {
            StartPoint = startPt,
            IsClosed = false,
            IsFilled = false
        };
        figure.Segments.Add(new ArcSegment(
            endPt,
            new Size(r, r),
            0,
            isLargeArc,
            SweepDirection.Counterclockwise,
            true));
        return figure;
    }

    static PathFigure DrawCircle(Circle circle, double scale, Point offset)
    {
        Point center = ToPoint(circle.Center, scale, offset);
        double r = circle.Radius * scale;

        // Draw as two arcs (full ellipse requires two segments)
        var figure = new PathFigure
        {
            StartPoint = new Point(center.X + r, center.Y),
            IsClosed = true,
            IsFilled = false
        };
        figure.Segments.Add(new ArcSegment(new Point(center.X - r, center.Y), new Size(r, r), 0, false, SweepDirection.Clockwise, true));
        figure.Segments.Add(new ArcSegment(new Point(center.X + r, center.Y), new Size(r, r), 0, false, SweepDirection.Clockwise, true));
        return figure;
    }

    static PathFigure DrawLwPolyline(Polyline2D poly, double scale, Point offset)
    {
        var verts = poly.Vertexes;
        if (verts.Count == 0) return null;

        var figure = new PathFigure
        {
            StartPoint = ToPoint(verts[0].Position, scale, offset),
            IsClosed = poly.IsClosed,
            IsFilled = false
        };
        for (int i = 1; i < verts.Count; i++)
            figure.Segments.Add(new LineSegment(ToPoint(verts[i].Position, scale, offset), true));
        return figure;
    }

    static PathFigure DrawPolyline3D(Polyline3D poly, double scale, Point offset)
    {
        var verts = poly.Vertexes;
        if (verts.Count == 0) return null;

        var figure = new PathFigure
        {
            StartPoint = ToPoint(verts[0], scale, offset),
            IsClosed = poly.IsClosed,
            IsFilled = false
        };
        for (int i = 1; i < verts.Count; i++)
            figure.Segments.Add(new LineSegment(ToPoint(verts[i], scale, offset), true));
        return figure;
    }

    static PathFigure DrawSpline(Spline spline, double scale, Point offset)
    {
        // Approximate spline via control points as a polyBezier
        var pts = spline.ControlPoints;
        if (pts.Length < 2) return null;

        var figure = new PathFigure
        {
            StartPoint = ToPoint(pts[0], scale, offset),
            IsClosed = false,
            IsFilled = false
        };

        // Group into bezier segments (degree 3)
        for (int i = 1; i + 2 < pts.Length; i += 3)
        {
            figure.Segments.Add(new BezierSegment(
                ToPoint(pts[i], scale, offset),
                ToPoint(pts[i + 1], scale, offset),
                ToPoint(pts[i + 2], scale, offset),
                true));
        }
        return figure;
    }
}