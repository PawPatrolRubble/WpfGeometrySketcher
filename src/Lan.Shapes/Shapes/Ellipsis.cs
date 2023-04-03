using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Shapes
{
    public class Ellipsis:ShapeVisual
    {

        public override Geometry RenderGeometry { get; }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonDown(Point newPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
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

        public override void CreateHandles()
        {
            throw new NotImplementedException();
        }
    }
}
