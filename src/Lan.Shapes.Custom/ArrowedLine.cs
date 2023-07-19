using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Custom
{
    public class ArrowedLine : ThickenedLine
    {
        #region fields

        private readonly PathGeometry _arrowGeometry = new PathGeometry();
        private readonly PathFigure _pathFigure = new PathFigure();
        private readonly double _triangleHeight = 50;
        private readonly double _triangleWidth = 30;
        #endregion

        #region constructors

        public ArrowedLine(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            //_line = new ThickenedLine(shapeLayer);
            _arrowGeometry.Figures.Add(_pathFigure);
            RenderGeometryGroup.Children.Add(_arrowGeometry);
            _pathFigure.IsClosed = true;
        }

        #endregion

        #region all other members

        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            //do nothing
        }

        public override void UpdateVisual()
        {
            if (CanRenderGeometry())
            {
                UpdateTriangle();
            }

            base.UpdateVisual();
        }

        //
        private void UpdateTriangle()
        {
            var p1 = End - new Vector(_triangleHeight, _triangleWidth / 2);
            var p2 = End - new Vector(_triangleHeight, -_triangleWidth / 2);
            var angle = GetAngleBetweenPoints(Start, End);
            var matrix = new Matrix();
            matrix.RotateAt(angle, End.X, End.Y);
            p1 = matrix.Transform(p1);
            p2 = matrix.Transform(p2);

            //_pathFigure.Segments.Clear();

            _pathFigure.StartPoint = End;
            if (_pathFigure.Segments.Count == 2)
            {
                if (_pathFigure.Segments[0] is LineSegment line1)
                {
                    line1.Point = p1;
                }

                if (_pathFigure.Segments[1] is LineSegment line2)
                {
                    line2.Point = p2;
                }
            }
            else
            {
                _pathFigure.Segments.Add(new LineSegment(p1, true));
                _pathFigure.Segments.Add(new LineSegment(p2, true));
            }

        }
        #endregion
    }
}