namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class GridDialogDialogViewModel : DialogViewModelBase
    {
        #region fields

        private int _colCount;

        private int _horizontalGap;
        private int _rowCount;

        private int _verticalGap;

        #endregion

        #region constructors

        #endregion


        #region Propeties

        public int ColCount
        {
            get => _colCount;
            set => SetField(ref _colCount, value);
        }

        public int VerticalGap
        {
            get => _verticalGap;
            set => SetField(ref _verticalGap, value);
        }

        public int RowCount
        {
            get => _rowCount;
            set => SetField(ref _rowCount, value);
        }

        public int HorizontalGap
        {
            get => _horizontalGap;
            set => SetField(ref _horizontalGap, value);
        }

        private bool _evenAllocation;

        public bool EvenAllocation
        {
            get => _evenAllocation;
            set => SetField(ref _evenAllocation, value);
        }

        #endregion
    }
}