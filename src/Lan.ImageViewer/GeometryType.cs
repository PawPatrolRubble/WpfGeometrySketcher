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
        public GeometryType(string name, string description, ImageSource? iconData)
        {
            Name = name;
            Description = description;
            IconData = iconData;
        }

        public string Name { get;  }
        public string Description { get; }
        public ImageSource? IconData { get; set; }
    }
}
