using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Lan.Shapes.DialogGeometry.Dialog
{
    public class GridDataEditDialogViewModel:DialogViewModelBase
    {

        public ObservableCollection<GridData> GridDataList { get; set; }

        public GridDataEditDialogViewModel()
        {
            GridDataList = new ObservableCollection<GridData>();
        }

        private GridData _selectedGridData;

        public GridData SelectedGridData
        {
            get => _selectedGridData;
            set { SetField(ref _selectedGridData, value); }
        }

    }
}
