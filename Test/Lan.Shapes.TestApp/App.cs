using System;
using System.Windows;
using Lan.SketchBoard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lan.Shapes.TestApp
{
    public class App : Application
    {

        private IServiceProvider _serviceProvider;
        private IServiceCollection _serviceCollection = new ServiceCollection();
        
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigServices();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            
            mainWindow.Show();
        }

        private void ConfigServices()
        {
            _serviceCollection.AddSingleton<MainWindowViewModel>();
            _serviceCollection.AddSingleton<MainWindow>();
            
            _serviceCollection.TryAddSingleton<ISketchBoardDataManager, SketchBoardDataManager>();
            
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