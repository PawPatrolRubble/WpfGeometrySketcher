using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lan.Shapes.Styler
{
    public class ShapeStyler : IShapeStyler
    {
        private Pen _sketchPen = new Pen();
        private string _dashStyle;
        public Pen SketchPen => _sketchPen;
        public Brush FillColor { get; set; }

        public string Name { get; set; }

        public IShapeStyler Clone()
        {
            return new ShapeStyler(FillColor, _sketchPen.Brush, _sketchPen.DashStyle, DragHandleSize);
        }

        public ShapeStylerParameter ToStylerParameter()
        {
            return new ShapeStylerParameter()
            {
                DashStyle = _dashStyle,
                DragHandleSize = DragHandleSize,
                FillColor = FillColor,
                StrokeColor = _sketchPen.Brush,
                StrokeThickness = _sketchPen.Thickness
            };
        }

        public void SetFillColor(Brush color)
        {
            FillColor = color;
        }

        public void SetStrokeColor(Brush color)
        {
            _sketchPen.Brush = color;
        }

        public void SetStrokeThickness(double thickness)
        {
            _sketchPen.Thickness = thickness;
        }

        public void SetPenDashStyle(DashStyle dashStyle)
        {
            _sketchPen.DashStyle = dashStyle;
        }

        public double DragHandleSize { get; set; }

        public ShapeStyler()
        {
        }

        public ShapeStyler(ShapeStylerParameter parameter)
        {
            FillColor = parameter.FillColor;
            _sketchPen.Thickness = 1;
            _sketchPen.Brush = parameter.StrokeColor;
            _sketchPen.DashStyle = ConvertStringToDashStyle(parameter.DashStyle) ;
            
            _dashStyle = parameter.DashStyle;
            DragHandleSize = parameter.DragHandleSize;
            FillColor.Opacity = parameter.FillOpacity;
        }

        private DashStyle ConvertStringToDashStyle(string dashStyleName)
        {
            DashStyle dashStyle;
            switch (dashStyleName)
            {
                case "Solid":
                    dashStyle = DashStyles.Solid;
                    break;
                case "Dash":
                    dashStyle = DashStyles.Dash;
                    break;
                case "Dot":
                    dashStyle = DashStyles.Dot;
                    break;
                case "DashDot":
                    dashStyle = DashStyles.DashDot;
                    break;
                case "DashDotDot":
                    dashStyle = DashStyles.DashDotDot;
                    break;
                default:
                    throw new ArgumentException("Invalid dash style name.");
            }

            return dashStyle;
        }

        public ShapeStyler(Brush fillColor, Brush strokeColor, DashStyle dashStyle, double dragHandleSize)
        {
            this.FillColor = fillColor;
            _sketchPen.Thickness = 1;
            _sketchPen.Brush = strokeColor;
            _sketchPen.DashStyle = dashStyle;
            DragHandleSize = dragHandleSize;
        }
    }
}