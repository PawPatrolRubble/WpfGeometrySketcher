using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lan.ImageViewer.Prism;
using Lan.Shapes.App;
using Lan.Shapes.SimpleApp.ViewModels;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using Serilog;
using Serilog.Enrichers;

using ImageViewerControlViewModel = Lan.ImageViewer.Prism.ImageViewerControlViewModel;

namespace Lan.Shapes.SimpleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ImageViewerControlViewModel>();
            containerRegistry.Register<MainPageViewModel>();
            containerRegistry.RegisterForNavigation<MainPage>();

            var config = GetConfiguration();
            containerRegistry.RegisterSingleton<IConfiguration>(() => config);
            var logger = CreateSerilogLogger(config);
            containerRegistry.RegisterServices(s =>
            {
                s.AddLogging(logBuilder => logBuilder.AddSerilog(logger));
            });

        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<ImageViewerModule>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Container.Resolve<IRegionManager>().RequestNavigate("MainContent", nameof(MainPage));
        }


        private ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithProperty(ThreadNameEnricher.ThreadNamePropertyName, "MainUi")
                .CreateLogger();
        }


        private bool IsLocalDebug()
        {
            var variables = Environment.GetEnvironmentVariables();

            if (variables.Contains("UseLocalDir") && variables["UseLocalDir"].Equals("true"))
            {
                return true;
            }

            return false;
        }


        private IConfiguration GetConfiguration()
        {
            if (IsLocalDebug())
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.local.json", false, true);
                return builder.Build();
            }
            else
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", false, true);
                return builder.Build();
            }
        }



    }
}
