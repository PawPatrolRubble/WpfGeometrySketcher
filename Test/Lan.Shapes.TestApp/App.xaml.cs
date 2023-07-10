using Lan.ImageViewer;
using Lan.Shapes.Custom;
using Lan.Shapes.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Lan.SketchBoard;

namespace Lan.Shapes.App
{
    public partial class App : Application
    {

        public static IServiceProvider ServiceProvider;
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigServices();
            ServiceProvider.GetService<IShapeLayerManager>().ReadShapeLayers("ShapeLayers.json");
            ServiceProvider.GetService<IGeometryTypeManager>().ReadGeometryTypesFromAssembly();
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

            ServiceProvider = _serviceCollection.BuildServiceProvider();
        }
    }

    //public class Program
    //{
    //    [STAThread]
    //    public static void Main(params string[] args)
    //    {
    //        var app = new App();
    //        app.Run();
    //    }
    //}
}