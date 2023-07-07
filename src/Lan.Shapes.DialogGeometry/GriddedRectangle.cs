using System.Windows;
using System.Windows.Media;
using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.DialogGeometry
{
    public class GridData
    {
        public int Id { get; set; }
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }

    }

    public class GriddedRectangle : Rectangle
    {
        private GridData[,] _lines;

        public int RowGap { get; set; }
        public int ColumnGap { get; set; }

        private GridGeometryParameter _gridGeometryParameter = new GridGeometryParameter();

        public GriddedRectangle(ShapeLayer layer) : base(layer)
        {

        }

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (IsGeometryRendered == false)
            {
                var dialog = new DialogService();
                dialog.ShowDialog<GridDialog, GridDialogDialogViewModel>(() => new GridDialogDialogViewModel(), x =>
                {
                    _gridGeometryParameter.RowCount = x.RowCount;
                    _gridGeometryParameter.ColumnCount = x.ColCount;
                });
                BottomRight = newPoint;

                RowGap = (int)((BottomRight.Y - TopLeft.Y) / _gridGeometryParameter.RowCount);
                ColumnGap = (int)((BottomRight.X - TopLeft.X) / _gridGeometryParameter.ColumnCount);

                UpdateOrAddLineGeometries();
                //RenderGeometryGroup.Children.AddRange(_horizontalLines);
                //RenderGeometryGroup.Children.AddRange(_verticalLines);
            }

            base.OnMouseLeftButtonUp(newPoint);
        }

        private void UpdateOrAddLineGeometries()
        {
            if (_lines == null)
            {
                _lines = new GridData[_gridGeometryParameter.RowCount, _gridGeometryParameter.ColumnCount];
            }

            for (var rowIndex = 0; rowIndex < _gridGeometryParameter.RowCount; rowIndex++)
            {
                for (var colIndex = 0; colIndex < _gridGeometryParameter.ColumnCount; colIndex++)
                {
                    if (_lines[rowIndex, colIndex] == null)
                    {
                        _lines[rowIndex, colIndex] ??= new GridData();
                        _lines[rowIndex, colIndex].Id = rowIndex * _gridGeometryParameter.ColumnCount + colIndex;
                    }

                    var topLeft = TopLeft + new Vector(colIndex * ColumnGap, rowIndex * RowGap);
                    _lines[rowIndex, colIndex].TopLeft = topLeft;
                    _lines[rowIndex, colIndex].BottomRight = topLeft + new Vector(ColumnGap, RowGap);
                }
            }
        }



    }
}