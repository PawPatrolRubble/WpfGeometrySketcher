using System.Collections.Generic;
using System.Windows;
using Lan.Shapes.Enums;

namespace Lan.Shapes.Interfaces
{
    /// <summary>
    /// 方形两个点，top, bottom
    /// 十字，四个点，竖直方向矩形 top + bottom, 水平方向矩形 top + bottom
    /// </summary>
    public class PointsData : IGeometryMetaData
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public PointsData(double strokeThickness, List<Point> dataPoints)
        {
            StrokeThickness = strokeThickness;
            DataPoints = dataPoints;
        }


        public PointsData(string tag, double strokeThickness, List<Point> dataPoints):this(strokeThickness,dataPoints)
        {
            Tag = tag;
        }

        public TagPosition TagPosition { get; set; } = TagPosition.Center;
        public string Tag { get; set; }
        public double StrokeThickness { get; set; }
        public List<Point> DataPoints { get; set; }
    }
}