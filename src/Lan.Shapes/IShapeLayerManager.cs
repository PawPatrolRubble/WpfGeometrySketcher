using System.Collections.ObjectModel;

namespace Lan.Shapes
{
    public interface IShapeLayerManager
    {
        void AddShape(ShapeVisualBase shape);
        void RemoveShape(ShapeVisualBase shape);
        void ReadShapeLayers(string configurationFilePath);
        ObservableCollection<ShapeLayer> Layers { get; }
        ShapeLayer SelectedLayer { get; set; }
    }
}