using System.Configuration;
using System.Drawing;
using System.IO;
using Lan.ImageViewer.Prism.Views;
using Lan.Shapes.Custom;
using Lan.Shapes.DialogGeometry;
using Lan.Shapes.Interfaces;
using Lan.SketchBoard;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Lan.ImageViewer.Prism {
    public class ImageViewerModule : IModule {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var baseDir = containerProvider.Resolve<IConfiguration>().GetSection("configBaseDir").Value;
            var shapeLayerJsonFilePath = containerProvider.Resolve<IConfiguration>().GetSection("shapeLayerPath").Value;
            var fullPath = Path.Combine(baseDir, shapeLayerJsonFilePath);
            containerProvider.Resolve<IShapeLayerManager>().ReadShapeLayers(fullPath);

            var _geometryTypeManager = containerProvider.Resolve<IGeometryTypeManager>();
            _geometryTypeManager.RegisterGeometryType<GridGeometry>();
            _geometryTypeManager.RegisterGeometryType<GriddedRectangle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedCircle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedCross>();
            _geometryTypeManager.RegisterGeometryType<ThickenedRectangle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedLine>();

        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterSingleton<IGeometryTypeManager, GeometryTypeManager>();
            containerRegistry.RegisterSingleton<IShapeLayerManager, ShapeLayerManager>();
            containerRegistry.Register<IImageViewerViewModel, ImageViewerControlViewModel>();
            containerRegistry.RegisterSingleton<ISketchBoardDataManager, SketchBoardDataManager>();
        }
    }
}