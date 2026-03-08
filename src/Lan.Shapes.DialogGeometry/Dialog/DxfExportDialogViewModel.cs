namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class DxfExportDialogViewModel : DialogViewModelBase
    {
        private double _topLeftX;
        private double _topLeftY;
        private double _pixelToMmFactor = 1.0;

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

        public double PixelToMmFactor
        {
            get => _pixelToMmFactor;
            set => SetField(ref _pixelToMmFactor, value);
        }
    }
}
