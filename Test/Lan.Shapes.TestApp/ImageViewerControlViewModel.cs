#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lan.ImageViewer;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;
using Lan.SketchBoard;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Lan.Shapes.App
{
    public class ImageViewerControlViewModel : ObservableObject, IImageViewerViewModel
    {
        private const double _scaleIncremental = 0.1;

        private IShapeStyler _shapeStyler;

        /// <summary>
        /// the sketch boar data manager used to manage sketch board
        /// </summary>
        public ISketchBoardDataManager SketchBoardDataManager { get; set; }

        /// <summary>
        /// geometry type list
        /// </summary>
        public IEnumerable<GeometryType> GeometryTypeList { get; set; }


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
                        SketchBoardDataManager.SelectDrawingTool(_selectedGeometryType.Name);

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


        public ImageViewerControlViewModel()
        {
            Scale = 1;

            _shapeStyler = new ShapeStylerFactory().CustomShapeStyler(Brushes.Transparent, Brushes.Red, 5, 15);

            SketchBoardDataManager = new SketchBoardDataManager();

            SketchBoardDataManager.SetShapeLayer(ShapeLayer.CreateLayer(_shapeStyler, 1, "mask", "mask layer"));
            SketchBoardDataManager.RegisterGeometryType("Rectangle", typeof(Rectangle));
            SketchBoardDataManager.RegisterGeometryType("Ellipse", typeof(Ellipse));
            SketchBoardDataManager.RegisterGeometryType(nameof(Polygon), typeof(Polygon));

            CreateGeometryTypeList();

            Image = CreateEmptyImageSource(1096,1024);

            ZoomOutCommand = new RelayCommand(() =>
            {
                Scale *= (1-_scaleIncremental);
            });

            ZoomInCommand = new RelayCommand(() =>
            {
                Scale *=(1+ _scaleIncremental);
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



            GeometryTypeList = new List<GeometryType>(SketchBoardDataManager.GetRegisteredGeometryTypes()
                .Select(x => new GeometryType(x, x, getIconImage(x))));

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

        private Dictionary<string, string> GeometryList= new Dictionary<string, string>();

    }
}
