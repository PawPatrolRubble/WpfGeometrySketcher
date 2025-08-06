using System;
using System.Globalization;
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
        #region others

        public static double Angle(Point start, Point end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X);
        }

        public static double Length(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
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
                // Lines are parallel or coincident
                return false;

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
        ///     Creates a perpendicular line through the specified point
        /// </summary>
        /// <param name="point">The point through which the perpendicular line passes</param>
        /// <param name="length">The length of the perpendicular line</param>
        /// <returns>A new line that is perpendicular to this line and passes through the specified point</returns>
        public static (Point, Point) GetPerpendicularLineThroughPoint(Point lineStart, Point lineEnd, Point point,
            double length = 200)
        {
            // Calculate the direction vector of the line
            var direction = new Vector(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);

            // Normalize the direction vector
            if (direction.Length > 0)
                direction.Normalize();
            else
                return (point, point); // Return degenerate line if input is degenerate

            // Calculate the perpendicular vector (-y, x)
            var perpendicular = new Vector(direction.Y, -direction.X);

            // Scale the perpendicular vector to the desired length
            perpendicular *= length / 2;

            // Create the perpendicular line through the point
            var start = new Point(point.X - perpendicular.X, point.Y - perpendicular.Y);
            var end = new Point(point.X + perpendicular.X, point.Y + perpendicular.Y);

            return (start, end);
        }

        /// <summary>
        ///     Creates a line parallel to the given line and passing through the specified point
        /// </summary>
        /// <param name="lineStart">The start point of the original line</param>
        /// <param name="lineEnd">The end point of the original line</param>
        /// <param name="point">The point through which the parallel line passes</param>
        /// <returns>A tuple containing the start and end points of the parallel line with the same length as the original line</returns>
        public static (Point, Point) GetParallelLineThroughPoint(Point lineStart, Point lineEnd, Point point)
        {
            // Calculate the direction vector of the original line
            var direction = new Vector(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);

            // If the original line is degenerate (zero length), return a degenerate line
            if (direction.Length < 1e-6) return (point, point);

            // Get the original line length
            var originalLength = direction.Length;

            // Normalize the direction vector
            direction.Normalize();

            // Scale the direction vector to half the original length
            direction *= originalLength / 2;

            // Create the parallel line through the point with the same length as the original
            var start = new Point(point.X - direction.X, point.Y - direction.Y);
            var end = new Point(point.X + direction.X, point.Y + direction.Y);

            return (start, end);
        }

        #endregion
    }


    public class Fiber : CustomGeometryBase
    {
        #region private fields

        private readonly RectDragHandle _bottomLeftHandle;
        private readonly RectDragHandle _bottomRightHandle;

        // Handle along angle bisector to adjust fillet circle radius
        private readonly RectDragHandle _filletRadiusHandle;
        private readonly PathFigure _pathFigure;

        private readonly PathGeometry _pathGeometry;

        // Handle at bottom edge center for rotation control
        private readonly RectDragHandle _rotationHandle;

        private readonly RectDragHandle _topLeftHandle;
        private readonly RectDragHandle _topRightHandle;


        // Handles at triangle base intersection points with rectangle edges
        private readonly RectDragHandle _triangleLeftBaseHandle;
        private readonly RectDragHandle _triangleRightBaseHandle;

        //default fillet radius is 20
        private double _filletRadius = 30;
        private Point _tipBottomRight; // The apex point of the triangle
        private double _triangleAngleInDeg = 45;

        #endregion

        #region properties

        public Point RectTopLeft
        {
            get => _rectTopLeft;
            set
            {
                _rectTopLeft = value;
                UpdateGeometry();
            }
        }

        public Point RectTopRight
        {
            get => _rectTopRight;
            set
            {
                _rectTopRight = value;
                UpdateGeometry();
            }
        }

        public Point RectBottomLeft
        {
            get => _rectBottomLeft;
            set
            {
                _rectBottomLeft = value;
                UpdateGeometry();
            }
        }

        public Point RectBottomRight
        {
            get => _rectBottomRight;
            set
            {
                _rectBottomRight = value;
                UpdateGeometry();
            }
        }


        /// <summary>
        ///     when the radius is known, the center of it is determined, as the center has to be tangent to the triangle
        /// </summary>
        public double FilletRadius
        {
            get => _filletRadius;
            set
            {
                SetField(ref _filletRadius, value);
                UpdateFilletCircle(); // Update fillet circle when radius changes
                UpdateVisual(); // Refresh visual
            }
        }


        /// <summary>
        ///     The angle formed by the lateral edges and the bottom edge of the triangle
        /// </summary>
        public double TriangleBottomEdgeAngleInDeg
        {
            get => _triangleAngleInDeg;
            set
            {
                SetField(ref _triangleAngleInDeg, value);
                UpdateGeometry();
            }
        }

        /// <summary>
        ///     The apex point of the triangle at the top of the fiber
        /// </summary>
        public Point TriangleApex
        {
            get => _tipBottomRight;
            private set => SetField(ref _tipBottomRight, value);
        }

        #endregion

        #region others

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
            var topCenter = new Point(
                (RectTopLeft.X + RectTopRight.X) / 2,
                (RectTopLeft.Y + RectTopRight.Y) / 2
            );

            // Calculate the apex point at the middle of the rectangle's top edge
            // The apex will always be positioned at the center of the top edge
            var apex = new Point(
                (RectTopLeft.X + RectTopRight.X) / 2,
                (RectTopLeft.Y + RectTopRight.Y) / 2
            );

            // Store the apex point
            TriangleApex = apex;

            // Calculate points on the left and right edges of the rectangle for triangle lateral edges
            // These points are calculated based on the triangle angle
            // The angle formed by the lateral edge and the rectangle edge equals (90° - TriangleAngleInDeg)

            // Convert angle to radians
            var angleInRadians = (90.0 - TriangleBottomEdgeAngleInDeg) * Math.PI / 180.0;

            // Calculate the horizontal distance from apex to the left/right edges
            var halfRectWidth = (RectTopRight.X - RectTopLeft.X) / 2.0;

            // Calculate the vertical distance down from apex using trigonometry
            // tan(angle) = opposite / adjacent = verticalDistance / halfRectWidth
            var verticalDistance = halfRectWidth * Math.Tan(angleInRadians);

            // Calculate the intersection points using proper line intersection
            // Find where the lateral edges (from apex at the given angle) intersect the rectangle edges

            // Convert from triangle bottom edge angle to lateral edge angle with vertical rectangle edge
            // If lateral edge makes angle θ with horizontal bottom, it makes (90° - θ) with vertical rectangle edge
            var lateralToVerticalAngle = 90.0 - TriangleBottomEdgeAngleInDeg;

            var leftEdgePoint =
                GetIntersectionPoint_Line1AnglePoint(RectTopLeft, RectBottomLeft, TriangleApex, lateralToVerticalAngle);
            var rightEdgePoint = GetIntersectionPoint_Line1AnglePoint(RectTopRight, RectBottomRight, TriangleApex,
                -lateralToVerticalAngle);

            // Calculate and update the fillet circle geometry
            UpdateFilletCircle();

            // First draw the rectangle
            // The rectangle is already drawn by the previous segments (lines 221-225)

            // Now add the triangle segments to the path figure
            // Start a new path from the apex
            _pathFigure.Segments.Add(new LineSegment(TriangleApex, true));

            // Draw line from apex to right edge point
            _pathFigure.Segments.Add(new LineSegment(rightEdgePoint, true));

            // Draw line from right edge point to left edge point
            // (This creates the base of the triangle)
            _pathFigure.Segments.Add(new LineSegment(leftEdgePoint, true));

            // Draw line from left edge point back to the apex to close the triangle
            _pathFigure.Segments.Add(new LineSegment(TriangleApex, true));

            //draw middle line of rectangle
            var topEdgeMiddlePoint =
                new Point((RectTopLeft.X + RectTopRight.X) / 2, (RectTopLeft.Y + RectTopRight.Y) / 2);
            var bottomEdgeMiddlePoint = new Point((RectBottomLeft.X + RectBottomRight.X) / 2,
                (RectBottomLeft.Y + RectBottomRight.Y) / 2);
            _pathFigure.Segments.Add(new LineSegment(topEdgeMiddlePoint, true));
            _pathFigure.Segments.Add(new LineSegment(bottomEdgeMiddlePoint, true));

            // Update handle positions
            _topLeftHandle.GeometryCenter = RectTopLeft;
            _topRightHandle.GeometryCenter = RectTopRight;
            _bottomLeftHandle.GeometryCenter = RectBottomLeft;
            _bottomRightHandle.GeometryCenter = RectBottomRight;

            // Ensure triangle base handles are constrained to rectangle edges
            // These points are already calculated to be on the rectangle edges by GetIntersectionPoint_Line1AnglePoint
            _triangleLeftBaseHandle.GeometryCenter = leftEdgePoint;
            _triangleRightBaseHandle.GeometryCenter = rightEdgePoint;

            // Position rotation handle at bottom edge center
            var bottomEdgeCenter = new Point(
                (RectBottomLeft.X + RectBottomRight.X) / 2,
                (RectBottomLeft.Y + RectBottomRight.Y) / 2
            );
            _rotationHandle.GeometryCenter = bottomEdgeCenter;

            UpdateVisual();
        }

        #endregion

        #region constructor

        private Point _rectTopLeft;
        private Point _rectTopRight;
        private Point _rectBottomLeft;
        private Point _rectBottomRight;
        private readonly EllipseGeometry _filletGeometry;

        public Fiber(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            var defaultStyle = shapeLayer.GetStyler(ShapeVisualState.Normal);
            _topLeftHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 1);
            _topRightHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 2);
            _bottomRightHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 3);
            _bottomLeftHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 4);
            _triangleLeftBaseHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 7);
            _triangleRightBaseHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 8);
            _filletRadiusHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 9);
            _rotationHandle = RectDragHandle.CreateRectDragHandleFromStyler(defaultStyle, new Point(), 10);

            _pathGeometry = new PathGeometry();
            _pathFigure = new PathFigure();
            _filletGeometry = new EllipseGeometry();

            _pathGeometry.Figures.Add(_pathFigure);
            _pathFigure.IsClosed = false;
            _pathGeometry.FillRule = FillRule.Nonzero;

            Handles.AddRange(new[]
            {
                _topLeftHandle,
                _topRightHandle,
                _bottomLeftHandle,
                _bottomRightHandle,
                _triangleLeftBaseHandle,
                _triangleRightBaseHandle,
                _filletRadiusHandle,
                _rotationHandle
            });

            RenderGeometryGroup.Children.Add(_pathGeometry);
            RenderGeometryGroup.Children.Add(_topLeftHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_topRightHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_bottomLeftHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_bottomRightHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_triangleLeftBaseHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_triangleRightBaseHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_filletRadiusHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_rotationHandle.HandleGeometry);
            RenderGeometryGroup.Children.Add(_filletGeometry);
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
                    var (bottomRightStart, bottomRightEnd) =
                        LineHelper.GetPerpendicularLineThroughPoint(RectTopLeft, RectBottomLeft, point);
                    var (topRightStart, topRightEnd) =
                        LineHelper.GetPerpendicularLineThroughPoint(RectTopLeft, RectBottomLeft, RectTopLeft);
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
                            var rightDir = new Vector(RectTopRight.X - RectTopLeft.X, RectTopRight.Y - RectTopLeft.Y);
                            var downDir = new Vector(RectBottomLeft.X - RectTopLeft.X,
                                RectBottomLeft.Y - RectTopLeft.Y);

                            if (rightDir.Length > 0) rightDir.Normalize();
                            if (downDir.Length > 0) downDir.Normalize();

                            // Calculate the vector from the fixed corner to the mouse position
                            var toMouse = new Vector(point.X - RectBottomRight.X, point.Y - RectBottomRight.Y);

                            // Project the mouse vector onto the rectangle's own axes
                            var rightProj =
                                -Vector.Multiply(toMouse,
                                    rightDir); // Negative because we're moving left from bottom right
                            var downProj =
                                -Vector.Multiply(toMouse,
                                    downDir); // Negative because we're moving up from bottom right

                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            var rightOffset = Vector.Multiply(rightDir, rightProj);
                            var downOffset = Vector.Multiply(downDir, downProj);

                            // Calculate new corner positions
                            var newTopLeft = new Point(
                                RectBottomRight.X - rightOffset.X - downOffset.X,
                                RectBottomRight.Y - rightOffset.Y - downOffset.Y);

                            var newTopRight = new Point(
                                RectBottomRight.X - downOffset.X,
                                RectBottomRight.Y - downOffset.Y);

                            var newBottomLeft = new Point(
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
                            var leftDirTR = new Vector(RectTopLeft.X - RectTopRight.X, RectTopLeft.Y - RectTopRight.Y);
                            var downDirTR = new Vector(RectBottomRight.X - RectTopRight.X,
                                RectBottomRight.Y - RectTopRight.Y);

                            if (leftDirTR.Length > 0) leftDirTR.Normalize();
                            if (downDirTR.Length > 0) downDirTR.Normalize();

                            // Calculate the vector from the fixed corner to the mouse position
                            var toMouseTR = new Vector(point.X - RectBottomLeft.X, point.Y - RectBottomLeft.Y);

                            // Project the mouse vector onto the rectangle's own axes
                            var leftProjTR =
                                -Vector.Multiply(toMouseTR,
                                    leftDirTR); // Negative because we want the opposite direction
                            var downProjTR =
                                -Vector.Multiply(toMouseTR,
                                    downDirTR); // Negative because we're moving up from bottom left

                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            var leftOffsetTR = Vector.Multiply(leftDirTR, leftProjTR);
                            var downOffsetTR = Vector.Multiply(downDirTR, downProjTR);

                            // Calculate new corner positions
                            var newTopRightTR = new Point(
                                RectBottomLeft.X - leftOffsetTR.X - downOffsetTR.X,
                                RectBottomLeft.Y - leftOffsetTR.Y - downOffsetTR.Y);

                            var newTopLeftTR = new Point(
                                RectBottomLeft.X - downOffsetTR.X,
                                RectBottomLeft.Y - downOffsetTR.Y);

                            var newBottomRightTR = new Point(
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
                            var rightDirBR = new Vector(RectTopRight.X - RectTopLeft.X, RectTopRight.Y - RectTopLeft.Y);
                            var downDirBR = new Vector(RectBottomLeft.X - RectTopLeft.X,
                                RectBottomLeft.Y - RectTopLeft.Y);

                            if (rightDirBR.Length > 0) rightDirBR.Normalize();
                            if (downDirBR.Length > 0) downDirBR.Normalize();

                            // Calculate the vector from the fixed corner to the mouse position
                            var toMouseBR = new Vector(point.X - RectTopLeft.X, point.Y - RectTopLeft.Y);

                            // Project the mouse vector onto the rectangle's own axes
                            var rightProjBR = Vector.Multiply(toMouseBR, rightDirBR);
                            var downProjBR = Vector.Multiply(toMouseBR, downDirBR);

                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            var rightOffsetBR = Vector.Multiply(rightDirBR, rightProjBR);
                            var downOffsetBR = Vector.Multiply(downDirBR, downProjBR);

                            // Calculate new corner positions
                            var newBottomRightBR = new Point(
                                RectTopLeft.X + rightOffsetBR.X + downOffsetBR.X,
                                RectTopLeft.Y + rightOffsetBR.Y + downOffsetBR.Y);

                            var newTopRightBR = new Point(
                                RectTopLeft.X + rightOffsetBR.X,
                                RectTopLeft.Y + rightOffsetBR.Y);

                            var newBottomLeftBR = new Point(
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
                            var leftDirBL = new Vector(RectTopLeft.X - RectTopRight.X, RectTopLeft.Y - RectTopRight.Y);
                            var downDirBL = new Vector(RectBottomRight.X - RectTopRight.X,
                                RectBottomRight.Y - RectTopRight.Y);

                            if (leftDirBL.Length > 0) leftDirBL.Normalize();
                            if (downDirBL.Length > 0) downDirBL.Normalize();

                            // Calculate the vector from the fixed corner to the mouse position
                            var toMouseBL = new Vector(point.X - RectTopRight.X, point.Y - RectTopRight.Y);

                            // Project the mouse vector onto the rectangle's own axes
                            var leftProjBL = Vector.Multiply(toMouseBL, leftDirBL);
                            var downProjBL = Vector.Multiply(toMouseBL, downDirBL);

                            // Ensure we maintain a rectangle by using these projections along the rectangle's own axes
                            var leftOffsetBL = Vector.Multiply(leftDirBL, leftProjBL);
                            var downOffsetBL = Vector.Multiply(downDirBL, downProjBL);

                            // Calculate new corner positions
                            var newBottomLeftBL = new Point(
                                RectTopRight.X + leftOffsetBL.X + downOffsetBL.X,
                                RectTopRight.Y + leftOffsetBL.Y + downOffsetBL.Y);

                            var newTopLeftBL = new Point(
                                RectTopRight.X + leftOffsetBL.X,
                                RectTopRight.Y + leftOffsetBL.Y);

                            var newBottomRightBL = new Point(
                                RectTopRight.X + downOffsetBL.X,
                                RectTopRight.Y + downOffsetBL.Y);

                            // Update all corners except top right (which stays fixed)
                            RectBottomLeft = newBottomLeftBL;
                            RectTopLeft = newTopLeftBL;
                            RectBottomRight = newBottomRightBL;
                            break;
                        case 7: // Left triangle base handle
                        case 8: // Right triangle base handle
                            // Use the same approach as rectangle resizing: direct delta-based updates
                            if (OldPointForTranslate.HasValue)
                            {
                                // Calculate vertical movement delta
                                var deltaY = point.Y - OldPointForTranslate.Value.Y;

                                // Convert vertical movement to angle change (scale factor for responsiveness)
                                var angleChange = deltaY * 0.1; // Adjust this factor for desired sensitivity

                                // Update triangle angle directly
                                var newAngle = TriangleBottomEdgeAngleInDeg + angleChange;
                                TriangleBottomEdgeAngleInDeg = Math.Max(10, Math.Min(80, newAngle));

                                // Update the reference point for next delta calculation
                                OldPointForTranslate = point;
                            }

                            break;
                        case 9: // Fillet radius handle
                            // IMPROVED APPROACH: Project mouse movement along rectangle center line
                            if (OldPointForTranslate.HasValue)
                            {
                                // Calculate rectangle center line direction (from top center to bottom center)
                                var topCenter = new Point(
                                    (RectTopLeft.X + RectTopRight.X) / 2,
                                    (RectTopLeft.Y + RectTopRight.Y) / 2
                                );
                                var bottomCenter = new Point(
                                    (RectBottomLeft.X + RectBottomRight.X) / 2,
                                    (RectBottomLeft.Y + RectBottomRight.Y) / 2
                                );

                                // Center line direction should point AWAY from triangle apex for intuitive interaction
                                // Since triangle apex is at top center, direction should point from top to bottom
                                // BUT for intuitive radius control: moving away from apex = bigger radius
                                // So we want positive projection when moving away from apex
                                var centerLineDirection = topCenter - bottomCenter; // Reversed for intuitive control
                                if (centerLineDirection.Length > 0)
                                {
                                    centerLineDirection.Normalize();

                                    // Project mouse movement onto the rectangle center line
                                    var mouseMovement = point - OldPointForTranslate.Value;
                                    var projectionOnCenterLine = Vector.Multiply(mouseMovement, centerLineDirection);

                                    // Update radius - this will automatically move circle center via UpdateFilletCircle
                                    FilletRadius += -projectionOnCenterLine;
                                }

                                // Update the reference point for next delta calculation
                                OldPointForTranslate = point;
                            }

                            break;
                        case 10: // Rotation handle
                            // Rotate the entire geometry around rectangle center
                            if (OldPointForTranslate.HasValue)
                            {
                                // Calculate bottom edge center
                                var filletCenter = _filletGeometry.Center;

                                // Calculate rotation angle based on mouse movement
                                var oldVector = OldPointForTranslate.Value - filletCenter;
                                var newVector = point - filletCenter;

                                if (oldVector.Length > 0 && newVector.Length > 0)
                                {
                                    // Calculate angle between old and new vectors
                                    var oldAngle = Math.Atan2(oldVector.Y, oldVector.X);
                                    var newAngle = Math.Atan2(newVector.Y, newVector.X);
                                    var rotationAngle = newAngle - oldAngle;

                                    // Apply rotation to all rectangle corners
                                    RectTopLeft = RotatePointAroundCenter(RectTopLeft, filletCenter, rotationAngle);
                                    RectTopRight =
                                        RotatePointAroundCenter(RectTopRight, filletCenter, rotationAngle);
                                    RectBottomLeft = RotatePointAroundCenter(RectBottomLeft, filletCenter,
                                        rotationAngle);
                                    RectBottomRight = RotatePointAroundCenter(RectBottomRight, filletCenter,
                                        rotationAngle);
                                }

                                // Update the reference point for next delta calculation
                                OldPointForTranslate = point;
                            }

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
            if (!OldPointForTranslate.HasValue) return;

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


        /// <summary>
        ///     Calculates the intersection point between a triangle lateral edge and a rectangle edge
        /// </summary>
        /// <param name="apex">The apex point of the triangle</param>
        /// <param name="edgeStart">Start point of the rectangle edge</param>
        /// <param name="edgeEnd">End point of the rectangle edge</param>
        /// <param name="triangleAngleInDeg">Triangle angle in degrees (angle at apex between the two lateral edges)</param>
        /// <param name="isLeftEdge">True for left edge, false for right edge</param>
        /// <returns>The intersection point on the rectangle edge</returns>
        private Point GetTriangleEdgeIntersection(Point apex, Point edgeStart, Point edgeEnd, double triangleAngleInDeg,
            bool isLeftEdge)
        {
            // The triangle angle is the angle at the apex between the two lateral edges
            // Each lateral edge makes an angle of (triangleAngleInDeg / 2) with the vertical centerline
            var halfTriangleAngleRad = triangleAngleInDeg / 2.0 * Math.PI / 180.0;

            // Calculate the direction from apex to the rectangle edge
            // For left edge: direction is towards left and down
            // For right edge: direction is towards right and down
            var xDirection = isLeftEdge ? -Math.Sin(halfTriangleAngleRad) : Math.Sin(halfTriangleAngleRad);
            var yDirection = Math.Cos(halfTriangleAngleRad); // Always positive (downward)

            var lateralDirection = new Vector(xDirection, yDirection);

            // Rectangle edge direction vector
            var edgeDirection = edgeEnd - edgeStart;
            var edgeLength = edgeDirection.Length;
            if (edgeLength == 0) return edgeStart;

            // Find intersection using parametric equations
            // Rectangle edge: edgeStart + t * edgeDirection
            // Lateral edge: apex + s * lateralDirection
            // Solve: edgeStart + t * edgeDirection = apex + s * lateralDirection

            var denominator = edgeDirection.X * lateralDirection.Y - edgeDirection.Y * lateralDirection.X;

            if (Math.Abs(denominator) < 1e-10)
                // Lines are parallel or nearly parallel, return edge center as fallback
                return edgeStart + 0.5 * (edgeEnd - edgeStart);

            var apexToEdgeStart = edgeStart - apex;
            var t = (apexToEdgeStart.X * lateralDirection.Y - apexToEdgeStart.Y * lateralDirection.X) / denominator;

            // Clamp t to keep intersection within edge bounds [0, 1]
            t = Math.Max(0, Math.Min(1, t));

            // Calculate and return the intersection point
            return edgeStart + t * (edgeEnd - edgeStart);
        }

        /// <summary>
        ///     Given a reference line (line1: defined by two points), an angle in degrees, and a point on the second line,
        ///     returns the intersection point between line1 and the line passing through the given point at the specified angle.
        /// </summary>
        public static Point GetIntersectionPoint_Line1AnglePoint(
            Point line1Point1, Point line1Point2, Point pointOnLine2, double angleInDegrees)
        {
            var dx1 = line1Point2.X - line1Point1.X;
            var dy1 = line1Point2.Y - line1Point1.Y;
            var alpha = Math.Atan2(dy1, dx1);

            var thetaRad = angleInDegrees * Math.PI / 180.0;
            var beta = alpha + thetaRad;

            var dx2 = Math.Cos(beta);
            var dy2 = Math.Sin(beta);

            // Set up the linear system:
            // [ dx1  -dx2 ] [t1] = [pointOnLine2.X - line1Point1.X]
            // [ dy1  -dy2 ] [t2]   [pointOnLine2.Y - line1Point1.Y]
            var det = dx1 * -dy2 - dy1 * -dx2;
            if (Math.Abs(det) < 1e-10)
                return line1Point1; // Lines are parallel; return start of line1 as fallback

            var rhsX = pointOnLine2.X - line1Point1.X;
            var rhsY = pointOnLine2.Y - line1Point1.Y;

            var t1 = (rhsX * -dy2 - rhsY * -dx2) / det;
            // double t2 = (dx1 * rhsY - dy1 * rhsX) / det; // Not needed unless you want the parameter along line2

            // CRITICAL FIX: Constrain t1 to keep intersection within the rectangle edge bounds
            // t1 = 0 means intersection at line1Point1, t1 = 1 means intersection at line1Point2
            // Values outside [0,1] mean intersection is outside the rectangle edge
            t1 = Math.Max(0.0, Math.Min(1.0, t1));

            // Intersection point (now guaranteed to be on the rectangle edge)
            return new Point(
                line1Point1.X + t1 * dx1,
                line1Point1.Y + t1 * dy1
            );
        }


        private void UpdateGeometryOnMouseMove()
        {
            //update the geometry
            UpdateGeometry();
        }

        /// <summary>
        ///     Calculates the triangle bottom edge angle based on the position of a triangle base handle
        ///     Projects the mouse position onto the rectangle edge for intuitive interaction
        /// </summary>
        /// <param name="point">The current mouse position</param>
        /// <param name="isLeftHandle">True if this is the left base handle, false for right</param>
        /// <returns>The calculated triangle bottom edge angle in degrees</returns>
        private double CalculateTriangleAngleFromBasePoint(Point point, bool isLeftHandle)
        {
            // Get the appropriate rectangle edge
            var edgeStart = isLeftHandle ? RectTopLeft : RectTopRight;
            var edgeEnd = isLeftHandle ? RectBottomLeft : RectBottomRight;

            // Project the mouse point onto the rectangle edge
            var edgeVector = edgeEnd - edgeStart;
            var toMouse = point - edgeStart;

            var edgeLength = edgeVector.Length;
            if (edgeLength == 0) return TriangleBottomEdgeAngleInDeg; // Return current angle if edge has no length

            // Calculate the projection parameter t
            var t = Vector.Multiply(toMouse, edgeVector) / (edgeLength * edgeLength);

            // Ensure the handle stays within the rectangle edge bounds
            // Clamp t to stay within the edge, with small margins to avoid extreme angles
            t = Math.Max(0.1, Math.Min(0.9, t));

            // Get the projected point on the rectangle edge (this ensures handle stays on edge)
            var projectedPoint = edgeStart + t * edgeVector;

            // Calculate the vector from triangle apex to the projected point
            var apexToProjected = projectedPoint - TriangleApex;

            // Calculate the horizontal distance from apex to the rectangle edge
            var horizontalDistance =
                Math.Abs(isLeftHandle ? RectTopLeft.X - TriangleApex.X : RectTopRight.X - TriangleApex.X);

            // Calculate the vertical distance from apex to the projected point
            var verticalDistance = Math.Abs(projectedPoint.Y - TriangleApex.Y);

            // Simple approach: use the projection parameter t directly for smooth, predictable behavior
            // This avoids the sensitivity issues with trigonometric calculations near the apex

            // The parameter t represents position along the rectangle edge:
            // t = 0.1 (near top) should give small angle (10°)
            // t = 0.9 (near bottom) should give large angle (80°)

            // Linear interpolation from t to angle - this is smooth and predictable
            var normalizedT = Math.Max(0.1, Math.Min(0.9, t));
            var angleFromT = 10 + (normalizedT - 0.1) * (80 - 10) / (0.9 - 0.1);

            return Math.Max(10, Math.Min(80, angleFromT));
        }



        /// <summary>
        ///     Updates the fillet circle geometry to be tangent to both lateral edges of the triangle
        /// </summary>
        private void UpdateFilletCircle()
        {
            if (FilletRadius <= 0)
            {
                // Hide the circle if radius is zero or negative
                _filletGeometry.Center = new Point(0, 0);
                _filletGeometry.RadiusX = 0;
                _filletGeometry.RadiusY = 0;
                return;
            }

            // Calculate half of the triangle angle in radians
            var halfTriangleAngleRad = (180 - 2 * TriangleBottomEdgeAngleInDeg) * Math.PI / 2 / 180.0;

            // For a circle tangent to both lateral edges, the center lies on the angle bisector
            // The distance from apex to circle center is: radius / sin(halfAngle)
            var distanceFromApex = FilletRadius / Math.Sin(halfTriangleAngleRad);

            // Calculate the direction vectors of both lateral edges
            // We need to get the actual intersection points first
            var lateralToVerticalAngle = 90.0 - TriangleBottomEdgeAngleInDeg;
            var leftEdgePoint =
                GetIntersectionPoint_Line1AnglePoint(RectTopLeft, RectBottomLeft, TriangleApex, lateralToVerticalAngle);
            var rightEdgePoint = GetIntersectionPoint_Line1AnglePoint(RectTopRight, RectBottomRight, TriangleApex,
                -lateralToVerticalAngle);

            // Calculate direction vectors from apex to each base point
            var leftEdgeDirection = new Vector(leftEdgePoint.X - TriangleApex.X, leftEdgePoint.Y - TriangleApex.Y);
            var rightEdgeDirection = new Vector(rightEdgePoint.X - TriangleApex.X, rightEdgePoint.Y - TriangleApex.Y);

            // Normalize the direction vectors
            leftEdgeDirection.Normalize();
            rightEdgeDirection.Normalize();

            // Calculate the angle bisector direction (average of the two normalized directions)
            var bisectorDirection = new Vector(
                (leftEdgeDirection.X + rightEdgeDirection.X) / 2.0,
                (leftEdgeDirection.Y + rightEdgeDirection.Y) / 2.0
            );
            bisectorDirection.Normalize();

            // The circle center is positioned along the angle bisector from the apex
            var circleCenter = new Point(
                TriangleApex.X + bisectorDirection.X * distanceFromApex,
                TriangleApex.Y + bisectorDirection.Y * distanceFromApex
            );

            // Update the ellipse geometry
            _filletGeometry.Center = circleCenter;
            _filletGeometry.RadiusX = FilletRadius;
            _filletGeometry.RadiusY = FilletRadius;

            // Position the fillet radius handle along the angle bisector
            // Place it on the circle edge for intuitive radius adjustment
            var handlePosition = new Point(
                circleCenter.X + bisectorDirection.X * FilletRadius,
                circleCenter.Y + bisectorDirection.Y * FilletRadius
            );
            _filletRadiusHandle.GeometryCenter = handlePosition;
        }

        protected override void OnStrokeThicknessChanges(double strokeThickness)
        {
            // Update the stroke thickness for all geometries to maintain uniform appearance when zooming
            // This method is called when the zoom level changes

            // We don't need to do anything special here since the base class should handle
            // the stroke thickness changes for the geometries in RenderGeometryGroup
        }

        /// <summary>
        ///     Rotates a point around a center point by the given angle in radians
        /// </summary>
        private Point RotatePointAroundCenter(Point point, Point center, double angleInRadians)
        {
            // Translate point to origin
            var x = point.X - center.X;
            var y = point.Y - center.Y;

            // Apply rotation matrix
            var cosAngle = Math.Cos(angleInRadians);
            var sinAngle = Math.Sin(angleInRadians);

            var rotatedX = x * cosAngle - y * sinAngle;
            var rotatedY = x * sinAngle + y * cosAngle;

            // Translate back to original position
            return new Point(rotatedX + center.X, rotatedY + center.Y);
        }

        /// <summary>
        ///     Override UpdateVisual to render only edges/strokes without fill
        ///     Only drag handles should have filled background for proper mouse hit testing
        /// </summary>
        public override void UpdateVisual()
        {
            if (ShapeStyler == null) return;

            var renderContext = RenderOpen();

            // SOLUTION: Render invisible filled geometry for hit testing (translation)
            // This allows the whole shape to be clickable for translation while appearing stroke-only
            renderContext.DrawGeometry(Brushes.Transparent, null, RenderGeometry);

            // Render the main geometry (rectangle + triangle) with stroke only, no fill
            renderContext.DrawGeometry(null, ShapeStyler.SketchPen, RenderGeometry);

            // Render the fillet circle with stroke only, no fill
            if (FilletRadius > 0)
            {
                // Invisible fill for hit testing
                renderContext.DrawGeometry(Brushes.Transparent, null, _filletGeometry);
                // Visible stroke
                renderContext.DrawGeometry(null, ShapeStyler.SketchPen, _filletGeometry);
            }

            // IMPORTANT: Always render drag handles with filled background for mouse events
            // This ensures proper hit testing regardless of selection state
            foreach (var handle in Handles)
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, handle.HandleGeometry);

            DrawAnnotationText(renderContext);

            renderContext.Close();
        }


        private double GetAngleBetweenLinesInDeg(Point line1Point1, Point line1Point2, Point line2Point1,
            Point line2Point2)
        {
            var line1Direction = new Vector(line1Point2.X - line1Point1.X, line1Point2.Y - line1Point1.Y);
            var line2Direction = new Vector(line2Point2.X - line2Point1.X, line2Point2.Y - line2Point1.Y);
            return GetAngleBetweenVectors(line1Direction, line2Direction) * 180.0 / Math.PI;
        }


        private double GetAngleBetweenVectors(Vector a, Vector b)
        {
            var dot = a.X * b.X + a.Y * b.Y;
            var normA = Math.Sqrt(a.X * a.X + a.Y * a.Y);
            var normB = Math.Sqrt(b.X * b.X + b.Y * b.Y);

            if (normA == 0 || normB == 0)
                throw new InvalidOperationException("Zero-length vector");

            var cosTheta = dot / (normA * normB);
            cosTheta = Math.Max(-1.0, Math.Min(1.0, cosTheta)); // Clamp for safety

            return Math.Acos(cosTheta);
        }

        private void DrawAnnotationText(DrawingContext renderContext)
        {
            DrawCircleText(renderContext);

            DrawRectText(renderContext);
        }

        private void DrawCircleText(DrawingContext renderContext)
        {
            // Draw the length text
            var radius = FilletRadius;
            var radiusInMm = 0.0;
            if (ShapeLayer.UnitsPerMillimeter != 0 && ShapeLayer.PixelPerUnit != 0)
                radiusInMm = radius * ShapeLayer.UnitsPerMillimeter / ShapeLayer.PixelPerUnit;

            var radiusText = new FormattedText(
                $"radius: {radiusInMm:f4} {ShapeLayer.UnitName}, {radius:f4} px",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            var angleText = new FormattedText(
                $"tip angle: {TriangleBottomEdgeAngleInDeg:f4}°",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);


            var rectTopEdgeLength = Math.Sqrt(Math.Pow(RectTopLeft.X - RectTopRight.X, 2) +
                                              Math.Pow(RectTopLeft.Y - RectTopRight.Y, 2));
            var rectTopEdgeLengthInMm = 0.0;
            if (ShapeLayer.UnitsPerMillimeter != 0 && ShapeLayer.PixelPerUnit != 0)
                rectTopEdgeLengthInMm = rectTopEdgeLength * ShapeLayer.UnitsPerMillimeter / ShapeLayer.PixelPerUnit;
            var rectTopEdgeLengthText = new FormattedText(
                $"width: {rectTopEdgeLengthInMm:f4} {ShapeLayer.UnitName}, {rectTopEdgeLength:f4} px",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            // Calculate the angle of the rectangle's top edge
            var topEdgeVector = RectTopRight - RectTopLeft;
            var topEdgeAngle = Math.Atan2(topEdgeVector.Y, topEdgeVector.X) * 180.0 / Math.PI;

            // Position text aligned with rectangle top edge
            var xPosition = (RectTopLeft.X + RectTopRight.X) / 2 - 30;
            var yPosition =
                (RectTopLeft.Y + RectTopRight.Y) / 2 - radiusText.Height; // Above top edge with small margin

            // Apply rotation transform to make text parallel to top edge
            renderContext.PushTransform(new RotateTransform(topEdgeAngle, xPosition, yPosition));
            renderContext.DrawText(radiusText, new Point(xPosition, yPosition));
            renderContext.Pop(); // Remove the rotation transform

            // Position text aligned with rectangle top edge
            xPosition = (RectTopLeft.X + RectTopRight.X) / 2 - 30;
            yPosition = (RectTopLeft.Y + RectTopRight.Y) / 2 - radiusText.Height -
                        angleText.Height; // Above top edge with small margin

            // Apply rotation transform to make text parallel to top edge
            renderContext.PushTransform(new RotateTransform(topEdgeAngle, xPosition, yPosition));
            renderContext.DrawText(angleText, new Point(xPosition, yPosition));
            renderContext.Pop(); // Remove the rotation transform

            // Position text aligned with rectangle top edge
            xPosition = (RectTopLeft.X + RectTopRight.X) / 2 - 30;
            yPosition = (RectTopLeft.Y + RectTopRight.Y) / 2 - radiusText.Height - angleText.Height -
                        rectTopEdgeLengthText.Height; // Above top edge with small margin

            // Apply rotation transform to make text parallel to top edge
            renderContext.PushTransform(new RotateTransform(topEdgeAngle, xPosition, yPosition));
            renderContext.DrawText(rectTopEdgeLengthText, new Point(xPosition, yPosition));
            renderContext.Pop(); // Remove the rotation transform
        }

        private void DrawRectText(DrawingContext renderContext)
        {
            var angleTextHorizontal = new FormattedText(
                $"{GetRectangleAngleWithHorizontal():f4}°",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            var leftEdgeLength = Math.Sqrt(Math.Pow(RectTopLeft.X - RectBottomLeft.X, 2) +
                                           Math.Pow(RectTopLeft.Y - RectBottomLeft.Y, 2));
            var leftEdgeLengthInMm = 0.0;
            if (ShapeLayer.UnitsPerMillimeter != 0 && ShapeLayer.PixelPerUnit != 0)
                leftEdgeLengthInMm = leftEdgeLength * ShapeLayer.UnitsPerMillimeter / ShapeLayer.PixelPerUnit;
            var leftEdgeLengthText = new FormattedText(
                $"height: {leftEdgeLengthInMm:f4} {ShapeLayer.UnitName}, {leftEdgeLength:f4} px",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                ShapeLayer.TagFontSize,
                Brushes.Red,
                96);

            var leftEdgeVector = RectTopLeft - RectBottomLeft;
            var leftEdgeAngle = Math.Atan2(leftEdgeVector.Y, leftEdgeVector.X) * 180.0 / Math.PI;

            // Position text aligned with rectangle left edge
            var xPosition = (RectTopLeft.X + RectBottomLeft.X) / 2;
            var yPosition = (RectTopLeft.Y + RectBottomLeft.Y) / 2; // Above top edge with small margin

            // Apply rotation transform to make text parallel to top edge
            renderContext.PushTransform(new RotateTransform(leftEdgeAngle, xPosition, yPosition));
            renderContext.DrawText(angleTextHorizontal, new Point(xPosition, yPosition));
            renderContext.Pop(); // Remove the rotation transform

            renderContext.PushTransform(new RotateTransform(leftEdgeAngle, xPosition, yPosition));
            renderContext.DrawText(leftEdgeLengthText, new Point(xPosition, yPosition - leftEdgeLengthText.Height));
            renderContext.Pop(); // Remove the rotation transform
        }

        private double GetRectangleAngleWithHorizontal()
        {
            var topEdgeVector = RectBottomLeft - RectTopLeft;
            var topEdgeAngle = Math.Atan2(topEdgeVector.Y, topEdgeVector.X) * 180.0 / Math.PI;
            return topEdgeAngle;
        }

        #endregion
    }
}