using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Lan.Shapes.DialogGeometry.Dialog;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.DialogGeometry
{
    public class GridGeometry : ShapeVisualBase, IDataExport<PointsData>
    {
        #region fields

        private readonly RectangleGeometry _boundGeometry = new RectangleGeometry();
        private Point _topLeft;
        private Point _bottomRight;
        private LineGeometry[,] _lines;

        #endregion

        #region constructors

        public GridGeometry(ShapeLayer layer) : base(layer)
        {
            RenderGeometryGroup.Children.Add(_boundGeometry);
        }

        #endregion

        #region properties

        public Point TopLeft
        {
            get => _topLeft;
            set
            {
                _topLeft = value;
                OnTopLeftPointChanges(_topLeft);
            }
        }

        private void OnTopLeftPointChanges(Point topLeft)
        {

            _boundGeometry.Rect = new Rect(topLeft, (BottomRight.X == 0 && BottomRight.Y == 0) ? topLeft : BottomRight);
            UpdateVisual();
        }

        public Point BottomRight
        {
            get => _bottomRight;
            set
            {
                _bottomRight = value;
                OnBottomRightChanges(_bottomRight);
            }
        }

        private void OnBottomRightChanges(Point bottomRight)
        {
            _boundGeometry.Rect = new Rect(TopLeft, bottomRight);
            UpdateVisual();
        }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public int RowGap { get; set; }
        public int ColumnGap { get; set; }

        /// <summary>
        /// </summary>
        public override Rect BoundsRect { get; }

        #endregion

        #region interface implementations

        public void FromData(PointsData data)
        {

        }

        public PointsData GetMetaData()
        {
            return new PointsData(1, new List<Point>());
        }

        #endregion

        #region all other members

        protected override void CreateHandles()
        {
        }

        protected override void HandleResizing(Point point)
        {
        }

        protected override void HandleTranslate(Point newPoint)
        {
        }

        /// <summary>
        ///     未选择状态
        /// </summary>
        public override void OnDeselected()
        {
        }

        /// <summary>
        ///     选择时
        /// </summary>
        public override void OnSelected()
        {
        }


        /// <summary>
        ///     left mouse button down event
        /// </summary>
        /// <param name="mousePoint"></param>
        public override void OnMouseLeftButtonDown(Point mousePoint)
        {
            base.OnMouseLeftButtonDown(mousePoint);
            if (IsGeometryRendered == false) TopLeft = mousePoint;
        }

        public override void OnMouseMove(Point point, MouseButtonState buttonState)
        {
            if (IsGeometryRendered == false)
            {
                BottomRight = point;
            }
        }


        /// <summary>
        ///     when mouse left button up
        /// </summary>
        /// <param name="newPoint"></param>
        public override void OnMouseLeftButtonUp(Point newPoint)
        {
            if (IsGeometryRendered == false)
            {
                var dialog = new DialogService();
                dialog.ShowDialog<GridDialog, GridDialogDialogViewModel>(() => new GridDialogDialogViewModel(), x =>
                {
                    RowCount = x.RowCount;
                    ColumnCount = x.ColCount;
                    RowGap = x.VerticalGap;
                    ColumnGap = x.HorizontalGap;
                });
                BottomRight = newPoint;
                _boundGeometry.Rect = new Rect(TopLeft, BottomRight);
                IsGeometryRendered = true;

                RowGap = (int)((BottomRight.Y - TopLeft.Y) / RowCount);
                ColumnGap = (int)((BottomRight.X - TopLeft.X) / ColumnCount);

                UpdateOrAddLineGeometries(true);
                //RenderGeometryGroup.Children.AddRange(_horizontalLines);
                //RenderGeometryGroup.Children.AddRange(_verticalLines);
            }

            UpdateVisual();
        }

        private void UpdateOrAddLineGeometries(bool addNew)
        {
            if (_lines == null)
            {
                _lines = new LineGeometry[RowCount, ColumnCount];

            }

            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {

                for (var colIndex = 0; colIndex < ColumnCount; colIndex++)
                {
                    if (_lines[rowIndex, colIndex] == null)
                    {

                        _lines[rowIndex, colIndex] ??= new LineGeometry();
                        RenderGeometryGroup.Children.Add(_lines[rowIndex, colIndex]);
                    }
                    var topLeft = TopLeft + new Vector(colIndex * ColumnGap, rowIndex * RowGap);
                    _lines[rowIndex, colIndex].StartPoint = topLeft;
                    _lines[rowIndex, colIndex].EndPoint = topLeft + new Vector(ColumnGap, RowGap);

                }
            }

        }

        #endregion
    }
}