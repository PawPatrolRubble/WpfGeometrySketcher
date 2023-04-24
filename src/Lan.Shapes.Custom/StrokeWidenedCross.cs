using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Custom
{
    public class StrokeWidenedCross : ShapeVisualBase
    {

        #region fields

        private readonly FilledCross _middleCross = new FilledCross();
        private readonly FilledCross _outerCross = new FilledCross();
        private readonly FilledCross _innerCross = new FilledCross();

        private readonly Pen _middlePen = new Pen(Brushes.Red, 1);
        private Vector _halfDistanceVector;
        #endregion


        #region properties

        private Point _verticalTopLeft;
        private Point VerticalTopLeft
        {
            get => _verticalTopLeft;
            set
            {
                _verticalTopLeft = value;
                UpdateTopLeftLocationForGeometry(_verticalTopLeft);
            }
        }

        private Point _verticalBottomRight;
        public Point VerticalBottomRight
        {
            get => _verticalBottomRight;
            set
            {
                _verticalBottomRight = value;
                UpdateBottomRightForGeometry(_horizontalBottomRight);
            }
        }


        private Point _horizontalBottomRight;
        public Point HorizontalBottomRight
        {
            get => _horizontalBottomRight;
            set
            {
                _horizontalBottomRight = value;
            }
        }

        private Point _horizontalTopLeft;
        public Point HorizontalTopLeft
        {
            get => _horizontalTopLeft;
            set => _horizontalTopLeft = value;
        }


        private double _distance;

        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;

                _halfDistanceVector.X = value / 2;
                _halfDistanceVector.Y = value / 2;
            }
        }


        private void UpdateTopLeftLocationForGeometry(Point topLeft)
        {
            //_middleCross.OnMouseLeftButtonDown();
            //_middleCross.TopLeft = topLeft;
            //_outerCross.TopLeft = topLeft - _halfDistanceVector;
            //_innerCross.TopLeft = topLeft + _halfDistanceVector;
        }

        private void UpdateBottomRightForGeometry(Point bottomRight)
        {
            //    _middleCross.BottomRight = bottomRight;
            //    _outerCross.BottomRight = bottomRight + _halfDistanceVector;
            //    _innerCross.BottomRight= bottomRight - _halfDistanceVector;
        }

        #endregion

        #region ctor

        public StrokeWidenedCross()
        {
            _halfDistanceVector = new Vector();
            Distance = 20;
            _middlePen.DashStyle = DashStyles.DashDot;

            RenderGeometryGroup.Children.Add(new CombinedGeometry(GeometryCombineMode.Xor,_outerCross.RenderGeometry,_innerCross.RenderGeometry));
        }

        #endregion

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            base.OnMouseLeftButtonUp(newPoint);
            _middleCross.OnMouseLeftButtonUp(newPoint);
            _innerCross.OnMouseLeftButtonUp(newPoint);
            _outerCross.OnMouseLeftButtonUp(newPoint);
        }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (!IsGeometryRendered)
            {

                _outerCross.OnMouseLeftButtonDown(mousePoint - _halfDistanceVector);
                _innerCross.OnMouseLeftButtonDown(mousePoint + _halfDistanceVector);
                _middleCross.OnMouseLeftButtonDown(mousePoint);
                //UpdateVisual();
            }
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
                    Console.WriteLine($"point in cross: {point}");
                    //VerticalBottomRight = point;
                    _middleCross.OnMouseMove(point, buttonState);
                    _outerCross.OnMouseMove(point + _halfDistanceVector, buttonState);
                    if ((point - _innerCross.TopLeft).X > Distance && (point - _innerCross.TopLeft).Y > Distance)
                    {
                        _innerCross.OnMouseMove(point - _halfDistanceVector, buttonState);
                    }
                    UpdateVisual();
                }
            }
        }


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
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }


        public override void UpdateVisual()
        {
            var renderContext = RenderOpen();
            if (ShapeStyler != null)
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometryGroup);


            renderContext.DrawGeometry(Brushes.Transparent, _middlePen, _middleCross.RenderGeometry);

            //var outerPen = new Pen(Brushes.Blue, 1);
            //renderContext.DrawGeometry(Brushes.Transparent, outerPen, _outerCross.RenderGeometry);

            //var innerPen = new Pen(Brushes.Green, 1);

            //renderContext.DrawGeometry(Brushes.Transparent, innerPen, _innerCross.RenderGeometry);
            renderContext.Close();

        }
    }
}
