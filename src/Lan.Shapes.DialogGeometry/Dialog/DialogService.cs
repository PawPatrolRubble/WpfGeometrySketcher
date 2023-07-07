using System;
using System.ComponentModel;
using System.Windows;

namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class DialogService : IDialogService
    {
        public void ShowDialog<TV, TVm>(Func<TVm> viewModel, Action<TVm> closedCallback) where TV : FrameworkElement, new() where TVm : INotifyPropertyChanged
        {
            var view = new TV();
            var vm = viewModel();

            view.DataContext = vm;

            var window = new Window();
            window.Content = view;
            window.Width = view.Width;
            window.Height = view.Height;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (vm is DialogViewModelBase dialogViewModel)
            {
                dialogViewModel.RequestClose +=()=>
                {
                    closedCallback(vm);
                    window.Close();
                };

            }

            window.ShowDialog();
        }
    }
}