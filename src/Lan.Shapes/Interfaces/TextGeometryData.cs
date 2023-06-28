using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Lan.Shapes.Interfaces
{
    public class TextGeometryData: IGeometryMetaData
    {
        public Point Location { get; set; }
        public double FontSize { get; set; }

        /// <summary>
        /// content
        /// </summary>
        public string Content { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextGeometryData(Point location, string textContent, double fontSize,double strokeThickness) 
        {
            Location = location;
            Content = textContent;
            FontSize = fontSize;
            StrokeThickness = strokeThickness;
        }

        public double StrokeThickness { get; set; }
    }
}
