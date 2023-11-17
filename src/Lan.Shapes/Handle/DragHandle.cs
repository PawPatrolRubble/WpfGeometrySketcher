#nullable enable
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Handle
{
    [DebuggerDisplay("{GeometryCenter}")]
    public abstract class DragHandle
    {
        #region constructor

        protected DragHandle(Size handleSize, Point geometryCenter, double detectionRange, int id)
        {
            HandleSize = handleSize;
            GeometryCenter = geometryCenter;
            DetectionRange = detectionRange;
            Id = id;
        }

        #endregion

        #region private fields

        private Point _geometryCenter;

        #endregion

        #region properties

        public int Id { get; }

        public double DetectionRange { get; }

        public Size HandleSize { get; }

        public Point GeometryCenter
        {
            get { return _geometryCenter; }
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

        #endregion

        #region other members

        protected abstract void SetCenter(Point center);

        public virtual bool FillContains(Point checkPoint)
        {
            return HandleGeometry?.FillContains(checkPoint, DetectionRange, ToleranceType.Absolute) ?? false;
        }

        #endregion
    }
}