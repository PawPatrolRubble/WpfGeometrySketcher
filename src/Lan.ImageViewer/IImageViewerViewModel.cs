using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Interfaces;
using Lan.SketchBoard;

namespace Lan.ImageViewer
{
    public interface IImageViewerViewModel
    {
        /// <summary>
        /// the sketch boar data manager used to manage sketch board
        /// </summary>
        ISketchBoardDataManager SketchBoardDataManager { get; }

        /// <summary>
        /// geometry type list
        /// </summary>
        ObservableCollection<GeometryType> GeometryTypeList { get; }

        /// <summary>
        ///
        /// </summary>
        GeometryType SelectedGeometryType { get; }

        /// <summary>
        /// the image displayed
        /// </summary>
        ImageSource Image { get; set; }

        double Scale { get; set; }

        ObservableCollection<ShapeLayer> Layers { get; set; }

        /// <summary>
        /// 当前选中的layer
        /// </summary>
        ShapeLayer SelectedShapeLayer { get; set; }

        /// <summary>
        /// 双击相对于图片位置
        /// </summary>
        Point MouseDoubleClickPosition { get; set; }

        #region commands

        ICommand ZoomOutCommand { get; }
        ICommand ZoomInCommand { get; }
        ICommand ScaleToOriginalSizeCommand { get; }
        ICommand ScaleToFitCommand { get; }

        /// <summary>
        /// if true, it will show canvas only, geometry list will be hidden
        /// </summary>
        bool ShowSimpleCanvas { get; set; }

        /// <summary>
        /// use to control the visibility of tools
        /// </summary>
        bool ShowShapeTypes { get; set; }


        /// <summary>
        /// show shapes only confirm to the conditions provided
        /// </summary>
        /// <param name="predicate"></param>
        void FilterGeometryTypes(Expression<Func<GeometryType, bool>> predicate);

        #endregion
    }
}
