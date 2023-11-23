using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;
using Lan.Shapes.SimpleApp.ViewModels;

using System.Windows;
using Prism.Ioc;

namespace Lan.Shapes.App
{
    /// <summary>
    /// Interaction logic for ImageViewerHalconWindow.xaml
    /// </summary>
    public partial class ImageViewerHalconWindow : Window
    {
        public ImageViewerHalconWindow()
        {
            InitializeComponent();
            DataContext = ContainerLocator.Container.Resolve<MainPageViewModel>();
        }

    }
}
