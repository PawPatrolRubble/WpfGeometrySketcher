using System;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Handle;

namespace Lan.Shapes.Custom
{
    public abstract class CustomGeometryBase : ShapeVisualBase
    {
        protected readonly DragHandle DistanceResizeHandle = new RectDragHandle(new Size(10, 10), new Point(), 10, 99);

        protected Pen? Pen;
        protected const double MaxStrokeThickness = 50;
        protected readonly Pen DragHandlePen = new Pen(Brushes.Red, 1);

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }
        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }

        protected override void CreateHandles()
        {
            
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}