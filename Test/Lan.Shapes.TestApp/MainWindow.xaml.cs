using System.Windows;

namespace Lan.Shapes.App
{
    public partial class MainWindow : Window
    {
     




        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

       

    }
}