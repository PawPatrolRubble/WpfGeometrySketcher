using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Handle;
using Lan.Shapes.Shapes;
using netDxf;
using netDxf.Entities;
using Point = System.Windows.Point;

namespace Lan.Shapes.DialogGeometry
{
    public class DxfGeometry : ShapeVisualBase
    {

        private Geometry? _geometry;
        private GeometryGroup? _dxfGeometryWrapper;

        public DxfGeometry(ShapeLayer layer) : base(layer)
        {

        }

        #region Overrides of ShapeVisualBase

        public override Rect BoundsRect { get => _dxfGeometryWrapper?.Bounds ?? new Rect(); }

        protected override void CreateHandles()
        {
            // handles are creating during ReadDxfFile
        }

        private void UpdateHandleLocation()
        {
            var bounds = BoundsRect;
            if (bounds.IsEmpty) return;

            foreach (var h in Handles)
            {
                if (h.Id == (int)DragLocation.TopLeft) h.GeometryCenter = bounds.TopLeft;
                else if (h.Id == (int)DragLocation.TopRight) h.GeometryCenter = bounds.TopRight;
                else if (h.Id == (int)DragLocation.BottomRight) h.GeometryCenter = bounds.BottomRight;
                else if (h.Id == (int)DragLocation.BottomLeft) h.GeometryCenter = bounds.BottomLeft;
                else if (h.Id == 100) h.GeometryCenter = new Point((bounds.Left + bounds.Right) / 2, bounds.Top - 30);
            }
        }

        protected override void HandleResizing(Point point)
        {
            if (SelectedDragHandle == null || _dxfGeometryWrapper == null) return;

            var oldBounds = BoundsRect;
            if (oldBounds.IsEmpty || oldBounds.Width == 0 || oldBounds.Height == 0) return;

            if (SelectedDragHandle.Id == 100) // Rotation handle
            {
                var center = new Point((oldBounds.Left + oldBounds.Right) / 2, (oldBounds.Top + oldBounds.Bottom) / 2);
                if (OldPointForTranslate.HasValue)
                {
                    Vector vOld = OldPointForTranslate.Value - center;
                    Vector vNew = point - center;

                    double angleOld = Math.Atan2(vOld.Y, vOld.X) * 180 / Math.PI;
                    double angleNew = Math.Atan2(vNew.Y, vNew.X) * 180 / Math.PI;
                    double angleDelta = angleNew - angleOld;

                    var rotateTransform = new RotateTransform(angleDelta, center.X, center.Y);

                    if (_dxfGeometryWrapper.Transform is TransformGroup rotGroup)
                    {
                        rotGroup.Children.Add(rotateTransform);
                    }
                    else
                    {
                        var group = new TransformGroup();
                        if (_dxfGeometryWrapper.Transform != null)
                        {
                            group.Children.Add(_dxfGeometryWrapper.Transform);
                        }
                        group.Children.Add(rotateTransform);
                        _dxfGeometryWrapper.Transform = group;
                    }
                }
                OldPointForTranslate = point;
                UpdateHandleLocation();
                return;
            }

            double centerX = oldBounds.Left;
            double centerY = oldBounds.Top;
            double handleX = oldBounds.Right;
            double handleY = oldBounds.Bottom;

            if (SelectedDragHandle.Id == (int)DragLocation.TopLeft)
            {
                centerX = oldBounds.Right;
                centerY = oldBounds.Bottom;
                handleX = oldBounds.Left;
                handleY = oldBounds.Top;
            }
            else if (SelectedDragHandle.Id == (int)DragLocation.TopRight)
            {
                centerX = oldBounds.Left;
                centerY = oldBounds.Bottom;
                handleX = oldBounds.Right;
                handleY = oldBounds.Top;
            }
            else if (SelectedDragHandle.Id == (int)DragLocation.BottomRight)
            {
                centerX = oldBounds.Left;
                centerY = oldBounds.Top;
                handleX = oldBounds.Right;
                handleY = oldBounds.Bottom;
            }
            else if (SelectedDragHandle.Id == (int)DragLocation.BottomLeft)
            {
                centerX = oldBounds.Right;
                centerY = oldBounds.Top;
                handleX = oldBounds.Left;
                handleY = oldBounds.Bottom;
            }

            Vector diagonal = new Vector(handleX - centerX, handleY - centerY);
            Vector mouseVec = new Vector(point.X - centerX, point.Y - centerY);

            // Calculate uniform scale by projecting the mouse movement along the diagonal
            double scale = (mouseVec * diagonal) / (diagonal * diagonal);

            // Prevent the scale from becoming too small, inverting, or effectively flattening
            if (scale < 0.05) scale = 0.05;

            var scaleTransform = new ScaleTransform(scale, scale, centerX, centerY);

            if (_dxfGeometryWrapper.Transform is TransformGroup tg)
            {
                tg.Children.Add(scaleTransform);
            }
            else
            {
                var group = new TransformGroup();
                if (_dxfGeometryWrapper.Transform != null)
                {
                    group.Children.Add(_dxfGeometryWrapper.Transform);
                }
                group.Children.Add(scaleTransform);
                _dxfGeometryWrapper.Transform = group;
            }

            UpdateHandleLocation();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue && IsGeometryRendered)
            {
                SetMouseCursorToHand();
                Vector delta = newPoint - OldPointForTranslate.Value;

                // 1. Translate the main DXF wrapper geometry
                if (_dxfGeometryWrapper != null)
                {
                    if (_dxfGeometryWrapper.Transform is TranslateTransform tt)
                    {
                        tt.X += delta.X;
                        tt.Y += delta.Y;
                    }
                    else if (_dxfGeometryWrapper.Transform == null || _dxfGeometryWrapper.Transform == Transform.Identity)
                    {
                        _dxfGeometryWrapper.Transform = new TranslateTransform(delta.X, delta.Y);
                    }
                    else if (_dxfGeometryWrapper.Transform is TransformGroup tg)
                    {
                        tg.Children.Add(new TranslateTransform(delta.X, delta.Y));
                    }
                    else
                    {
                        var group = new TransformGroup();
                        group.Children.Add(_dxfGeometryWrapper.Transform);
                        group.Children.Add(new TranslateTransform(delta.X, delta.Y));
                        _dxfGeometryWrapper.Transform = group;
                    }
                }

                // 2. Translate any handles attached
                foreach (var h in Handles)
                {
                    h.GeometryCenter += delta;
                }

                OldPointForTranslate = newPoint;
            }
        }

        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }

        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        public override void OnMouseRightButtonUp(Point mousePosition)
        {
            base.OnMouseRightButtonUp(mousePosition);

            if (IsGeometryRendered)
            {
                var contextMenu = new ContextMenu();

                var exportItem = new MenuItem { Header = "Export to DXF..." };
                exportItem.Click += (s, e) =>
                {
                    var dialog = new DialogService();
                    dialog.ShowDialog<DxfExportDialog, DxfExportDialogViewModel>(() => new DxfExportDialogViewModel(), x =>
                    {
                        if (x.Result == DialogResult.Ok)
                        {
                            var doc = ExportToDxf(new Point(x.TopLeftX, x.TopLeftY), x.PixelToMmFactor);

                            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                            saveFileDialog.Filter = "DXF files (*.dxf)|*.dxf|All files (*.*)|*.*";
                            saveFileDialog.DefaultExt = "dxf";
                            saveFileDialog.AddExtension = true;
                            saveFileDialog.FileName = "exported_sketch.dxf";

                            if (saveFileDialog.ShowDialog() == true)
                            {
                                doc.Save(saveFileDialog.FileName);
                                MessageBox.Show("Export to DXF completed!");
                            }
                        }
                    });
                };
                contextMenu.Items.Add(exportItem);

                var anotherItem = new MenuItem { Header = "Delete Geometry" };
                anotherItem.Click += (s, e) =>
                {
                    // Handle deletion
                };
                contextMenu.Items.Add(anotherItem);

                contextMenu.IsOpen = true;
            }
        }


        public override void UpdateVisual()
        {
            if (ShapeStyler == null)
            {
                return;
            }

            var renderContext = RenderOpen();

            // Draw a background rectangle using the shape's bounds to make the inner area clickable
            var bounds = BoundsRect;
            if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height > 0)
            {
                // Use a transparent or very light fill color so it's clickable
                Brush backgroundBrush = Brushes.Transparent;
                if (ShapeStyler.FillColor != null)
                {
                    var tint = ShapeStyler.FillColor.CloneCurrentValue();
                    tint.Opacity = 0.1; // Slight tint if there is a fill color
                    backgroundBrush = tint;
                }

                renderContext.DrawRectangle(backgroundBrush, null, bounds);
            }

            renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, RenderGeometry);

            if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height > 0 && ShapeStyler.SketchPen != null)
            {
                var topCenter = new Point((bounds.Left + bounds.Right) / 2, bounds.Top);
                var handleCenter = new Point((bounds.Left + bounds.Right) / 2, bounds.Top - 30);
                renderContext.DrawLine(ShapeStyler.SketchPen, topCenter, handleCenter);
            }

            foreach (var h in Handles)
            {
                if (h.HandleGeometry != null)
                {
                    renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, h.HandleGeometry);
                }
            }
            renderContext.Close();
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (IsGeometryRendered && buttonState == MouseButtonState.Pressed)
            {
                if (SelectedDragHandle != null)
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleResizing(point);
                    UpdateGeometryGroup();
                    UpdateVisual();
                }
                else
                {
                    IsBeingDraggedOrPanMoving = true;
                    HandleTranslate(point);
                    UpdateGeometryGroup();
                    UpdateVisual();
                }
            }
            else
            {
                base.OnMouseMove(point, buttonState);
            }
        }

        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            base.OnMouseLeftButtonUp(newPoint);

            BakeTransform();
            OldPointForTranslate = null;
            SelectedDragHandle = null;
            IsBeingDraggedOrPanMoving = false;
        }

        private void BakeTransform()
        {
            if (_dxfGeometryWrapper == null || _dxfGeometryWrapper.Transform == null || _dxfGeometryWrapper.Transform.Value.IsIdentity)
                return;

            if (!(_geometry is PathGeometry originalPathGeo))
                return;

            var matrix = _dxfGeometryWrapper.Transform.Value;

            // Calculate uniform scale and rotation from the standard affine 2D matrix
            double scale = Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M21 * matrix.M21);
            double rotation = Math.Atan2(matrix.M21, matrix.M11) * 180 / Math.PI;

            var bakedGeometry = new PathGeometry();
            bakedGeometry.FillRule = originalPathGeo.FillRule;

            foreach (var figure in originalPathGeo.Figures)
            {
                var newFigure = new PathFigure
                {
                    StartPoint = matrix.Transform(figure.StartPoint),
                    IsClosed = figure.IsClosed,
                    IsFilled = figure.IsFilled
                };

                foreach (var segment in figure.Segments)
                {
                    if (segment is LineSegment ls)
                    {
                        newFigure.Segments.Add(new LineSegment(matrix.Transform(ls.Point), ls.IsStroked));
                    }
                    else if (segment is ArcSegment arc)
                    {
                        newFigure.Segments.Add(new ArcSegment(
                            matrix.Transform(arc.Point),
                            new Size(arc.Size.Width * scale, arc.Size.Height * scale),
                            arc.RotationAngle + rotation,
                            arc.IsLargeArc,
                            arc.SweepDirection,
                            arc.IsStroked));
                    }
                    else if (segment is BezierSegment bezier)
                    {
                        newFigure.Segments.Add(new BezierSegment(
                            matrix.Transform(bezier.Point1),
                            matrix.Transform(bezier.Point2),
                            matrix.Transform(bezier.Point3),
                            bezier.IsStroked));
                    }
                    else if (segment is PolyLineSegment pls)
                    {
                        var newPoints = new PointCollection();
                        foreach (var p in pls.Points)
                            newPoints.Add(matrix.Transform(p));
                        newFigure.Segments.Add(new PolyLineSegment(newPoints, pls.IsStroked));
                    }
                }
                bakedGeometry.Figures.Add(newFigure);
            }

            bakedGeometry.Freeze();

            _geometry = bakedGeometry;
            _dxfGeometryWrapper.Children.Clear();
            _dxfGeometryWrapper.Children.Add(_geometry);
            _dxfGeometryWrapper.Transform = Transform.Identity;

            UpdateHandleLocation();
            UpdateVisual();
        }

        public override void OnMouseLeftButtonDown(System.Windows.Point mousePoint)
        {
            base.OnMouseLeftButtonDown(mousePoint);
            if (!IsGeometryRendered)
            {
                var filePicker = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "DXF Files (*.dxf)|*.dxf|All Files (*.*)|*.*"
                };

                if (filePicker.ShowDialog() == true)
                {
                    string filePath = filePicker.FileName;
                    ReadDxfFile(filePath, mousePoint);
                    //RenderGeometryGroup.Children.Add(new RectangleGeometry(new Rect(mousePoint, new Size(200,200))));

                    UpdateVisual();
                    IsGeometryRendered = true;
                }
            }
            else
            {
                foreach (var dragHandle in Handles)
                {
                    if (dragHandle.FillContains(mousePoint))
                    {
                        SelectedDragHandle = dragHandle;
                        break;
                    }
                }
            }
        }

        private void ReadDxfFile(string filePath, Point offset)
        {

            var doc = DxfDocument.Load(filePath);
            // Implement the logic to read a DXF file and extract geometry data
            // You can use a library like netDxf to simplify this process
            _geometry = DxfRenderer.BuildGeometry(doc, 1, offset);

            _dxfGeometryWrapper = new GeometryGroup();
            if (_geometry != null)
            {
                _dxfGeometryWrapper.Children.Add(_geometry);
            }

            var dragHandleSize = ShapeStyler?.DragHandleSize ?? 10;
            var bounds = BoundsRect;

            Handles.Add(new RectDragHandle(dragHandleSize, bounds.TopLeft, (int)DragLocation.TopLeft));
            Handles.Add(new RectDragHandle(dragHandleSize, bounds.TopRight, (int)DragLocation.TopRight));
            Handles.Add(new RectDragHandle(dragHandleSize, bounds.BottomRight, (int)DragLocation.BottomRight));
            Handles.Add(new RectDragHandle(dragHandleSize, bounds.BottomLeft, (int)DragLocation.BottomLeft));

            // Add a rotation handle above the center
            Handles.Add(new RectDragHandle(dragHandleSize, new Point((bounds.Left + bounds.Right) / 2, bounds.Top - 30), 100));

            // foreach (var h in Handles)
            // {
            //     if (h.HandleGeometry != null)
            //     {
            //         RenderGeometryGroup.Children.Add(h.HandleGeometry);
            //     }
            // }

            RenderGeometryGroup.Children.Add(_dxfGeometryWrapper);
        }

        private (double MinX, double MinY, double MaxX, double MaxY) CalcBounds(DxfDocument doc)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var e in doc.Entities.All)
            {
                switch (e)
                {
                    case netDxf.Entities.Line line:
                        Expand(ref minX, ref minY, ref maxX, ref maxY, line.StartPoint);
                        Expand(ref minX, ref minY, ref maxX, ref maxY, line.EndPoint);
                        break;
                    case Arc arc:
                        Expand(ref minX, ref minY, ref maxX, ref maxY,
                            arc.Center.X - arc.Radius, arc.Center.Y - arc.Radius,
                            arc.Center.X + arc.Radius, arc.Center.Y + arc.Radius);
                        break;
                    case netDxf.Entities.Circle circle:
                        Expand(ref minX, ref minY, ref maxX, ref maxY,
                            circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius,
                            circle.Center.X + circle.Radius, circle.Center.Y + circle.Radius);
                        break;
                    case Polyline2D poly:
                        foreach (var v in poly.Vertexes)
                            Expand(ref minX, ref minY, ref maxX, ref maxY, v.Position.X, v.Position.Y);
                        break;
                    case Polyline3D poly3d:
                        foreach (var v in poly3d.Vertexes)
                            Expand(ref minX, ref minY, ref maxX, ref maxY, v);
                        break;
                    case Spline spline:
                        foreach (var cp in spline.ControlPoints)
                            Expand(ref minX, ref minY, ref maxX, ref maxY, cp);
                        break;
                }
            }

            if (minX == double.MaxValue)
            {
                return (0, 0, 100, 100);
            }

            return (minX, minY, maxX, maxY);
        }

        static void Expand(ref double minX, ref double minY, ref double maxX, ref double maxY, Vector3 pt)
        {
            if (pt.X < minX) minX = pt.X;
            if (pt.Y < minY) minY = pt.Y;
            if (pt.X > maxX) maxX = pt.X;
            if (pt.Y > maxY) maxY = pt.Y;
        }

        static void Expand(ref double minX, ref double minY, ref double maxX, ref double maxY,
            double x1, double y1, double x2, double y2)
        {
            if (x1 < minX) minX = x1;
            if (y1 < minY) minY = y1;
            if (x2 > maxX) maxX = x2;
            if (y2 > maxY) maxY = y2;
        }

        static void Expand(ref double minX, ref double minY, ref double maxX, ref double maxY, double x, double y)
        {
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
        }

        public DxfDocument ExportToDxf(Point sketchBoardTopLeftRealWorld, double pixelToMmFactor)
        {
            BakeTransform();

            var doc = new DxfDocument();

            if (_geometry is PathGeometry pathGeo)
            {
                // WPF's GetFlattenedPathGeometry natively drops all figures where IsFilled=false!
                // We securely clone the geometry and temporarily force IsFilled=true to guarantee full point data retrieval.
                var cloneGeo = pathGeo.Clone();
                foreach (var figure in cloneGeo.Figures)
                {
                    figure.IsFilled = true;
                }

                // We use Flatten to universally convert complex Bézier and arc curves correctly back into highly 
                // precise linear Polyline2D entities suitable for standard CAD and CNC tools
                var flatGeo = cloneGeo.GetFlattenedPathGeometry(0.01, ToleranceType.Absolute);

                foreach (var figure in flatGeo.Figures)
                {
                    var vertices = new List<netDxf.Vector2>();

                    var startPos = ToDxfPoint(figure.StartPoint, sketchBoardTopLeftRealWorld, pixelToMmFactor);
                    vertices.Add(startPos);

                    foreach (var segment in figure.Segments)
                    {
                        if (segment is LineSegment ls)
                        {
                            vertices.Add(ToDxfPoint(ls.Point, sketchBoardTopLeftRealWorld, pixelToMmFactor));
                        }
                        else if (segment is PolyLineSegment pls)
                        {
                            foreach (var p in pls.Points)
                            {
                                vertices.Add(ToDxfPoint(p, sketchBoardTopLeftRealWorld, pixelToMmFactor));
                            }
                        }
                    }

                    var poly = new Polyline2D(vertices, figure.IsClosed);
                    doc.Entities.Add(poly);
                }
            }

            return doc;
        }

        private netDxf.Vector2 ToDxfPoint(Point p, Point topLeftRealWorld, double pixelToMm)
        {
            return new netDxf.Vector2(
                topLeftRealWorld.X + p.X / pixelToMm,

                // Flip the Y-axis back because WPF rendering coordinate Y goes down, but DXF Y goes up
                topLeftRealWorld.Y - p.Y / pixelToMm
            );
        }

        #endregion
    }
}
