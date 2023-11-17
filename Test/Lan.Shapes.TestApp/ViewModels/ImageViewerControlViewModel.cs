﻿#nullable enable

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Lan.ImageViewer;
using Lan.Shapes.Custom;
using Lan.Shapes.DialogGeometry;
using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;

#endregion

namespace Lan.Shapes.App.ViewModels
{
    public class ImageViewerControlViewModel : ObservableObject, IImageViewerViewModel
    {
        #region constructor

        #region Constructors

        public ImageViewerControlViewModel(
            IShapeLayerManager shapeLayerManager,
            ISketchBoardDataManager sketchBoardDataManager,
            IGeometryTypeManager geometryTypeManager)
        {
            _resourceDictionary = new ResourceDictionary();
            _resourceDictionary.Source = new Uri("pack://application:,,,/Lan.ImageViewer;component/Geometries.xaml");
            SketchBoardDataManager = sketchBoardDataManager;
            _shapeLayerManager = shapeLayerManager;
            _geometryTypeManager = geometryTypeManager;
            GeometryTypeList = new ObservableCollection<GeometryType>();

            Scale = 1;
            ShowSimpleCanvas = true;
            CreateGeometryTypeList();

            Image = CreateEmptyImageSource(2048, 2048);
            //Image = ImageFromFile(Path.Combine(Environment.CurrentDirectory, "996.png"));

            ZoomOutCommand = new RelayCommand(() => { Scale *= 1 - ScaleIncremental; });

            ChooseGeometryTypeCommand = new RelayCommand<GeometryType>(ChooseGeometryTypeCommandImpl);

            ZoomInCommand = new RelayCommand(() => { Scale *= 1 + ScaleIncremental; });

            ScaleToFitCommand = new RelayCommand(() => Scale = -1);
            ScaleToOriginalSizeCommand = new RelayCommand(() => Scale = 0);


        }

        #endregion

        #endregion

        #region private fields

        #region fields

        private const double ScaleIncremental = 0.1;

        #endregion

        #endregion

        #region properties

        #region Propeties

        public ICommand ChooseGeometryTypeCommand { get; }

        #endregion

        #endregion

        #region fields

        private readonly IGeometryTypeManager _geometryTypeManager;
        private readonly ResourceDictionary _resourceDictionary;
        private readonly IShapeLayerManager _shapeLayerManager;

        private double _scale;


        private GeometryType? _selectedGeometryType;

        /// <summary>
        ///     当前选中的layer
        /// </summary>
        //public ShapeLayer SelectedShapeLayer { get; set; }
        private ShapeLayer _selectedShapeLayer;

        #endregion

        #region implementations

        /// <summary>
        ///     the sketch boar data manager used to manage sketch board
        /// </summary>
        public ISketchBoardDataManager SketchBoardDataManager { get; set; }


        private ObservableCollection<GeometryType> _geometryTypeList;

        /// <summary>
        ///     geometry type list
        /// </summary>
        public ObservableCollection<GeometryType> GeometryTypeList { get; }


        public ObservableCollection<ShapeLayer> Layers { get; set; }

        public ShapeLayer SelectedShapeLayer
        {
            get { return _selectedShapeLayer; }
            set
            {
                if (SetProperty(ref _selectedShapeLayer, value))
                {
                    SketchBoardDataManager.SetShapeLayer(_selectedShapeLayer);
                }
            }
        }

        /// <summary>
        ///     双击相对于图片位置
        /// </summary>
        public Point MouseDoubleClickPosition { get; set; }

        /// <summary>
        /// </summary>
        public GeometryType? SelectedGeometryType
        {
            get { return _selectedGeometryType; }
            set
            {
                SetProperty(ref _selectedGeometryType, value);
                if (_selectedGeometryType != null)
                {
                    SketchBoardDataManager.SetGeometryType(
                        _geometryTypeManager.GetGeometryTypeByName(_selectedGeometryType.Name));
                }
            }
        }

        /// <summary>
        ///     the image displayed
        /// </summary>
        private ImageSource _image;

        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }


        public double Scale
        {
            get { return _scale; }
            set { SetProperty(ref _scale, value); }
        }

        public ICommand ZoomOutCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ScaleToOriginalSizeCommand { get; set; }
        public ICommand ScaleToFitCommand { get; set; }
        public ICommand DeleteShapeCommand { get; }


        /// <summary>
        ///     if true, it will show canvas only, geometry list will be hidden
        /// </summary>
        private bool _showSimpleCanvas;

        public bool ShowSimpleCanvas
        {
            get { return _showSimpleCanvas; }
            set { SetProperty(ref _showSimpleCanvas, value); }
        }

        /// <summary>
        ///     use to control the visibility of tools
        /// </summary>
        public bool ShowShapeTypes { get; set; } = true;

        /// <summary>
        ///     show shapes only confirm to the conditions provided
        /// </summary>
        /// <param name="predicate"></param>
        public void FilterGeometryTypes(Expression<Func<GeometryType, bool>> predicate)
        {
            var func = predicate.Compile();
            GeometryTypeList.Clear();
            GeometryTypeList.AddRange(_geometryTypeList.Where(x => func(x)));
        }

        #endregion

        #region others

        private void ChooseGeometryTypeCommandImpl(GeometryType? geometryType)
        {
            if (geometryType == null)
            {
                return;
            }

            if (SelectedGeometryType != null)
            {
                SelectedGeometryType.IsSelected = false;
            }

            SelectedGeometryType = geometryType;
            SelectedGeometryType.IsSelected = true;
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
            var iconPngsFromResource = new Dictionary<string, Geometry?>
            {
                { nameof(Ellipse), _resourceDictionary["Ellipse"] as Geometry },
                { nameof(Rectangle), _resourceDictionary["Rectangle"] as Geometry },
                { nameof(Polygon), _resourceDictionary["Polygon"] as Geometry },
                { nameof(ThickenedCircle), _resourceDictionary["ThickenedCircle"] as Geometry },
                { nameof(ThickenedCross), _resourceDictionary["ThickenedCross"] as Geometry },
                { nameof(ThickenedRectangle), _resourceDictionary["ThickenedRectangle"] as Geometry },
                { nameof(ThickenedLine), _resourceDictionary["ThickenedLine"] as Geometry },
                { nameof(FixedCenterCircle), _resourceDictionary["FixedCenterCircle"] as Geometry }
            };


            Geometry? GetIconImage(string iconName)
            {
                return iconPngsFromResource.ContainsKey(iconName) ? iconPngsFromResource[iconName] : null;
            }


            _geometryTypeManager.RegisterGeometryType<GridGeometry>();
            _geometryTypeManager.RegisterGeometryType<GriddedRectangle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedCircle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedCross>();
            _geometryTypeManager.RegisterGeometryType<ThickenedRectangle>();
            _geometryTypeManager.RegisterGeometryType<ThickenedLine>();
            _geometryTypeManager.RegisterGeometryType<ArrowedLine>();
            _geometryTypeManager.RegisterGeometryType<Circle>();
            _geometryTypeManager.RegisterGeometryType<Cross>();
            _geometryTypeManager.RegisterGeometryType<Line>();

            _geometryTypeList = new ObservableCollection<GeometryType>(_geometryTypeManager.GetRegisteredGeometryTypes()
                .Select(x => new GeometryType(x, x, GetIconImage(x))));

            GeometryTypeList.AddRange(_geometryTypeList);
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