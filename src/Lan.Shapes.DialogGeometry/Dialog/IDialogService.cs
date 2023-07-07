using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lan.Shapes.DialogGeometry.Dialog
{
    public interface IDialogService
    {

        void ShowDialog<TV, TVm>(Func<TVm> viewModel, Action<TVm> closedCallback) where TV:FrameworkElement,new () where TVm:INotifyPropertyChanged;
    }
}
