#nullable enable
using System.Collections.Generic;
using System.Windows;

namespace Lan.Shapes.Interfaces
{
    /// <summary>
    /// used to export critical position data
    /// </summary>
    public interface IDataExport<out T> where T : IGeometryMetaData
    {
        T GetMetaData();
    }

    /// <summary>
    /// it is used to exchange data with 
    /// </summary>
    public interface IGeometryMetaData
    {
        double StrokeThickness { get; }

    }


    public class EllipseData : IGeometryMetaData
    {
        public double StrokeThickness { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public Point Center { get; set; }
    }

    public class PointsData : IGeometryMetaData
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public PointsData(double strokeThickness, List<Point> dataPoints)
        {
            StrokeThickness = strokeThickness;
            DataPoints = dataPoints;
        }

        public double StrokeThickness { get; set; }
        public List<Point> DataPoints { get; set; }
    }

}