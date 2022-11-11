using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Styler;

namespace Lan.Shapes.Shapes
{
    public class Rectangle : ShapeVisual
    {
        private readonly GeometryGroup _geometryGroup = new GeometryGroup();
        
        public override Geometry RenderGeometry
        {
            get => _geometryGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => _geometryGroup.Bounds;
        }


        #region mouse events handler

        private void HandleTranslate(Point point)
        {
            
        }


        private void HandleDragToScale(Point point)
        {
            
        }
        
        
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
        public override void OnMouseMove(MouseEventArgs eventArgs)
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

        #endregion
    }
}