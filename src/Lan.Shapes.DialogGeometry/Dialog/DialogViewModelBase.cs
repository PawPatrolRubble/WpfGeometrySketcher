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
    public enum DialogResult
    {
        None,
        Ok,
        Cancel,
        Close
    }

    public class DialogViewModelBase : INotifyPropertyChanged
    {
        public Action RequestClose { get; set; }
        public DialogResult Result { get; set; } = DialogResult.None;

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

        public ICommand CloseCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand OkCommand { get; set; }

        public DialogViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
            CancelCommand = new RelayCommand(Cancel);
            OkCommand = new RelayCommand(Ok);
        }

        protected virtual void Ok()
        {
            Result = DialogResult.Ok;
            RequestClose?.Invoke();
        }

        protected virtual void Cancel()
        {
            Result = DialogResult.Cancel;
            RequestClose?.Invoke();
        }

        public virtual void Close()
        {
            Result = DialogResult.Close;
            RequestClose?.Invoke();
        }
    }


}
