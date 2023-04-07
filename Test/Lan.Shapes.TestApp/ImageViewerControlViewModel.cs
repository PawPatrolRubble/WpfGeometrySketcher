#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lan.ImageViewer;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;
using Lan.SketchBoard;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Lan.Shapes.App
{
    public class ImageViewerControlViewModel : ObservableObject, IImageViewerViewModel
    {

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

        public ImageViewerControlViewModel()
        {
            _shapeStyler = new ShapeStylerFactory().CustomShapeStyler(Brushes.Transparent, Brushes.Red, 5, 15);

            SketchBoardDataManager = new SketchBoardDataManager();

            SketchBoardDataManager.SetShapeLayer(ShapeLayer.CreateLayer(_shapeStyler, 1, "mask", "mask layer"));
            SketchBoardDataManager.RegisterGeometryType("Rectangle", typeof(Rectangle));
            SketchBoardDataManager.RegisterGeometryType("Ellipse", typeof(Ellipse));
            SketchBoardDataManager.RegisterGeometryType(nameof(Polygon), typeof(Polygon));

            GeometryTypeList = new List<GeometryType>(SketchBoardDataManager.GetRegisteredGeometryTypes()
                .Select(x => new GeometryType(x, x, null)));
            Image = new BitmapImage(new Uri("pack://application:,,,/Lan.Shapes.App;component/reference.png"));
        }


    }
}
