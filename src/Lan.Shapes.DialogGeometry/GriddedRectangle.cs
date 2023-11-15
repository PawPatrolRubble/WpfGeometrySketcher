#region

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Shapes;

#endregion

namespace Lan.Shapes.DialogGeometry
{
    public class GridData : NotifiableObject
    {
        private int _id;

        #region Propeties

        public Point BottomRight { get; set; }

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public Point TopLeft { get; set; }

        #endregion
    }

    public class GriddedRectangle : Rectangle
    {
        #region fields

        private List<LineGeometry> _verticalLines = new List<LineGeometry>();
        private List<LineGeometry> _horizontalLines = new List<LineGeometry>();

        private readonly GridGeometryParameter _gridGeometryParameter = new GridGeometryParameter();
        private GridData[,] _lines;

        #endregion

        #region Propeties

        private int _columnGap;
        private int _rowGap;

        public List<GridData>? GridData
        {
            get
            {
                if (_lines == null)
                {
                    return null;
                }

                return _lines.Cast<GridData>().ToList();
            }
        }

        public int TotalRow => _gridGeometryParameter.RowCount;
        public int TotalCol => _gridGeometryParameter.ColumnCount;
        public int RowGap => _rowGap;
        public int ColGap => _columnGap;


        private Point TopRight
        {
            get => new Point(BottomRight.X, TopLeft.Y);
        }

        private Point BottomLeft
        {
            get => new Point(TopLeft.X, BottomRight.Y);
        }


        #endregion

        #region Constructors

        public GriddedRectangle(ShapeLayer layer) : base(layer)
        {
        }

        #endregion

        #region local methods

        /// <summary>
        /// when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (IsGeometryRendered == false)
            {
                _dialogService.ShowDialog<GridDialog, GridDialogDialogViewModel>(
                    () => new GridDialogDialogViewModel(), x =>
                {
                    _gridGeometryParameter.RowCount = x.RowCount;
                    _gridGeometryParameter.ColumnCount = x.ColCount;
                });

                BottomRight = newPoint;

                UpdateOrAddLineGeometries();
                //RenderGeometryGroup.Children.AddRange(_horizontalLines);
                //RenderGeometryGroup.Children.AddRange(_verticalLines);
            }

            base.OnMouseLeftButtonUp(newPoint);
        }


        private void UpdateOrAddLineGeometries()
        {
            _rowGap = (int)((BottomRight.Y - TopLeft.Y) / _gridGeometryParameter.RowCount);
            _columnGap = (int)((BottomRight.X - TopLeft.X) / _gridGeometryParameter.ColumnCount);

            if (_lines == null)
            {
                _lines = new GridData[_gridGeometryParameter.RowCount, _gridGeometryParameter.ColumnCount];
                _verticalLines.AddRange(Enumerable.Range(0, _gridGeometryParameter.ColumnCount).Select(x => new LineGeometry()));
                _horizontalLines.AddRange(Enumerable.Range(0, _gridGeometryParameter.RowCount).Select(x => new LineGeometry()));

                RenderGeometryGroup.Children.AddRange(_verticalLines);
                RenderGeometryGroup.Children.AddRange(_horizontalLines);
            }

            for (var rowIndex = 0; rowIndex < _gridGeometryParameter.RowCount; rowIndex++)
            {
                _horizontalLines[rowIndex].StartPoint = TopLeft + new Vector(0, _rowGap * rowIndex);
                _horizontalLines[rowIndex].EndPoint = TopRight + new Vector(0, _rowGap * rowIndex);

                for (var colIndex = 0; colIndex < _gridGeometryParameter.ColumnCount; colIndex++)
                {
                    if (rowIndex == 0)
                    {
                        _verticalLines[colIndex].StartPoint = TopLeft + new Vector(_columnGap * colIndex, 0);
                        _verticalLines[colIndex].EndPoint = BottomLeft + new Vector(_columnGap * colIndex, 0);

                    }
                    if (_lines[rowIndex, colIndex] == null)
                    {
                        _lines[rowIndex, colIndex] ??= new GridData();
                        _lines[rowIndex, colIndex].Id = rowIndex * _gridGeometryParameter.ColumnCount + colIndex + 1;
                    }

                    var topLeft = TopLeft + new Vector(colIndex * _columnGap, rowIndex * _rowGap);
                    _lines[rowIndex, colIndex].TopLeft = topLeft;
                    _lines[rowIndex, colIndex].BottomRight = topLeft + new Vector(_columnGap, _rowGap);
                }
            }
        }

        public override void UpdateVisual()
        {
            if (IsGeometryRendered)
            {
                UpdateOrAddLineGeometries();
            }
            base.UpdateVisual();
        }


        private readonly DialogService _dialogService = new DialogService();
        public override void OnMouseLeftButtonDoubleClick(Point mouseDoubleClickPoint)
        {
            base.OnMouseLeftButtonDoubleClick(mouseDoubleClickPoint);
            _dialogService.ShowDialog<GridDataEditDialog, GridDataEditDialogViewModel>
            (
                ()=>
                {
                    var vm = new GridDataEditDialogViewModel();
                    vm.GridDataList.AddRange(GridData);
                    return vm;
                },
                x =>
                {
                    if (GridData != null)
                    {
                        GridData.Clear();
                        GridData.AddRange(x.GridDataList);
                    }
                });
        }

        #endregion
    }
}