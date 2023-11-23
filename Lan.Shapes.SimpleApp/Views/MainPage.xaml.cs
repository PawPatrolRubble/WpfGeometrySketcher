using Lan.ImageViewer;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;
using Lan.Shapes.SimpleApp.ViewModels;

using Prism.Ioc;

using System.Windows;
using System.Windows.Controls;

namespace Lan.Shapes.App
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = ContainerLocator.Container.Resolve<MainPageViewModel>();
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