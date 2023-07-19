using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Interfaces;
using Newtonsoft.Json;

namespace Lan.ImageViewer.Prism
{
    public class ShapeLayerManager : DependencyObject, IShapeLayerManager, INotifyPropertyChanged
    {

        #region fields

        private string _path;

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

        public void SaveLayerConfigurations(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                _path = filePath;
            }

            var serialized = JsonConvert.SerializeObject(Layers.Select(x=>x.ToShapeLayerParameter()).ToList());
            File.WriteAllText(_path, serialized);
        }


        public void ReadShapeLayers(string configurationFilePath)
        {
            if (!string.IsNullOrEmpty(configurationFilePath))
            {

                using (var file = new StreamReader(configurationFilePath))
                {
                    var shapeLayerParameters = JsonConvert.DeserializeObject<List<ShapeLayerParameter>>(file.ReadToEnd());
                    CollectionExtension.AddRange(Layers, shapeLayerParameters.Select(x => new ShapeLayer(x)));
                }

                _path = configurationFilePath;
            }
        }

        public ObservableCollection<ShapeLayer> Layers { get; private set; } = new ObservableCollection<ShapeLayer>();

        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(nameof(Shapes),
            typeof(ObservableCollection<ShapeVisualBase>), typeof(ShapeLayerManager),
            new PropertyMetadata(default(ObservableCollection<ShapeVisualBase>)));

        public ObservableCollection<ShapeVisualBase> Shapes
        {
            get => (ObservableCollection<ShapeVisualBase>)GetValue(ShapesProperty);
            set { SetValue(ShapesProperty, value); }
        }

        #endregion


        #region constructor

        public ShapeLayerManager()
        {
            this.Shapes = new ObservableCollection<ShapeVisualBase>();
            this.SetValue(ShapesProperty, new ObservableCollection<ShapeVisualBase>());
        }

        #endregion

        #region public methods

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


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}