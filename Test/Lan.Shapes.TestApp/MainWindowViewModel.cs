#nullable enable

#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Lan.ImageViewer;
using Lan.Shapes.Custom;
using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

#endregion

namespace Lan.Shapes.App
{
    public class MainWindowViewModel : ObservableObject
    {
        #region fields

        private readonly IShapeLayerManager _shapeLayerManager;

        private Point _mouseDblPosition;

        private IImageViewerViewModel? _selectedImageViewModel;

        #endregion

        #region Constructors

        #region constructors

        //public IImageViewerViewModel Camera2 { get; set; }

        public MainWindowViewModel(
            IServiceProvider serviceProvider,
            IShapeLayerManager shapeLayerManager)
        {
            Camera1 = serviceProvider.GetService<IImageViewerViewModel>();
            //Camera2 = serviceProvider.GetService<IImageViewerViewModel>();
            _shapeLayerManager = shapeLayerManager;

            Camera1.Layers = _shapeLayerManager.Layers;
            Camera1.SelectedShapeLayer = Camera1.Layers[0];

            //Camera2.Layers = _shapeLayerManager.Layers;
            //Camera2.SelectedShapeLayer = Camera2.Layers[0];

            //Camera1 = camera;
            SelectOneShapeCommand = new RelayCommand(SelectOneShapeCommandImpl);
            GetShapeInfoCommand = new RelayCommand(GetShapeInfoCommandImpl);
            LoadFromParameterCommand = new RelayCommand(LoadFromParameterCommandImpl);
            LockEditCommand = new RelayCommand(LockEditCommandImpl);
            UnlockEditCommand = new RelayCommand(UnlockEditCommandImpl);
            FilterShapeTypeCommand = new RelayCommand(FilterShapeTypeCommandImpl);
            SetTagNameCommand = new RelayCommand(SetTagNameCommandImpl);
            ChooseFileDialogCommand = new RelayCommand(ChooseFileDialogCommandImpl);
            ClearAllShapesCommand = new RelayCommand(ClearAllShapesCommandImpl);

            ImageViewerViewModels.Add(Camera1);
            //ImageViewerViewModels.Add(Camera2);
            SelectedImageViewModel = ImageViewerViewModels[0];
        }

        #endregion

        #endregion

        #region properties

        public IImageViewerViewModel Camera1 { get; set; }

        public IImageViewerViewModel? SelectedImageViewModel
        {
            get => _selectedImageViewModel;
            set => SetProperty(ref _selectedImageViewModel, value);
        }

        public ObservableCollection<IImageViewerViewModel> ImageViewerViewModels { get; set; } =
            new ObservableCollection<IImageViewerViewModel>();


        public ICommand SelectOneShapeCommand { get; }

        public ICommand GetShapeInfoCommand { get; }

        public ICommand SetTagNameCommand { get; }

        public RelayCommand LoadFromParameterCommand { get; }

        public RelayCommand LockEditCommand { get; }


        public RelayCommand UnlockEditCommand { get; }

        public RelayCommand FilterShapeTypeCommand { get; }
        public RelayCommand ChooseFileDialogCommand { get; private set; }

        public RelayCommand ClearAllShapesCommand { get; private set; }


        private void ClearAllShapesCommandImpl()
        {
            if (SelectedImageViewModel != null)
            {
                SelectedImageViewModel.SketchBoardDataManager.ClearAllShapes();
            }
        }

        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set { SetProperty(_imagePath, value, changed => UpdateImageSource(changed)); }
        }

        private void UpdateImageSource(string changed)
        {
            if (SelectedImageViewModel != null && !string.IsNullOrEmpty(changed))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(File.ReadAllBytes(changed));
                bitmapImage.EndInit();
                SelectedImageViewModel.Image = bitmapImage;
            }
        }

        private void ChooseFileDialogCommandImpl()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.Filter =
                "JPEG files (*.jpg, *.jpeg)|*.jpg;*.jpeg|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }


        public Point MouseDblPosition
        {
            get => _mouseDblPosition;
            set
            {
                SetProperty(ref _mouseDblPosition, value);
                Console.WriteLine(_mouseDblPosition);
            }
        }

        #endregion

        #region all other members

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

        private void SelectOneShapeCommandImpl()
        {
        }

        private void GetShapeInfoCommandImpl()
        {
        }


        private void SetTagNameCommandImpl()
        {
            try
            {
                if (SelectedImageViewModel?.SketchBoardDataManager.SelectedGeometry != null)
                {
                    SelectedImageViewModel.SketchBoardDataManager.SelectedGeometry.Tag = "absdasdasd";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadFromParameterCommandImpl()
        {
            //Camera1.SketchBoardDataManager.LoadShape<Rectangle, PointsData>(new PointsData(1, new List<Point>()
            //{
            //    new Point(10,10),
            //    new Point(50,50)
            //})); 


            Camera1.SketchBoardDataManager.LoadShape<TextGeometry, TextGeometryData>(
                new TextGeometryData(new Point(200, 200), "Hello world", 50));


            //Camera1.SketchBoardDataManager.LoadShape<Ellipse, EllipseData>(new EllipseData()
            //{
            //    Center = new Point(150, 150),
            //    RadiusX = 100,
            //    RadiusY = 100,
            //});

            //Camera1.SketchBoardDataManager.LoadShape<ThickenedCross, PointsData>(new PointsData(10, new List<Point>()
            //{
            //    new Point(152,52),
            //    new Point(359,463),
            //    new Point(50,154),
            //    new Point(461,361),
            //}));

            //Camera1.SketchBoardDataManager.LoadShape<ThickenedCircle, EllipseData>(new EllipseData()
            //{
            //    Center = new Point(400, 400),
            //    StrokeThickness = 10,
            //    RadiusX = 150,
            //    RadiusY = 150
            //});

            //Camera1.SketchBoardDataManager.LoadShape<ThickenedRectangle, PointsData>(new PointsData(10,
            //    new List<Point>()
            //{
            //    new Point(600,600),
            //    new Point(800,800),
            //}));


            //Camera1.SketchBoardDataManager.LoadShape<ThickenedLine, PointsData>(new PointsData(10,
            //    new List<Point>()
            //{
            //    new Point(600,600),
            //    new Point(800,800),
            //}));
            //Camera1.SketchBoardDataManager.Shapes[0].Lock();
        }

        private void LockEditCommandImpl()
        {
            SelectedImageViewModel?.SketchBoardDataManager.SelectedGeometry?.Lock();
        }

        private void UnlockEditCommandImpl()
        {
            SelectedImageViewModel?.SketchBoardDataManager.Shapes.Last().UnLock();
        }

        private void FilterShapeTypeCommandImpl()
        {
            SelectedImageViewModel?.FilterGeometryTypes(x => x.Name.Equals(nameof(Rectangle)));
        }

        #endregion
    }
}