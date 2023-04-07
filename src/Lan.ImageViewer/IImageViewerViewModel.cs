using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes;
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
        IEnumerable<GeometryType> GeometryTypeList { get; }

        /// <summary>
        ///
        /// </summary>
        GeometryType SelectedGeometryType { get; }

        /// <summary>
        /// the image displayed
        /// </summary>
        ImageSource Image { get; }

        double Scale { get; set; }


        #region commands

        ICommand ZoomOutCommand { get; }
        ICommand ZoomInCommand { get; }
        ICommand ScaleToOriginalSizeCommand { get; }
        ICommand ScaleToFitCommand { get; }

        #endregion
    }
}
