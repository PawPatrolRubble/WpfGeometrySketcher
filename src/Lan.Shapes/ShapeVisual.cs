using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    public abstract class ShapeVisual : DrawingVisual
    {
        public  IShapeStyler ShapeStyler { get; set; }

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
        public abstract void OnMouseMove(MouseEventArgs eventArgs);

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
            renderContext.Close();
        }
    }
}