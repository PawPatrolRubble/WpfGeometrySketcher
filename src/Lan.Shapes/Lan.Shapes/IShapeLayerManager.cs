using System.Collections.ObjectModel;

namespace Lan.Shapes
{
    public interface IShapeLayerManager
    {
        void AddShape(ShapeVisual shape);
        void RemoveShape(ShapeVisual shape);
        void ReadShapeLayers(string configurationFilePath);
        ObservableCollection<ShapeLayer> Layers { get; }
        ShapeLayer SelectedLayer { get; set; }
    }
}