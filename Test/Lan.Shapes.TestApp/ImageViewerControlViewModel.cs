#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lan.ImageViewer;
using Lan.Shapes.Shapes;
using Lan.SketchBoard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lan.Shapes.App
{
    public class ImageViewerControlViewModel : ObservableObject, IImageViewerViewModel
    {
        private readonly IShapeLayerManager _shapeLayerManager;
        private readonly IGeometryTypeManager _geometryTypeManager;
        private const double ScaleIncremental = 0.1;

        /// <summary>
        /// the sketch boar data manager used to manage sketch board
        /// </summary>
        public ISketchBoardDataManager SketchBoardDataManager { get; set; }

        /// <summary>
        /// geometry type list
        /// </summary>
        public IEnumerable<GeometryType> GeometryTypeList { get; set; }


        public ObservableCollection<ShapeLayer> Layers { get; set; }

        /// <summary>
        /// 当前选中的layer
        /// </summary>
        //public ShapeLayer SelectedShapeLayer { get; set; }

        private ShapeLayer _selectedShapeLayer;

        public ShapeLayer SelectedShapeLayer
        {
            get => _selectedShapeLayer;
            set
            {
                if (SetProperty(ref _selectedShapeLayer, value))
                {
                   SketchBoardDataManager.SetShapeLayer(_selectedShapeLayer);
                }
            }
        }



        private GeometryType? _selectedGeometryType;

        /// <summary>
        ///
        /// </summary>
        public GeometryType? SelectedGeometryType
        {
            get => _selectedGeometryType;
            set
            {
                if (SetProperty(ref _selectedGeometryType, value))
                {
                    if (_selectedGeometryType != null)
                        SketchBoardDataManager.SetGeometryType(_geometryTypeManager.GetGeometryTypeByName(_selectedGeometryType.Name));

                }

            }
        }

        /// <summary>
        /// the image displayed
        /// </summary>
        public ImageSource Image { get; set; }


        private double _scale;

        public double Scale
        {
            get => _scale;
            set
            {
                SetProperty(ref _scale, value);
            }
        } 

        public ICommand ZoomOutCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ScaleToOriginalSizeCommand { get; set; }
        public ICommand ScaleToFitCommand { get; set; }


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
            Image = ImageFromFile(Path.Combine(Environment.CurrentDirectory, "roi.png") );

            ZoomOutCommand = new RelayCommand(() =>
            {
                Scale *= (1-ScaleIncremental);
            });


            ZoomInCommand = new RelayCommand(() =>
            {
                Scale *=(1+ ScaleIncremental);
            });

            ScaleToFitCommand = new RelayCommand(() => Scale = -1);
            ScaleToOriginalSizeCommand = new RelayCommand(() => Scale = 0);
        }



        private void CreateGeometryTypeList()
        {
            var iconPngsFromResource = new Dictionary<string, string>
            {
                { "Ellipse", "pack://application:,,,/Lan.ImageViewer;component/Icons/ellipse.png" } ,
                { "Rectangle", "pack://application:,,,/Lan.ImageViewer;component/Icons/square.png" } ,
                { "Polygon", "pack://application:,,,/Lan.ImageViewer;component/Icons/polygon.png" } ,
            };


            Func<string,ImageSource> getIconImage = (string iconName) =>
            
            {
                if (iconPngsFromResource.ContainsKey(iconName))
                {
                    return new BitmapImage(new Uri(iconPngsFromResource[iconName],UriKind.Absolute));
                }

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

        private ImageSource CreateEmptyImageSource(int width, int height)
        {

            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.LightBlue);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            return BitmapSource.Create(
                width, height,
                96, 96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);
        }


    }
}
