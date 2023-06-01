using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lan.ImageViewer;
using Lan.Shapes.Custom;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lan.Shapes.App
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IShapeLayerManager _shapeLayerManager;
        public IImageViewerViewModel Camera1 { get; set; }
        public IImageViewerViewModel Camera2 { get; set; }

        public MainWindowViewModel(
            IServiceProvider serviceProvider,
            IShapeLayerManager shapeLayerManager)
        {
            Camera1 = serviceProvider.GetService<IImageViewerViewModel>();
            Camera2 = serviceProvider.GetService<IImageViewerViewModel>();
            _shapeLayerManager = shapeLayerManager;

            Camera1.Layers = _shapeLayerManager.Layers;
            Camera1.SelectedShapeLayer = Camera1.Layers[0];

            Camera2.Layers = _shapeLayerManager.Layers;
            Camera2.SelectedShapeLayer = Camera2.Layers[0];


            //Camera1 = camera;
            SelectOneShapeCommand = new RelayCommand(SelectOneShapeCommandImpl);
            GetShapeInfoCommand = new RelayCommand(GetShapeInfoCommandImpl);
            LoadFromParameterCommand = new RelayCommand(LoadFromParameterCommandImpl);
            LockEditCommand = new RelayCommand(LockEditCommandImpl);
            UnlockEditCommand = new RelayCommand(UnlockEditCommandImpl);
            FilterShapeTypeCommand = new RelayCommand(FilterShapeTypeCommandImpl);
            SetTagNameCommand = new RelayCommand(SetTagNameCommandImpl);

            ImageViewerViewModels.Add(Camera1);
            ImageViewerViewModels.Add(Camera2);

        }

        private ImageSource CreateImageSourceFromFile(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = fileStream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        private IImageViewerViewModel _selectedImageViewModel;

        public IImageViewerViewModel SelectedImageViewModel
        {
            get => _selectedImageViewModel;
            set { SetProperty(ref _selectedImageViewModel, value); }
        }

        public ObservableCollection<IImageViewerViewModel> ImageViewerViewModels { get; set; } =
            new ObservableCollection<IImageViewerViewModel>();


        public ICommand SelectOneShapeCommand { get; private set; }
        private void SelectOneShapeCommandImpl()
        {

        }

        public ICommand GetShapeInfoCommand { get; private set; }
        private void GetShapeInfoCommandImpl()
        {

        }

        public ICommand SetTagNameCommand { get; private set; }


        private void SetTagNameCommandImpl()
        {
            SelectedImageViewModel.SketchBoardDataManager.SelectedGeometry.Tag = "absdasdasd";
        }

        public RelayCommand LoadFromParameterCommand { get; private set; }
        private void LoadFromParameterCommandImpl()
        {
            Camera1.SketchBoardDataManager.LoadShape<Rectangle, PointsData>(new PointsData(1, new List<Point>()
            {
                new Point(10,10),
                new Point(50,50)
            }));

            Camera1.SketchBoardDataManager.LoadShape<Ellipse, EllipseData>(new EllipseData()
            {
                Center = new Point(150, 150),
                RadiusX = 100,
                RadiusY = 100,
            });

            Camera1.SketchBoardDataManager.LoadShape<ThickenedCross, PointsData>(new PointsData(10, new List<Point>()
            {
                new Point(152,52),
                new Point(359,463),
                new Point(50,154),
                new Point(461,361),
            }));

            Camera1.SketchBoardDataManager.LoadShape<ThickenedCircle, EllipseData>(new EllipseData()
            {
                Center = new Point(400, 400),
                StrokeThickness = 10,
                RadiusX = 150,
                RadiusY = 150
            });

            Camera1.SketchBoardDataManager.LoadShape<ThickenedRectangle, PointsData>(new PointsData(10,
                new List<Point>()
            {
                new Point(600,600),
                new Point(800,800),
            }));


            Camera1.SketchBoardDataManager.LoadShape<ThickenedLine, PointsData>(new PointsData(10,
                new List<Point>()
            {
                new Point(600,600),
                new Point(800,800),
            }));
            Camera1.SketchBoardDataManager.Shapes[0].Lock();
        }

        public RelayCommand LockEditCommand { get; private set; }
        private void LockEditCommandImpl()
        {
            SelectedImageViewModel.SketchBoardDataManager.SelectedGeometry?.Lock();
        }


        public RelayCommand UnlockEditCommand { get; private set; }
        private void UnlockEditCommandImpl()
        {
            SelectedImageViewModel.SketchBoardDataManager.Shapes.Last().UnLock();
        }

        public RelayCommand FilterShapeTypeCommand { get; private set; }
        private void FilterShapeTypeCommandImpl()
        {
            SelectedImageViewModel.FilterGeometryTypes(x => !x.Name.Equals(nameof(ThickenedCross)));
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