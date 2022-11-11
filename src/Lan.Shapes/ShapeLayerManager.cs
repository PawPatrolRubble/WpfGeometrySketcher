using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace Lan.Shapes
{
    public class ShapeLayerManager : DependencyObject, IShapeLayerManager, INotifyPropertyChanged
    {
        #region fields

        private readonly List<ShapeVisual> _shapes = new List<ShapeVisual>();

        #endregion


        #region properties

        public event PropertyChangedEventHandler PropertyChanged;


        private ShapeLayer _selectedLayer;

        public ShapeLayer SelectedLayer
        {
            get => _selectedLayer ?? Layers[0];
            set
            {
                _selectedLayer = value;
                OnPropertyChanged();
            }
        }

        public void ReadShapeLayers(string configurationFilePath)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ShapeLayer> Layers { get; private set; } = new ObservableCollection<ShapeLayer>();

        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(nameof(Shapes),
            typeof(ObservableCollection<ShapeVisual>), typeof(ShapeLayerManager),
            new PropertyMetadata(default(ObservableCollection<ShapeVisual>)));

        public ObservableCollection<ShapeVisual> Shapes
        {
            get => (ObservableCollection<ShapeVisual>)GetValue(ShapesProperty);
            set { SetValue(ShapesProperty, value); }
        }

        #endregion


        #region constructor

        public ShapeLayerManager()
        {
            this.Shapes = new ObservableCollection<ShapeVisual>();
            this.SetValue(ShapesProperty, new ObservableCollection<ShapeVisual>());
        }

        #endregion

        #region public methods

        public void ReadShapeLayersFromConfig(string configurationFilePath)
        {
            var shapes = JObject.Parse(File.ReadAllText(configurationFilePath));
            var layers = from section in shapes["ShapeLayerList"]
                select new ShapeLayer(new ShapeLayerParameter
                {
                    LayerId = (int)section["LayerId"],
                    Name = (string)section["Name"],
                    Description = (string)section["Description"],
                    StylerFillColor =
                        ColorWithOpacity((string)section["FillColor"], (double)section["FillColorOpacity"]),
                    StylerStrokeColor = FromHexStringToBrush((string)section["StrokeColor"]),
                    StylerStrokeThickness = (double)section["LineThickness"],
                    StylerDashStyle = ConvertToDashStyleFromString((string)section["lineStyle"]),
                    TextForeground = FromHexStringToBrush((string)section["TextForeground"]),
                    BorderBackground = FromHexStringToBrush((string)section["BorderBackground"])
                });
            Layers.AddRange(layers);
        }

        private Brush ColorWithOpacity(string colorString, double opacity)
        {
            var b = FromHexStringToBrush(colorString);

            b.Opacity = opacity;
            return b;
        }

        private DashStyle ConvertToDashStyleFromString(string s)
        {
            switch (s)
            {
                case var dash when s.Equals("dash", StringComparison.OrdinalIgnoreCase):
                    return DashStyles.Dash;
                case var dash when s.Equals("DashDot", StringComparison.OrdinalIgnoreCase):
                    return DashStyles.DashDot;
                case var dash when s.Equals("DashDotDot", StringComparison.OrdinalIgnoreCase):
                    return DashStyles.DashDotDot;
                case var dash when s.Equals("Dot", StringComparison.OrdinalIgnoreCase):
                    return DashStyles.Dot;
                case var dash when s.Equals("Solid", StringComparison.OrdinalIgnoreCase):
                default:
                    return DashStyles.Solid;
            }
        }

        private Brush FromHexStringToBrush(string hexString)
        {
            var converter = new System.Windows.Media.BrushConverter();
            return (Brush)converter.ConvertFromString(hexString);
        }


        public void AddShape(ShapeVisual shape)
        {
            if (SelectedLayer != null)
            {
                _shapes.Add(shape);
            }
        }

        public void RemoveShape(ShapeVisual shape)
        {
            _shapes.Remove(shape);
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}