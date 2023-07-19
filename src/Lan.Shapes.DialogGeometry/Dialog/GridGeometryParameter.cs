namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class GridGeometryParameter : NotifiableObject
    {
        private int _rowCount;

        public int RowCount
        {
            get => _rowCount;
            set { SetProperty(ref _rowCount, value); }
        }

        private int _columnCount;

        public int ColumnCount
        {
            get => _columnCount;
            set { SetProperty(ref _columnCount, value); }
        }
    }
}