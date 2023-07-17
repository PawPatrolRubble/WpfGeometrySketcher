using System.Windows;

namespace Lan.Shapes.ExtensionMethods
{
    public static class PointExtension
    {
        public static Point MiddleWith(this Point pointStart, Point pointEnd)
        {
            return new Point((pointStart.X + pointEnd.X) / 2, (pointStart.Y + pointEnd.Y) / 2);
        }
    }
}