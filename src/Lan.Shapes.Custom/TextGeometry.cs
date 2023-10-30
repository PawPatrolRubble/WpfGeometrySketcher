using Lan.Shapes.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Shapes;
using System.Windows.Shapes;
using System.Xml;
using Path = System.IO.Path;

namespace Lan.Shapes.Custom
{
    public class TextGeometry : ShapeVisualBase, IDataExport<TextGeometryData>
    {
        private TextGeometryData? _textGeometryData;
        private Geometry _geometry;
        public TextGeometry(ShapeLayer shapeLayer) : base(shapeLayer)
        {

        }

        public void FromData(TextGeometryData data)
        {
            _textGeometryData = data;
            IsGeometryRendered = true;
            UpdateVisual();
        }

        public new TextGeometryData GetMetaData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get => _geometry.Bounds; }

        protected override void CreateHandles()
        {

        }

        protected override void HandleResizing(Point point)
        {

        }

        protected override void HandleTranslate(Point newPoint)
        {

        }

        /// <summary>
        /// add geometries to group
        /// </summary>
        protected override void UpdateGeometryGroup()
        {

        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
        {

        }

        /// <summary>
        /// left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
        }

        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {

        }

        public override void UpdateVisual()
        {
            if (ShapeStyler == null)
            {
                return;
            }

            if (_textGeometryData != null && !string.IsNullOrWhiteSpace(_textGeometryData.Content))
            {

                var render = RenderOpen();
                _geometry = GetTextGeometry(_textGeometryData);


                _geometry.Transform = new TransformGroup();
                var scaleTransform = new ScaleTransform(-1, 1, _geometry.Bounds.TopLeft.X, _geometry.Bounds.TopLeft.Y);

                ((TransformGroup)_geometry.Transform).Children.Add(scaleTransform);
                ((TransformGroup)_geometry.Transform).Children.Add(new TranslateTransform(700, 0));
                ShapeStyler.SetStrokeThickness(5);
                render.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _geometry);

                //var pathGeometry = _geometry.GetFlattenedPathGeometry();


                using (XmlWriter writer = XmlWriter.Create("path.svg"))
                {
                    // Write the XML declaration and the root SVG element
                    writer.WriteStartDocument();
                    writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");

                    // Set the width and height of the SVG canvas
                    writer.WriteAttributeString("width", "500");
                    writer.WriteAttributeString("height", "500");

                    // Convert the Path object to SVG path data and write it as a "d" attribute
                    var svgContent = Convert(_geometry);
                    //string pathData = GeometryToStringConverter.Convert(path.Data);
                    writer.WriteAttributeString("d", svgContent);

                    // End the root SVG element and the XML document
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                render.Close();
            }
        }

        public static string Convert(Geometry geometry)
        {
            if (geometry == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            PathGeometry pathGeometry = geometry.GetFlattenedPathGeometry();

            var transformGroup = new TransformGroup();
            //mirror
            transformGroup.Children.Add(new ScaleTransform(-1,1,pathGeometry.Bounds.TopLeft.X, pathGeometry.Bounds.TopLeft.Y));

            transformGroup.Children.Add(new TranslateTransform(pathGeometry.Bounds.Width, 0));

            pathGeometry.Transform = transformGroup;



            foreach (PathFigure figure in pathGeometry.Figures)
            {
                sb.Append("M");
                sb.Append(figure.StartPoint.X.ToString("F2"));
                sb.Append(",");
                sb.Append(figure.StartPoint.Y.ToString("F2"));

                foreach (PathSegment segment in figure.Segments)
                {
                    if (segment is LineSegment lineSegment)
                    {
                        sb.Append(" L");
                        sb.Append(lineSegment.Point.X.ToString("F2"));
                        sb.Append(",");
                        sb.Append(lineSegment.Point.Y.ToString("F2"));
                    }
                    else if (segment is ArcSegment arcSegment)
                    {
                        sb.Append(" A");
                        sb.Append(arcSegment.Size.Width.ToString("F2"));
                        sb.Append(",");
                        sb.Append(arcSegment.Size.Height.ToString("F2"));
                        sb.Append(" ");
                        sb.Append(arcSegment.RotationAngle.ToString("F2"));
                        sb.Append(" ");
                        sb.Append(arcSegment.IsLargeArc ? "1" : "0");
                        sb.Append(",");
                        sb.Append(arcSegment.SweepDirection == SweepDirection.Clockwise ? "1" : "0");
                        sb.Append(" ");
                        sb.Append(arcSegment.Point.X.ToString("F2"));
                        sb.Append(",");
                        sb.Append(arcSegment.Point.Y.ToString("F2"));
                    }
                    // Handle other segment types if necessary
                }
            }

            return sb.ToString();
        }


        private Geometry GetTextGeometry(TextGeometryData geometryData)
        {
            FormattedText formattedText = new FormattedText(geometryData.Content,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("song"),
                geometryData.FontSize,
                ShapeStyler.SketchPen.Brush, 96);

            return formattedText.BuildGeometry(geometryData.Location);
        }

    }
}
