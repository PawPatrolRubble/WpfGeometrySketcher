using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Enums;
using Lan.Shapes.Handle;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Custom
{
    public class Fiber: CustomGeometryBase
    {
        private readonly RectangleGeometry _outerBoundGeometry = new RectangleGeometry(new Rect(new Point(),new Point()));
        private readonly PathGeometry _triganleTipGeometry = new PathGeometry();
        private readonly EllipseGeometry _filletGeometry = new EllipseGeometry(new Point(),0,0);

        private readonly LineSegment _leftTriangleLine = new LineSegment(new Point(),true);
        private readonly LineSegment _rightTriangleLine = new LineSegment(new Point(), true);

        
        private readonly RectDragHandle _topLeftHandle;
        private readonly RectDragHandle _topRightHandle;
        private readonly RectDragHandle _bottomLeftHandle;
        private readonly RectDragHandle _bottomRightHandle;
        private readonly RectDragHandle _tranangleHeightHandle;

        private const int TriangleHeightHandleId = 10;

        public Fiber(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            var pathFigures = new PathFigure()
            {
                StartPoint = new Point()
            };

            pathFigures.Segments.Add(_leftTriangleLine);
            pathFigures.Segments.Add(_rightTriangleLine);

            _triganleTipGeometry.Figures.Add(pathFigures);

            var defaultStyle = shapeLayer.GetStyler(ShapeVisualState.Normal);
            _topLeftHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle,new Point(),1);
            _topRightHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle,new Point(),2);
            _bottomRightHandle= RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle,new Point(),3);
            _bottomLeftHandle= RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle,new Point(),4);
            _tranangleHeightHandle= RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle,new Point(),5);

            Handles.AddRange(new[]
            {
                _topLeftHandle,
                _topRightHandle,
                _bottomLeftHandle,
                _tranangleHeightHandle,
                _bottomRightHandle,
            });

            this.RenderGeometryGroup.Children.Add(_outerBoundGeometry);
            this.RenderGeometryGroup.Children.Add(_triganleTipGeometry);
            this.RenderGeometryGroup.Children.Add(_filletGeometry);
        }

        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            throw new NotImplementedException();
        }



        private Point _start;

        public Point Start
        {
            get => _start;
            set { SetField(ref _start, value); }
        }

        private Point _end;

        public Point End
        {
            get => _end;
            set { SetField(ref _end, value); }
        }

        private Point _tipBottomRight;

        public Point TipBottomRight
        {
            get => _tipBottomRight;
            set { SetField(ref _tipBottomRight, value); }
        }



        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (IsGeometryRendered)
            {
                //set the start point
                FindSelectedHandle(mousePoint);
            }
            else
            {
                //find handle
                Start = mousePoint;
                End = mousePoint;
                MouseDownPoint = mousePoint;
            }

            OldPointForTranslate = mousePoint;
        }


        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (buttonState == MouseButtonState.Pressed)
            {
                if (!IsGeometryRendered)
                {
                    //draw the whole geometry
                    End = point;
                }else if (SelectedDragHandle != null)
                {
                    switch (SelectedDragHandle.Id)
                    {
                        case 1://top left
                            Start = point;
                            break;
                        case 2:
                            End = new Point(point.X, End.Y);
                            break;

                        case 3://bottom right
                            End = point;
                            break;
                        case 4:
                            Start = new Point(point.X, Start.Y);
                            break;
                        case 5://update the triangle
                            TipBottomRight = point;
                            break;
                    }
                }

            }

            base.OnMouseMove(point, buttonState);
        }


        private void UpdateGeometryOnMouseMove()
        {
            //todo

        }


        private double _filletRadius;

        /// <summary>
        /// when the raidus is known, the center of it is determined, as the center has to be tangent to the triangle
        /// </summary>
        public double FilletRadius
        {
            get => _filletRadius;
            set => SetField(ref _filletRadius, value);
        }

        private double _tipAngleInDeg;

        public double TipAngleInDeg
        {
            get => _tipAngleInDeg;
            set => SetField(ref _tipAngleInDeg, value);
        }

        private double _tipFilletCenterY;

        public double TipFilletCenterY
        {
            get => _tipFilletCenterY;
            set => SetField(ref _tipFilletCenterY, value);
        }


    }
}
