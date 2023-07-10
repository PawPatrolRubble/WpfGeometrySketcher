using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Lan.Shapes.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        }
    }
}