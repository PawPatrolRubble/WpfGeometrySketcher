using System.Collections.ObjectModel;

namespace Lan.Shapes
{
    public interface IShapeLayerManager
    {
        /// <summary>
        /// read shape layers from config json file, it can be from IConfiguration or from a json file directly
        /// </summary>
        /// <param name="configurationFilePath"></param>
        void ReadShapeLayers(string configurationFilePath = "");
        
        /// <summary>
        /// get all shape layers
        /// </summary>
        ObservableCollection<ShapeLayer> Layers { get; }

        /// <summary>
        /// persist any updated data for shape layers read from files 
        /// </summary>
        /// <param name="filePath"></param>
        void SaveLayerConfigurations(string filePath="");

    }
}