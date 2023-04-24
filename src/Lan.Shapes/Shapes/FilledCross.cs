using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.Shapes
{
    public class FilledCross : ShapeVisualBase
    {

        #region fields

        private readonly RectangleGeometry _vRectangleGeometry = new RectangleGeometry();
        private readonly RectangleGeometry _hRectangleGeometry = new RectangleGeometry();
        private readonly CombinedGeometry _combinedGeometry;


        #endregion


        #region properties

        private Point _topLeft;

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                _topLeft = value;
                if (!IsGeometryRendered)
                {
                    CreateGeometry(_topLeft);
                }
            }
        }

        private void CreateGeometry(Point topLeft)
        {
            _vRectangleGeometry.Rect = new Rect(topLeft, new Size());
        }

        private Point _bottomRight;
        public Point BottomRight
        {
            get => _bottomRight;
            set
            {
                _bottomRight = value;

                HandleResizing(_bottomRight);

            }
        }

        private Point _hTopLeft;
        public Point HTopLeft
        {
            get => _hTopLeft;
            private set => _hTopLeft = value;
        }


        private Point _hBottomRight;
        public Point HBottomRight
        {
            get => _hBottomRight;
            private set => _hBottomRight = value;
        }

        #endregion


        #region constructor

        public FilledCross()
        {
            _combinedGeometry = new CombinedGeometry(GeometryCombineMode.Union, _hRectangleGeometry, _vRectangleGeometry);
        }

        #endregion

        public override Geometry RenderGeometry { get => _combinedGeometry; }


        private readonly Rect _boundsRect;

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect
        {
            get => _boundsRect;
        }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
        {
            throw new NotImplementedException();
        }

        protected override void DrawGeometryInMouseMove(Point oldPoint, Point newPoint)
        {
     
        }


        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {
                TopLeft = mousePoint;
                Console.WriteLine($"cross started from top left co: {mousePoint}");
            }
        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            base.OnMouseLeftButtonUp(newPoint);
            Console.WriteLine($"cross ends at bottom right co: {newPoint}");

        }

        /// <summary>
        /// 鼠标点击移动
        /// </summary>
        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (!IsGeometryRendered)
            {
                if (buttonState == MouseButtonState.Pressed)
                {
                    BottomRight = point;
                    UpdateVisual();
                }
            }
        }

        protected override void HandleResizing(Point point)
        {
            Console.WriteLine($"location in mouse move: {_vRectangleGeometry.Rect.Location}");
            _vRectangleGeometry.Rect = new Rect(_vRectangleGeometry.Rect.Location, point);
            _hRectangleGeometry.Rect = new Rect(GetHTopLeft(), GetHBottomRight());
        }

        private Point GetHBottomRight()
        {
            var width = _vRectangleGeometry.Rect.Width;
           
            return GetHTopLeft() + new Vector(_vRectangleGeometry.Rect.Height,width);
        }

        private Point GetHTopLeft()
        {
            var width = _vRectangleGeometry.Rect.Width;
            var height = _vRectangleGeometry.Rect.Height;
            return _vRectangleGeometry.Rect.Location + new Vector(-(height-width)/2, (height-width)/2);
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
            throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }
    }
}
