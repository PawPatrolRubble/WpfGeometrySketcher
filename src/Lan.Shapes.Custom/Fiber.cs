using System;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Enums;
using Lan.Shapes.Handle;
using Vector = System.Windows.Vector;

namespace Lan.Shapes.Custom
{
    public static class LineHelper
    {
   
        #region properties        
        public static double Length(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
        }

        public static double Angle(Point start, Point end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X);
        }

        #endregion

        #region other members

        public static bool TryGetIntersection(
            Vector2 p1, Vector2 p2,
            Vector2 p3, Vector2 p4,
            out Vector2 intersection)
        {
            intersection = default;

            float x1 = p1.X, y1 = p1.Y;
            float x2 = p2.X, y2 = p2.Y;
            float x3 = p3.X, y3 = p3.Y;
            float x4 = p4.X, y4 = p4.Y;

            var denominator = (x1 - x2) * (y3 - y4) -
                              (y1 - y2) * (x3 - x4);

            if (Math.Abs(denominator) < 1e-6)
            {
                // Lines are parallel or coincident
                return false;
            }

            var pre = x1 * y2 - y1 * x2;
            var post = x3 * y4 - y3 * x4;

            var x = (pre * (x3 - x4) - (x1 - x2) * post) / denominator;
            var y = (pre * (y3 - y4) - (y1 - y2) * post) / denominator;

            intersection = new Vector2(x, y);
            return true;
        }

        public static Point GetIntersectionWithLine(Point start, Point end, Point lineStart, Point lineEnd)
        {
            TryGetIntersection(new Vector2((float)start.X, (float)start.Y), new Vector2((float)end.X, (float)end.Y),
                new Vector2((float)lineStart.X, (float)lineStart.Y),
                new Vector2((float)lineEnd.X, (float)lineEnd.Y), out var intersection);
            return new Point(intersection.X, intersection.Y);
        }

        public static double GetAngleBetweenLines(Point start, Point end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X);
        }

        /// <summary>
        /// Creates a perpendicular line through the specified point
        /// </summary>
        /// <param name="point">The point through which the perpendicular line passes</param>
        /// <param name="length">The length of the perpendicular line</param>
        /// <returns>A new line that is perpendicular to this line and passes through the specified point</returns>
        public static (Point, Point) GetPerpendicularLineThroughPoint(Point lineStart, Point lineEnd, Point point, double length = 200)
        {
            // Calculate the direction vector of the line
            Vector direction = new Vector(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
            
            // Normalize the direction vector
            if (direction.Length > 0)
            {
                direction.Normalize();
            }
            else
            {
                return (point, point); // Return degenerate line if input is degenerate
            }
    
            // Calculate the perpendicular vector (-y, x)
            Vector perpendicular = new Vector(direction.Y, -direction.X);
    
            // Scale the perpendicular vector to the desired length
            perpendicular *= length / 2;
            
            // Create the perpendicular line through the point
            Point start = new Point(point.X - perpendicular.X, point.Y - perpendicular.Y);
            Point end = new Point(point.X + perpendicular.X, point.Y + perpendicular.Y);
            
            return (start, end);
        }

        /// <summary>
        /// Creates a line parallel to the given line and passing through the specified point
        /// </summary>
        /// <param name="lineStart">The start point of the original line</param>
        /// <param name="lineEnd">The end point of the original line</param>
        /// <param name="point">The point through which the parallel line passes</param>
        /// <returns>A tuple containing the start and end points of the parallel line with the same length as the original line</returns>
        public static (Point, Point) GetParallelLineThroughPoint(Point lineStart, Point lineEnd, Point point)
        {
            // Calculate the direction vector of the original line
            Vector direction = new Vector(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
            
            // If the original line is degenerate (zero length), return a degenerate line
            if (direction.Length < 1e-6)
            {
                return (point, point);
            }
            
            // Get the original line length
            double originalLength = direction.Length;
            
            // Normalize the direction vector
            direction.Normalize();
            
            // Scale the direction vector to half the original length
            direction *= originalLength / 2;
            
            // Create the parallel line through the point with the same length as the original
            Point start = new Point(point.X - direction.X, point.Y - direction.Y);
            Point end = new Point(point.X + direction.X, point.Y + direction.Y);
            
            return (start, end);
        }

        #endregion
    }


    public class Fiber : CustomGeometryBase
    {
        #region constructor

        private Point _rectTopLeft;
        private Point _rectTopRight;
        private Point _rectBottomLeft;
        private Point _rectBottomRight;

        public Fiber(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            var defaultStyle = shapeLayer.GetStyler(ShapeVisualState.Normal);
            _topLeftHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 1);
            _topRightHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 2);
            _bottomRightHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 3);
            _bottomLeftHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 4);
            _triangleBottomCenterHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 5);
            _triangleTipHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 6);

            _pathGeometry = new PathGeometry();
            _pathFigure = new PathFigure();

            _pathGeometry.Figures.Add(_pathFigure);
            _pathFigure.IsClosed = false;
            _pathGeometry.FillRule = FillRule.Nonzero;
            
            Handles.AddRange(new[]
            {
                _topLeftHandle,
                _topRightHandle,
                _bottomLeftHandle,
                _bottomRightHandle,
                _triangleBottomCenterHandle,
                _triangleTipHandle
            });

            RenderGeometryGroup.Children.Add(_pathGeometry);
            RenderGeometryGroup.Children.Add(_topLeftHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_topRightHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_bottomLeftHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_bottomRightHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_triangleBottomCenterHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_triangleTipHandle.HandleGeometry);
        }

        #endregion

        #region private fields

        private readonly PathGeometry _pathGeometry;
        private readonly PathFigure _pathFigure;
        private readonly RectDragHandle _bottomLeftHandle;
        private readonly RectDragHandle _bottomRightHandle;

        private readonly RectDragHandle _topLeftHandle;
        private readonly RectDragHandle _topRightHandle;

        //placed at the center of bottom edge of triangle
        private readonly RectDragHandle _triangleBottomCenterHandle;
        private readonly RectDragHandle _triangleTipHandle;

        public Point RectTopLeft
        {
            get => _rectTopLeft; set
            {
                _rectTopLeft = value;
                UpdateGeometry();
            }
        }

        private void UpdateGeometry()
        {
            //recreate the geometry
            
            _pathFigure.Segments.Clear();
            _pathFigure.StartPoint = RectTopLeft;
            _pathFigure.Segments.Add(new LineSegment(RectTopRight, true));
            _pathFigure.Segments.Add(new LineSegment(RectBottomRight, true));
            _pathFigure.Segments.Add(new LineSegment(RectBottomLeft, true));
            _pathFigure.Segments.Add(new LineSegment(RectTopLeft, true));
            
            // Calculate the top center point of the rectangle
            Point topCenter = new Point(
                (RectTopLeft.X + RectTopRight.X) / 2,
                (RectTopLeft.Y + RectTopRight.Y) / 2
            );
            
            // Calculate the apex point on the top edge of the rectangle
            // The apex will be positioned along the top edge based on TipAngleInDeg
            // Convert TipAngleInDeg from degrees to radians for calculations
            double angleInRadians = TipAngleInDeg * Math.PI / 180.0;
            
            // Calculate the position of the apex along the top edge
            // 0 degrees would be at the left end, 90 degrees at the right end
            double positionRatio = TipAngleInDeg / 90.0;
            positionRatio = Math.Max(0.0, Math.Min(1.0, positionRatio)); // Clamp between 0 and 1
            
            // Calculate the apex point on the top edge
            Point apex = new Point(
                RectTopLeft.X + (RectTopRight.X - RectTopLeft.X) * positionRatio,
                RectTopLeft.Y + (RectTopRight.Y - RectTopLeft.Y) * positionRatio
            );
            
            // Store the apex point
            TipBottomRight = apex;
            
            // Update the triangle tip handle position
            _triangleTipHandle.GeometryCenter = apex;
            
            // Calculate a point inside the rectangle (e.g., center of the rectangle)
            Point rectCenter = new Point(
                (RectTopLeft.X + RectBottomRight.X) / 2,
                (RectTopLeft.Y + RectBottomRight.Y) / 2
            );
            
            // Calculate intersection points with left and right edges
            // We draw lines from the apex to the left and right edges through the rectangle center
            Point leftEdgeIntersection = GetIntersectionWithEdge(TipBottomRight, rectCenter, RectTopLeft, RectBottomLeft);
            Point rightEdgeIntersection = GetIntersectionWithEdge(TipBottomRight, rectCenter, RectTopRight, RectBottomRight);
            
            // First draw the rectangle
            // The rectangle is already drawn by the previous segments (lines 221-225)
            
            // Now add the triangle segments to the path figure
            // Start a new path from the apex
            _pathFigure.Segments.Add(new LineSegment(TipBottomRight, true));
            
            // Draw line from apex to right edge intersection
            _pathFigure.Segments.Add(new LineSegment(rightEdgeIntersection, true));
            
            // Draw line from right edge intersection to left edge intersection
            // (This creates the base of the triangle)
            _pathFigure.Segments.Add(new LineSegment(leftEdgeIntersection, true));
            
            // Draw line from left edge intersection back to the apex to close the triangle
            _pathFigure.Segments.Add(new LineSegment(TipBottomRight, true));
            
            // Update handle positions
            _topLeftHandle.GeometryCenter = RectTopLeft;
            _topRightHandle.GeometryCenter = RectTopRight;
            _bottomLeftHandle.GeometryCenter = RectBottomLeft;
            _bottomRightHandle.GeometryCenter = RectBottomRight;
            _triangleBottomCenterHandle.GeometryCenter = topCenter;
            _triangleTipHandle.GeometryCenter = TipBottomRight;
            
            UpdateVisual();
        }

        public Point RectTopRight
        {
            get => _rectTopRight; set
            {
                _rectTopRight = value;
                UpdateGeometry();
            }
        }

        public Point RectBottomLeft
        {
            get => _rectBottomLeft; set
            {
                _rectBottomLeft = value;
                UpdateGeometry();
            }
        }

        public Point RectBottomRight
        {
            get => _rectBottomRight; set
            {
                _rectBottomRight = value;
                UpdateGeometry();
            }
        }

        //default fillet radius is 20
        private double _filletRadius = 20;
        private double _tipAngleInDeg = 45;
        private Point _tipBottomRight; // The apex point of the triangle

        #endregion

        #region properties


        /// <summary>
        ///     when the radius is known, the center of it is determined, as the center has to be tangent to the triangle
        /// </summary>
        public double FilletRadius
        {
            get { return _filletRadius; }
            set { SetField(ref _filletRadius, value); }
        }

        public double TipAngleInDeg
        {
            get { return _tipAngleInDeg; }
            set
            {
                SetField(ref _tipAngleInDeg, value);
                UpdateGeometryOnMouseMove();
            }
        }

        /// <summary>
        ///     The apex point of the triangle at the top of the fiber
        /// </summary>
        public Point TipBottomRight
        {
            get { return _tipBottomRight; }
            private set { SetField(ref _tipBottomRight, value); }
        }


        #endregion

        #region other members

        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            if (IsGeometryRendered)
            {
                //set the start point
                FindSelectedHandle(mousePoint);
            }
            else
            {
                RectTopLeft = mousePoint;
                //find handle
                RectBottomRight = mousePoint;
                RectTopRight = mousePoint;
                RectBottomLeft = mousePoint;
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
                    RectBottomLeft = point;
                    var (bottomRightStart, bottomRightEnd) = LineHelper.GetPerpendicularLineThroughPoint(RectTopLeft, RectBottomLeft, point);
                    var (topRightStart, topRightEnd) = LineHelper.GetPerpendicularLineThroughPoint(RectTopLeft, RectBottomLeft, RectTopLeft);
                    RectBottomRight = bottomRightEnd;
                    RectTopRight = topRightEnd;

                    //update handle position
                    _topLeftHandle.GeometryCenter = RectTopLeft;
                    _topRightHandle.GeometryCenter = RectTopRight;
                    _bottomLeftHandle.GeometryCenter = RectBottomLeft;
                    _bottomRightHandle.GeometryCenter = RectBottomRight;

                }
                else if (SelectedDragHandle != null)
                {
                    switch (SelectedDragHandle.Id)
                    {
                        case 1: //top left
                            // When dragging top left, we need to:
                            // 1. Keep the bottom right corner fixed
                            // 2. Find the best rectangle that includes both the fixed corner and the mouse position
                            // 3. Maintain the current orientation of the rectangle
                            
                            // Calculate the current rectangle's orientation vectors
                            Vector rightDir = new Vector(RectTopRight.X - RectTopLeft.X, RectTopRight.Y - RectTopLeft.Y);
                            Vector downDir = new Vector(RectBottomLeft.X - RectTopLeft.X, RectBottomLeft.Y - RectTopLeft.Y);
                            
                            if (rightDir.Length > 0) rightDir.Normalize();
                            if (downDir.Length > 0) downDir.Normalize();
                            
                            // Calculate the vector from the fixed corner to the mouse position
                            Vector toMouse = new Vector(point.X - RectBottomRight.X, point.Y - RectBottomRight.Y);
                            
                            // Project the mouse vector onto the rectangle's own axes
                            double rightProj = -Vector.Multiply(toMouse, rightDir); // Negative because we're moving left from bottom right
                            double downProj = -Vector.Multiply(toMouse, downDir);   // Negative because we're moving up from bottom right
                            
                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            Vector rightOffset = Vector.Multiply(rightDir, rightProj);
                            Vector downOffset = Vector.Multiply(downDir, downProj);
                            
                            // Calculate new corner positions
                            Point newTopLeft = new Point(
                                RectBottomRight.X - rightOffset.X - downOffset.X,
                                RectBottomRight.Y - rightOffset.Y - downOffset.Y);
                                
                            Point newTopRight = new Point(
                                RectBottomRight.X - downOffset.X,
                                RectBottomRight.Y - downOffset.Y);
                                
                            Point newBottomLeft = new Point(
                                RectBottomRight.X - rightOffset.X,
                                RectBottomRight.Y - rightOffset.Y);
                            
                            // Update all corners except bottom right (which stays fixed)
                            RectTopLeft = newTopLeft;
                            RectTopRight = newTopRight;
                            RectBottomLeft = newBottomLeft;
                            break;
                            
                        case 2: //top right
                            // When dragging top right, we need to:
                            // 1. Keep the bottom left corner fixed
                            // 2. Find the best rectangle that includes both the fixed corner and the mouse position
                            // 3. Maintain the current orientation of the rectangle
                            
                            // Calculate the current rectangle's orientation vectors
                            Vector leftDirTR = new Vector(RectTopLeft.X - RectTopRight.X, RectTopLeft.Y - RectTopRight.Y);
                            Vector downDirTR = new Vector(RectBottomRight.X - RectTopRight.X, RectBottomRight.Y - RectTopRight.Y);
                            
                            if (leftDirTR.Length > 0) leftDirTR.Normalize();
                            if (downDirTR.Length > 0) downDirTR.Normalize();
                            
                            // Calculate the vector from the fixed corner to the mouse position
                            Vector toMouseTR = new Vector(point.X - RectBottomLeft.X, point.Y - RectBottomLeft.Y);
                            
                            // Project the mouse vector onto the rectangle's own axes
                            double leftProjTR = -Vector.Multiply(toMouseTR, leftDirTR);  // Negative because we want the opposite direction
                            double downProjTR = -Vector.Multiply(toMouseTR, downDirTR);  // Negative because we're moving up from bottom left
                            
                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            Vector leftOffsetTR = Vector.Multiply(leftDirTR, leftProjTR);
                            Vector downOffsetTR = Vector.Multiply(downDirTR, downProjTR);
                            
                            // Calculate new corner positions
                            Point newTopRightTR = new Point(
                                RectBottomLeft.X - leftOffsetTR.X - downOffsetTR.X,
                                RectBottomLeft.Y - leftOffsetTR.Y - downOffsetTR.Y);
                                
                            Point newTopLeftTR = new Point(
                                RectBottomLeft.X - downOffsetTR.X,
                                RectBottomLeft.Y - downOffsetTR.Y);
                                
                            Point newBottomRightTR = new Point(
                                RectBottomLeft.X - leftOffsetTR.X,
                                RectBottomLeft.Y - leftOffsetTR.Y);
                            
                            // Update all corners except bottom left (which stays fixed)
                            RectTopRight = newTopRightTR;
                            RectTopLeft = newTopLeftTR;
                            RectBottomRight = newBottomRightTR;
                            break;
                            
                        case 3: //bottom right
                            // When dragging bottom right, we need to:
                            // 1. Keep the top left corner fixed
                            // 2. Find the best rectangle that includes both the fixed corner and the mouse position
                            // 3. Maintain the current orientation of the rectangle
                            
                            // Calculate the current rectangle's orientation vectors
                            Vector rightDirBR = new Vector(RectTopRight.X - RectTopLeft.X, RectTopRight.Y - RectTopLeft.Y);
                            Vector downDirBR = new Vector(RectBottomLeft.X - RectTopLeft.X, RectBottomLeft.Y - RectTopLeft.Y);
                            
                            if (rightDirBR.Length > 0) rightDirBR.Normalize();
                            if (downDirBR.Length > 0) downDirBR.Normalize();
                            
                            // Calculate the vector from the fixed corner to the mouse position
                            Vector toMouseBR = new Vector(point.X - RectTopLeft.X, point.Y - RectTopLeft.Y);
                            
                            // Project the mouse vector onto the rectangle's own axes
                            double rightProjBR = Vector.Multiply(toMouseBR, rightDirBR);
                            double downProjBR = Vector.Multiply(toMouseBR, downDirBR);
                            
                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            Vector rightOffsetBR = Vector.Multiply(rightDirBR, rightProjBR);
                            Vector downOffsetBR = Vector.Multiply(downDirBR, downProjBR);
                            
                            // Calculate new corner positions
                            Point newBottomRightBR = new Point(
                                RectTopLeft.X + rightOffsetBR.X + downOffsetBR.X,
                                RectTopLeft.Y + rightOffsetBR.Y + downOffsetBR.Y);
                                
                            Point newTopRightBR = new Point(
                                RectTopLeft.X + rightOffsetBR.X,
                                RectTopLeft.Y + rightOffsetBR.Y);
                                
                            Point newBottomLeftBR = new Point(
                                RectTopLeft.X + downOffsetBR.X,
                                RectTopLeft.Y + downOffsetBR.Y);
                            
                            // Update all corners except top left (which stays fixed)
                            RectBottomRight = newBottomRightBR;
                            RectTopRight = newTopRightBR;
                            RectBottomLeft = newBottomLeftBR;
                            break;
                            
                        case 4: //bottom left
                            // When dragging bottom left, we need to:
                            // 1. Keep the top right corner fixed
                            // 2. Find the best rectangle that includes both the fixed corner and the mouse position
                            // 3. Maintain the current orientation of the rectangle
                            
                            // Calculate the current rectangle's orientation vectors
                            Vector leftDirBL = new Vector(RectTopLeft.X - RectTopRight.X, RectTopLeft.Y - RectTopRight.Y);
                            Vector downDirBL = new Vector(RectBottomRight.X - RectTopRight.X, RectBottomRight.Y - RectTopRight.Y);
                            
                            if (leftDirBL.Length > 0) leftDirBL.Normalize();
                            if (downDirBL.Length > 0) downDirBL.Normalize();
                            
                            // Calculate the vector from the fixed corner to the mouse position
                            Vector toMouseBL = new Vector(point.X - RectTopRight.X, point.Y - RectTopRight.Y);
                            
                            // Project the mouse vector onto the rectangle's own axes
                            double leftProjBL = Vector.Multiply(toMouseBL, leftDirBL);
                            double downProjBL = Vector.Multiply(toMouseBL, downDirBL);
                            
                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            Vector leftOffsetBL = Vector.Multiply(leftDirBL, leftProjBL);
                            Vector downOffsetBL = Vector.Multiply(downDirBL, downProjBL);
                            
                            // Calculate new corner positions
                            Point newBottomLeftBL = new Point(
                                RectTopRight.X + leftOffsetBL.X + downOffsetBL.X,
                                RectTopRight.Y + leftOffsetBL.Y + downOffsetBL.Y);
                                
                            Point newTopLeftBL = new Point(
                                RectTopRight.X + leftOffsetBL.X,
                                RectTopRight.Y + leftOffsetBL.Y);
                                
                            Point newBottomRightBL = new Point(
                                RectTopRight.X + downOffsetBL.X,
                                RectTopRight.Y + downOffsetBL.Y);
                            
                            // Update all corners except top right (which stays fixed)
                            RectBottomLeft = newBottomLeftBL;
                            RectTopLeft = newTopLeftBL;
                            RectBottomRight = newBottomRightBL;
                            break;
                        case 5: //update the triangle
                            TipAngleInDeg = CalculateTipAngleInDeg(point);
                            break;
                    }
                }
                else
                {
                    HandleTranslate(point);
                }
            }

            //base.OnMouseMove(point, buttonState);
        }

        protected override void HandleTranslate(Point point)
        {

            if (!OldPointForTranslate.HasValue)
            {
                return;
            }

            var oldPoint = OldPointForTranslate.Value;
            var dx = point.X - oldPoint.X;
            var dy = point.Y - oldPoint.Y;

            RectBottomLeft = new Point(RectBottomLeft.X + dx, RectBottomLeft.Y + dy);
            RectTopLeft = new Point(RectTopLeft.X + dx, RectTopLeft.Y + dy);
            RectBottomRight = new Point(RectBottomRight.X + dx, RectBottomRight.Y + dy);
            RectTopRight = new Point(RectTopRight.X + dx, RectTopRight.Y + dy);

            //update handle position
            // _topLeftHandle.GeometryCenter = RectTopLeft;
            // _topRightHandle.GeometryCenter = RectTopRight;
            // _bottomLeftHandle.GeometryCenter = RectBottomLeft;
            // _bottomRightHandle.GeometryCenter = RectBottomRight;

            OldPointForTranslate = point;
            
        }
        

        private double CalculateTipAngleInDeg(Point point)
        {
            // Calculate the length of the top edge
            Vector topEdge = new Vector(RectTopRight.X - RectTopLeft.X, RectTopRight.Y - RectTopLeft.Y);
            double topEdgeLength = topEdge.Length;
            
            // Calculate the vector from top-left to the mouse point
            Vector topLeftToPoint = new Vector(point.X - RectTopLeft.X, point.Y - RectTopLeft.Y);
            
            // Project this vector onto the top edge to find where along the edge the mouse is
            if (topEdge.Length > 0)
            {
                topEdge.Normalize(); // Normalize for projection calculation
            }
            
            // Calculate the projection of the mouse position onto the top edge
            double projection = Vector.Multiply(topLeftToPoint, topEdge);
            
            // Convert this to a ratio along the top edge (0 = left end, 1 = right end)
            double positionRatio = projection / topEdgeLength;
            
            // Clamp the ratio between 0 and 1
            positionRatio = Math.Max(0.0, Math.Min(1.0, positionRatio));
            
            // Convert the position ratio to an angle between 0 and 90 degrees
            double angleInDegrees = positionRatio * 90.0;
            
            // Clamp the angle to a reasonable range (e.g., 10 to 80 degrees)
            return Math.Max(10, Math.Min(80, angleInDegrees));
        }

        private int GetAngleBetweenPoints(Point start, Point end)
        {
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;

            var deg = Convert.ToInt32(Math.Atan2(dy, dx) * (180 / Math.PI));
            if (deg < 0)
            {
                deg += 360;
            }

            return deg;
        }

        /// <summary>
        /// Calculates the intersection point between a line and a rectangle edge
        /// </summary>
        /// <param name="lineStart">Start point of the line</param>
        /// <param name="lineEnd">End point of the line</param>
        /// <param name="edgeStart">Start point of the edge</param>
        /// <param name="edgeEnd">End point of the edge</param>
        /// <returns>The intersection point between the line and the edge</returns>
        private Point GetIntersectionWithEdge(Point lineStart, Point lineEnd, Point edgeStart, Point edgeEnd)
        {
            // Use the LineHelper to calculate the intersection
            return LineHelper.GetIntersectionWithLine(lineStart, lineEnd, edgeStart, edgeEnd);
        }

        private void UpdateGeometryOnMouseMove()
        {
            //update the geometry
            UpdateGeometry();
        }

        private Point CalculateFilletCenter()
        {
            // For the fiber geometry shown in the image, the fillet (circle) should be
            // positioned at the apex of the triangle
            return TipBottomRight;
        }

        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            // Update the stroke thickness for all geometries to maintain uniform appearance when zooming
            // This method is called when the zoom level changes

            // We don't need to do anything special here since the base class should handle
            // the stroke thickness changes for the geometries in RenderGeometryGroup
        }

        #endregion
    }
}