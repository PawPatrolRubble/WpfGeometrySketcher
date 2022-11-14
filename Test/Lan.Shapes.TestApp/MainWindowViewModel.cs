using System.Windows.Input;
using Lan.Shapes.Shapes;
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
            SketchBoardDataManager.RegisterDrawingTool("rectangle", typeof(Rectangle));
            SelectOneShapeCommand= new RelayCommand(SelectOneShapeCommandImpl);

            
        }

        public ISketchBoardDataManager  SketchBoardDataManager { get; set; }

        public ICommand SelectOneShapeCommand { get; private set; }
        private void SelectOneShapeCommandImpl()
        {
            SketchBoardDataManager.SelectDrawingTool("rectangle");
        }
    }
}