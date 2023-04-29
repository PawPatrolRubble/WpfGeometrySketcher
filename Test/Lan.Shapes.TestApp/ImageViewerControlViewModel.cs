#nullable enable

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lan.ImageViewer;
using Lan.SketchBoard;

#endregion

namespace Lan.Shapes.App
{
    public class ImageViewerControlViewModel : ObservableObject, IImageViewerViewModel
    {
        #region fields

        private const double ScaleIncremental = 0.1;

        #endregion

        #region fields

        private readonly IGeometryTypeManager _geometryTypeManager;
        private readonly IShapeLayerManager _shapeLayerManager;


        private double _scale;


        private GeometryType? _selectedGeometryType;

        /// <summary>
        /// 当前选中的layer
        /// </summary>
        //public ShapeLayer SelectedShapeLayer { get; set; }
        private ShapeLayer _selectedShapeLayer;

        #endregion

        #region Propeties

        public ICommand ChooseGeometryTypeCommand { get; private set; }

        #endregion

        #region Constructors

        public ImageViewerControlViewModel(
            IShapeLayerManager shapeLayerManager,
            ISketchBoardDataManager sketchBoardDataManager,
            IGeometryTypeManager geometryTypeManager)
        {
            SketchBoardDataManager = sketchBoardDataManager;
            _shapeLayerManager = shapeLayerManager;
            _geometryTypeManager = geometryTypeManager;

            Scale = 1;
            CreateGeometryTypeList();

            //Image = CreateEmptyImageSource(1096, 1024);
            Image = ImageFromFile(Path.Combine(Environment.CurrentDirectory, "roi.png"));

            ZoomOutCommand = new RelayCommand(() => { Scale *= 1 - ScaleIncremental; });

            ChooseGeometryTypeCommand = new RelayCommand<GeometryType>(ChooseGeometryTypeCommandImpl);

            ZoomInCommand = new RelayCommand(() => { Scale *= 1 + ScaleIncremental; });

            ScaleToFitCommand = new RelayCommand(() => Scale = -1);
            ScaleToOriginalSizeCommand = new RelayCommand(() => Scale = 0);
        }

        #endregion

        #region implementations

        /// <summary>
        /// the sketch boar data manager used to manage sketch board
        /// </summary>
        public ISketchBoardDataManager SketchBoardDataManager { get; set; }

        /// <summary>
        /// geometry type list
        /// </summary>
        public IEnumerable<GeometryType> GeometryTypeList { get; set; }


        public ObservableCollection<ShapeLayer> Layers { get; set; }

        public ShapeLayer SelectedShapeLayer
        {
            get => _selectedShapeLayer;
            set
            {
                if (SetProperty(ref _selectedShapeLayer, value))
                    SketchBoardDataManager.SetShapeLayer(_selectedShapeLayer);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public GeometryType? SelectedGeometryType
        {
            get => _selectedGeometryType;
            set
            {
                if (SetProperty(ref _selectedGeometryType, value))
                    if (_selectedGeometryType != null)
                        SketchBoardDataManager.SetGeometryType(
                            _geometryTypeManager.GetGeometryTypeByName(_selectedGeometryType.Name));
            }
        }

        /// <summary>
        /// the image displayed
        /// </summary>
        public ImageSource Image { get; set; }

        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        public ICommand ZoomOutCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ScaleToOriginalSizeCommand { get; set; }
        public ICommand ScaleToFitCommand { get; set; }

        #endregion

        #region others

        private void ChooseGeometryTypeCommandImpl(GeometryType? geometryType)
        {
            SelectedGeometryType = geometryType;
        }

        private ImageSource CreateEmptyImageSource(int width, int height)
        {
            var stride = width / 8;
            var pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            var colors = new List<Color>();
            colors.Add(Colors.LightBlue);
            colors.Add(Colors.Blue);
            colors.Add(Colors.Green);
            var myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            return BitmapSource.Create(
                width, height,
                96, 96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);
        }


        private void CreateGeometryTypeList()
        {
            var iconPngsFromResource = new Dictionary<string, string>
            {
                { "Ellipse", "pack://application:,,,/Lan.ImageViewer;component/Icons/ellipse.png" },
                { "Rectangle", "pack://application:,,,/Lan.ImageViewer;component/Icons/square.png" },
                { "Polygon", "pack://application:,,,/Lan.ImageViewer;component/Icons/polygon.png" }
            };


            Func<string, ImageSource> getIconImage = iconName =>

            {
                if (iconPngsFromResource.ContainsKey(iconName))
                    return new BitmapImage(new Uri(iconPngsFromResource[iconName], UriKind.Absolute));

                return CreateEmptyImageSource(16, 16);
            };


            GeometryTypeList = new List<GeometryType>(_geometryTypeManager.GetRegisteredGeometryTypes()
                .Select(x => new GeometryType(x, x, getIconImage(x))));
        }

        private ImageSource ImageFromFile(string filePath)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filePath);
            image.EndInit();
            image.Freeze();
            return image;
        }

        #endregion
    }
}