using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    [DebuggerDisplay("{Location}")]
    public class DragHandle
    {
        public int Id { get; }
        public DragHandle(Size handleSize, Point location, double detectionRange, int id)
        {
            HandleSize = handleSize;
            Location = location;
            DetectionRange = detectionRange;
            Id = id;
            HandleGeometry = new EllipseGeometry(Location, HandleSize.Width, HandleSize.Height);
        }

        public double DetectionRange { get; }
        public Size HandleSize { get; }
        private Point _location;

        public Point Location
        {
            get => _location;
            internal set
            {
                _location = value;
                if (HandleGeometry != null)
                {
                    ((EllipseGeometry)HandleGeometry).Center = value;
                }
            }
        }

        public virtual Geometry HandleGeometry { get; }
    }
}