using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class DialogViewModelBase : INotifyPropertyChanged
    {
        public Action RequestClose { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void Close()
        {
            RequestClose?.Invoke();
        }

        public ICommand CloseCommand { get; set; }
        public ICommand OkCommand { get; set; }

        public DialogViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
            OkCommand = new RelayCommand(Ok);
        }

        protected virtual void Ok()
        {
            RequestClose?.Invoke();

        }
    }


}
