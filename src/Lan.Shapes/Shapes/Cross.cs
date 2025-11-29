using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Interfaces;

namespace Lan.Shapes.Shapes
{
    /// <summary>
    /// Cross shape defined by center point, width and height
    /// </summary>
    public class Cross : ShapeVisualBase, IDataExport<CrossData>
    {
        private readonly LineGeometry _verticalLine = new LineGeometry();
        private readonly LineGeometry _horizontalLine = new LineGeometry();
        private Point _center;
        private int _height;
        private int _width;

        public Cross(ShapeLayer layer) : base(layer)
        {

        }

        public Point Center
        {
            get { return _center; }
            set
            {
                SetField(ref _center, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }


        public int Height
        {
            get { return _height; }
            set
            {
                SetField(ref _height, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                SetField(ref _width, value);
                UpdateVerticalAndHorizontalLine();
                UpdateVisual();
            }
        }


        private void UpdateVerticalAndHorizontalLine()
        {

            _horizontalLine.StartPoint = Center + new Vector(-Width * 1.0 / 2, 0);
            _horizontalLine.EndPoint = Center + new Vector(Width * 1.0 / 2, 0);

            _verticalLine.StartPoint = Center + new Vector(0, -Height * 1.0 / 2);
            _verticalLine.EndPoint = Center + new Vector(0, Height * 1.0 / 2);
        }


        public override Rect BoundsRect => new Rect(
            Center.X - Width / 2.0, 
            Center.Y - Height / 2.0, 
            Width, 
            Height);
        protected override void CreateHandles()
        {
            // Cross shape doesn't use drag handles
        }

        protected override void HandleResizing(Point point)
        {
            // Cross shape doesn't support resizing via handles
        }

        protected override void HandleTranslate(Point newPoint)
        {
            if (OldPointForTranslate.HasValue)
            {
                var offset = newPoint - OldPointForTranslate.Value;
                Center = new Point(Center.X + offset.X, Center.Y + offset.Y);
                OldPointForTranslate = newPoint;
            }
        }

        public void FromData(CrossData data)
        {
            Center = data.Center;
            Width = data.Width;
            Height = data.Height;
            ShapeStyler.SetStrokeThickness(data.StrokeThickness);
        }

        public CrossData GetMetaData()
        {
            return new CrossData()
            {
                Center = Center,
                Width = Width,
                Height = Height,
                StrokeThickness = ShapeStyler.SketchPen.Thickness,
            };
        }

        public override void AddText(string content, Point? location = null)
        {

        }


        public override void UpdateVisual()
        {
            base.UpdateVisual();

            var renderContext = RenderOpen();
            if (ShapeStyler != null)
            {
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _verticalLine);
                renderContext.DrawGeometry(ShapeStyler.FillColor, ShapeStyler.SketchPen, _horizontalLine);

            }

            renderContext.Close();
        }
    }
}