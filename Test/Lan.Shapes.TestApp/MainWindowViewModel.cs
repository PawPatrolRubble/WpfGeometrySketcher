using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lan.ImageViewer;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Lan.Shapes.App
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IShapeLayerManager _shapeLayerManager;
        public IImageViewerViewModel Camera1 { get; set; }
        public IImageViewerViewModel Camera2 { get; set; }

        public MainWindowViewModel(
            IImageViewerViewModel camera1,
            IShapeLayerManager shapeLayerManager)
        {
            Camera1 = camera1;
            _shapeLayerManager = shapeLayerManager;
            
            Camera1.Layers = _shapeLayerManager.Layers;

            Camera1.SelectedShapeLayer = Camera1.Layers[0];
            //Camera1 = camera;
            SelectOneShapeCommand = new RelayCommand(SelectOneShapeCommandImpl);
            GetShapeInfoCommand = new RelayCommand(GetShapeInfoCommandImpl);

        }

        public ICommand SelectOneShapeCommand { get; private set; }
        private void SelectOneShapeCommandImpl()
        {
        }

        public ICommand GetShapeInfoCommand { get; private set; }
        private void GetShapeInfoCommandImpl()
        {

        }

        private Point _mouseDblPosition;

        public Point MouseDblPosition
        {
            get => _mouseDblPosition;
            set
            {
                SetProperty(ref _mouseDblPosition, value);
                Console.WriteLine(_mouseDblPosition);
            }
        }
    }
}