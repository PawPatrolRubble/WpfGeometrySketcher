using Lan.Shapes.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Shapes;

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
                render.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _geometry);
                render.Close();
            }
        }


        private Geometry GetTextGeometry(TextGeometryData geometryData)
        {
            FormattedText formattedText = new FormattedText(geometryData.Content,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                geometryData.FontSize,
                ShapeStyler.SketchPen.Brush, 96);

            return formattedText.BuildGeometry(geometryData.Location);
        }

    }
}
