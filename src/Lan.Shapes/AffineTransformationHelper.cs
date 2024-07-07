using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes
{
    public class AffineTransformationHelper
    {

        public static Point Translate(Point source, double deltaX, double deltaY)
        {
            var matrix = new Matrix();

            matrix.Translate(deltaX, deltaY);

            return matrix.Transform(source);
        }

        public static Point Rotate(Point source, double angle)
        {
            var matrix = new Matrix();
            matrix.Rotate(angle);

            return matrix.Transform(source);
        }

        public static Point Scale(Point source, double scaleX, double scaleY)
        {
            var matrix = new Matrix();
            matrix.Scale(scaleX,scaleY);

            return matrix.Transform(source);
        }

    }
}
