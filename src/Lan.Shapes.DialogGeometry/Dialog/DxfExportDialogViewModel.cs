namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class DxfExportDialogViewModel : DialogViewModelBase
    {
        private double _topLeftX;
        private double _topLeftY;

        public double TopLeftX
        {
            get => _topLeftX;
            set => SetField(ref _topLeftX, value);
        }

        public double TopLeftY
        {
            get => _topLeftY;
            set => SetField(ref _topLeftY, value);
        }

        private bool _reverseYAxis;

        public bool ReverseYAxis
        {
            get => _reverseYAxis;
            set { SetField(ref _reverseYAxis, value); }
        }

    }
}
