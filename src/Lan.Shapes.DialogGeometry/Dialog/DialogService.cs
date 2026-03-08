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

            var window = new DialogWindow()
            {
                Content = view,
            };

            if (vm is DialogViewModelBase dialogViewModel)
            {
                dialogViewModel.RequestClose += () =>
                {
                    window.Close();
                };

                window.Closing += (s, e) =>
                {
                    // Only set to Close if not already set by OK/Cancel
                    if (dialogViewModel.Result == DialogResult.None)
                    {
                        dialogViewModel.Result = DialogResult.Close;
                    }
                };
            }

            window.Closed += (s, e) =>
            {
                closedCallback(vm);
            };

            window.ShowDialog();
        }
    }
}