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
            _handleSize = handleSize;  // Use backing field to avoid triggering setter before geometry is ready
            DetectionRange = detectionRange;
            Id = id;
            GeometryCenter = geometryCenter;  // This triggers SetCenter after size is set
        }

        #endregion

        #region private fields

        private Point _geometryCenter;

        #endregion

        #region properties

        public int Id { get; }

        public double DetectionRange { get; }

        private Size _handleSize;
        public Size HandleSize
        {
            get => _handleSize;
            set
            {
                _handleSize = value;
                // Update geometry when size changes
                if (HandleGeometry != null)
                {
                    SetCenter(_geometryCenter);
                }
            }
        }

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

        public abstract bool FillContains(Point checkPoint);

        #endregion
    }
}