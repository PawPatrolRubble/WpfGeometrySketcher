#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Lan.ImageViewer
{
    public class GeometryType
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GeometryType(string name, string description, Geometry? iconGeometry)
        {
            Name = name;
            Description = description;
            IconGeometry = iconGeometry;
        }

        public string Name { get;  }
        public string Description { get; }
        public Geometry? IconGeometry { get; set; }
        

    }
}
