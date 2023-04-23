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
            HandleGeometry = new RectangleGeometry( new Rect(location.X - handleSize.Width/2, location.Y- handleSize.Height/2, handleSize.Width, handleSize.Height));
        }

        public double DetectionRange { get; }
        public Size HandleSize { get; }

        private Point _location;

        public Point Location
        {
            get => _location;
            set
            {
                _location = value;
                if (HandleGeometry != null)
                {
                    ((RectangleGeometry)HandleGeometry).Rect = new Rect(_location, HandleSize);
                }
            }
        }

        public virtual Geometry HandleGeometry { get; }
    }
}