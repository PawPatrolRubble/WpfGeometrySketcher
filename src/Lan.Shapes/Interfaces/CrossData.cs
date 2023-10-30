using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Lan.Shapes.Interfaces
{
    public class CrossData:IGeometryMetaData
    {
        public CrossData()
        {
        }

        public int Width { get; set; }
        public int Height { get; set; }


        public Point Center { get; set; }

        public double StrokeThickness { get; set; }
    }
}
