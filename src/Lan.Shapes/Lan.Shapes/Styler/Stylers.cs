using System.Windows.Media;

namespace Lan.Shapes.Styler
{
    public  class Stylers
    {

        public static IShapeStyler SelectedStyler { get; } =
            new ShapeStyler(Brushes.Red, Brushes.Lime, DashStyles.Solid);

        public static IShapeStyler UnselectedStyler { get; } =
            new ShapeStyler(Brushes.Transparent, Brushes.Lime, DashStyles.Solid);

    }
}