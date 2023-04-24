#nullable enable
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    [DebuggerDisplay("{GeometryCenter}")]
    public abstract class DragHandle
    {
        public int Id { get; }

        protected DragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id)
        {
            HandleSize = handleSize;
            GeometryCenter = geometryCenter;
            DetectionRange = detectionRange;
            Id = id;
        }

        public double DetectionRange { get; }

        public Size HandleSize { get; }


        private Point _geometryCenter;
        public Point GeometryCenter
        {
            get => _geometryCenter;
            set
            {
                _geometryCenter = value;
                if (HandleGeometry != null)
                {
                    SetCenter(_geometryCenter);
                }
            }
        }

        public abstract Geometry? HandleGeometry { get; }

        protected abstract void SetCenter(Point center);

        public virtual bool FillContains(Point checkPoint)
        {
            return HandleGeometry?.FillContains(checkPoint, DetectionRange, ToleranceType.Absolute) ?? false;
        }
    }
}