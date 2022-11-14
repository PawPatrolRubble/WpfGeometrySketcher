#nullable enable
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Handle;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    public abstract class ShapeVisual : DrawingVisual
    {

        /// <summary>
        /// set this, when basic geometry is set
        /// </summary>
        public bool IsGeometryInitialized { get; protected set; }
        public IShapeStyler ShapeStyler { get; set; }

        public abstract Geometry RenderGeometry { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract Rect BoundsRect { get; }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="newPoint"></param>
        public abstract void OnMouseLeftButtonDown(Point newPoint);

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public abstract void OnMouseLeftButtonUp(Point newPoint);

        /// <summary>
        /// 鼠标点击
        /// </summary>
        public abstract void OnMouseMove(Point point);

        /// <summary>
        /// 选择时
        /// </summary>
        public abstract void OnSelected();

        /// <summary>
        /// 未选择状态
        /// </summary>
        public abstract void OnDeselected();

        
        public virtual void UpdateVisual()
        {
            var renderContext = RenderOpen();
            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);
            renderContext.DrawRectangle(ShapeStyler.FillColor,ShapeStyler.SketchPen,RenderGeometry.Bounds);
            renderContext.Close();
        }

        public virtual void FindSelectedHandle(Point p)
        {
            foreach (var handle in Handles)   
            {
                if (handle.HitTest(p))
                {
                    SelectedDragHandle = handle;
                    return;
                }
            }

            SelectedDragHandle = null;
        }

        protected List<DragHandle> Handles = new List<DragHandle>();
        private DragHandle? _selectedDragHandle;

        protected DragHandle? SelectedDragHandle
        {
            get => _selectedDragHandle;
            set
            {
                _selectedDragHandle = value;
            }
        }

        public abstract void CreateHandles();

    }
}