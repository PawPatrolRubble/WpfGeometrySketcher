using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Lan.ImageViewer;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;
using Lan.SketchBoard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Lan.Shapes.App
{
    public class MainWindowViewModel:ObservableObject
    {

        public IImageViewerViewModel Camera1 { get; set; }
        public IImageViewerViewModel Camera2 { get; set; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            SelectOneShapeCommand= new RelayCommand(SelectOneShapeCommandImpl);
            GetShapeInfoCommand= new RelayCommand(GetShapeInfoCommandImpl);
            Camera1 = new ImageViewerControlViewModel();
            Camera2 = new ImageViewerControlViewModel();
        }


        public ICommand SelectOneShapeCommand { get; private set; }
        private void SelectOneShapeCommandImpl()
        {
        }

        public ICommand GetShapeInfoCommand { get; private set; }
        private void GetShapeInfoCommandImpl()
        {

        }
    }
}