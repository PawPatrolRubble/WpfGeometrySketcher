using System;
using System.Windows;
using Lan.ImageViewer;
using Lan.Shapes.Custom;
using Lan.Shapes.Interfaces;
using Lan.SketchBoard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lan.Shapes.App
{
    public class App : Application
    {

        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigServices();

            //initialize shape layer data
            _serviceProvider.GetService<IShapeLayerManager>().ReadShapeLayers("ShapeLayers.json");
            _serviceProvider.GetService<IGeometryTypeManager>().ReadGeometryTypesFromAssembly();

            var currentShapeLayer = _serviceProvider.GetRequiredService<IShapeLayerManager>().Layers[0];
            var shape = new ThickenedCircle(currentShapeLayer);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }   

        private void ConfigServices()
        {

            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Environment.CurrentDirectory)
            //    .AddJsonFile("ShapeLayers.json").Build();

            //_serviceCollection.AddSingleton(config);


            _serviceCollection.AddSingleton<MainWindowViewModel>();
            _serviceCollection.AddSingleton<MainWindow>();
            _serviceCollection.AddSingleton<IGeometryTypeManager, GeometryTypeManager>();
            _serviceCollection.AddSingleton<IShapeLayerManager, ShapeLayerManager>();
            _serviceCollection.AddTransient<IImageViewerViewModel, ImageViewerControlViewModel>();
            _serviceCollection.AddTransient<ISketchBoardDataManager, SketchBoardDataManager>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();
            
        }
    }

    public class Program
    {
        [STAThread]
        public static void Main(params string[] args)
        {
            var app = new App();
            app.Run();
        }
    }
}