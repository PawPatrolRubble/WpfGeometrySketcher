using System.Windows;
using Lan.ImageViewer;
using Lan.Shapes.App.ViewModels;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace Lan.Shapes.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ImageViewerControl imageViewerControl)
            {
                if (imageViewerControl.DataContext is ImageViewerControlViewModel vm)
                {
                    vm.SketchBoardDataManager.LoadShape<Cross, CrossData>(new CrossData()
                    {
                        Center = new Point(40, 40),
                        Height = 50,
                        Width = 50,
                        StrokeThickness = 1
                    });
                }
                ;
                //imageViewerControl
            }
        }
    }
}