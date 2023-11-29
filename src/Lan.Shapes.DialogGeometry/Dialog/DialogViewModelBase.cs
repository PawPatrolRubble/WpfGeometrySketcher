using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class DialogViewModelBase : INotifyPropertyChanged
    {
        #region constructor

        public DialogViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
            OkCommand = new RelayCommand(Ok);
        }

        #endregion

        #region properties

        public Action RequestClose { get; set; }

        public ICommand CloseCommand { get; set; }
        public ICommand OkCommand { get; set; }

        #endregion

        #region interface implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region other members

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void Close()
        {
            RequestClose?.Invoke();
        }

        protected virtual void Ok()
        {
            RequestClose?.Invoke();
        }

        #endregion
    }
}