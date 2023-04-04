using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes.Shapes;
using Lan.Shapes.Styler;
using Lan.SketchBoard;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Lan.Shapes.TestApp
{
    public class MainWindowViewModel:ObservableObject
    {
        public MainWindowViewModel(ISketchBoardDataManager sketchBoardDataManager)
        {
            SketchBoardDataManager = sketchBoardDataManager;
            SketchBoardDataManager.RegisterDrawingTool("Rectangle", typeof(Rectangle));
            SketchBoardDataManager.RegisterDrawingTool("Ellipse", typeof(Ellipse));
            SketchBoardDataManager.RegisterDrawingTool(nameof(Polygon), typeof(Polygon));
            SelectOneShapeCommand= new RelayCommand(SelectOneShapeCommandImpl);
            GetShapeInfoCommand= new RelayCommand(GetShapeInfoCommandImpl);
        }

        public ISketchBoardDataManager  SketchBoardDataManager { get; set; }

        public ICommand SelectOneShapeCommand { get; private set; }
        private void SelectOneShapeCommandImpl()
        {
            SketchBoardDataManager.SelectDrawingTool(nameof(Polygon), new ShapeStylerFactory().CustomShapeStyler(Brushes.Transparent, Brushes.Red, 5,15));
        }

        public ICommand GetShapeInfoCommand { get; private set; }
        private void GetShapeInfoCommandImpl()
        {
            var shapeVisual = SketchBoardDataManager.SelectedShape;

        }
    }
}