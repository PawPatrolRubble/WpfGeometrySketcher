using Lan.ImageViewer;
using Lan.Shapes.App.ViewModels;
using Lan.Shapes.Interfaces;

using Lan.Shapes.Shapes;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Lan.ImageViewer.Halcon;

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
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ImageViewerHalconControl imageViewerControl)
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
