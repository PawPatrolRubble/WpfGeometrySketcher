using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    /// <summary>
    /// responsible for grouping shapes drawn and setting display style in the group
    /// all shapes must be managed by layer, 
    /// layer information is read from appsetting.json
    /// </summary>
    public class ShapeLayer : INotifyPropertyChanged
    {
        #region fields

        private List<ShapeVisualBase> _shapeVisuals = new List<ShapeVisualBase>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region properties

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        internal IShapeStyler GetStyler(bool isSelected)
        {
            return isSelected ? GetSelectedState() : GetUnselectedShapeStyler();
        }

        public int LayerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IShapeStyler Styler { get; set; }

        public Brush TextForeground { get; set; } = Brushes.Black;
        public Brush BorderBackground { get; set; } = Brushes.LightBlue;

        #endregion


        #region constructor

        private ShapeLayer(int layerId, string name, string description)
        {
            LayerId = layerId;
            Name = name;
            Description = description;
        }

        public ShapeLayer(ShapeLayerParameter shapeLayerParameter)
        {
            LayerId = shapeLayerParameter.LayerId;
            Name = shapeLayerParameter.Name;
            Description = shapeLayerParameter.Description;


            Styler = new ShapeStyler();
            Styler.SetFillColor(shapeLayerParameter.StylerFillColor);
            Styler.SetPenDashStyle(shapeLayerParameter.StylerDashStyle);
            Styler.SetStrokeColor(shapeLayerParameter.StylerStrokeColor);
            Styler.SetStrokeThickness(shapeLayerParameter.StylerStrokeThickness);
            BorderBackground = shapeLayerParameter.BorderBackground;
            TextForeground = shapeLayerParameter.TextForeground;
        }


        public static ShapeLayer CreateLayer(IShapeStyler shapeStyler, int layerId, string name, string description)
        {
            return new ShapeLayer(layerId, name, description)
            {
                Styler = shapeStyler,
            };
        }

        #endregion


        #region public interfaces

        public IEnumerable<ShapeVisualBase> RenderShapes(IEnumerable<ShapeVisualBase> shapeVisuals)
        {
            foreach (var visual in shapeVisuals)
            {
                var dc = visual.RenderOpen();
                dc.DrawGeometry(Styler.FillColor, Styler.SketchPen, visual.RenderGeometry);
                dc.Close();
            }

            return shapeVisuals;
        }

        #endregion

        public IShapeStyler GetSelectedState()
        {
            var brush = BrushFromHexString("#3eb03f");
            brush.Opacity = 0.3;
            Styler.SetFillColor(brush);

            return Styler.Clone();
        }

        private Brush BrushFromHexString(string hextString)
        {
            var converter = new BrushConverter();
            return (Brush)converter.ConvertFromString(hextString);
        }

        public IShapeStyler GetUnselectedShapeStyler()
        {
            Styler.SetFillColor(Brushes.Transparent);

            return Styler.Clone();
        }
    }
}