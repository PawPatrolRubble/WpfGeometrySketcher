#region

using System;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes.Handle;

#endregion

namespace Lan.Shapes.Custom
{
    public abstract class CustomGeometryBase : ShapeVisualBase
    {
        #region fields

        protected double MaxStrokeThickness { get; private set; }

        #endregion

        #region fields

        protected readonly DragHandle
            DistanceResizeHandle; //= new RectDragHandle(new Size(10, 10), new Point(), 10, 99);

        protected readonly SolidColorBrush DragHandleFillColor = Brushes.Aquamarine;
        protected readonly Pen DragHandlePen; // = new Pen(Brushes.Red, 1);
        private double _strokeThickness = 15;
        protected Pen? Pen;

        #endregion

        #region Propeties

        /// <summary>
        /// 
        /// </summary>
        public override Rect BoundsRect { get; }

        protected double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                _strokeThickness = value;
                _strokeThickness = Math.Min(MaxStrokeThickness, _strokeThickness);
                _strokeThickness = Math.Max(0, _strokeThickness);

                if (Pen != null)
                {
                    Pen.Thickness = StrokeThickness;
                }

                //update handle position, when stroke thickness changes
                OnStrokeThicknessChanges(_strokeThickness);
            }
        }

        #endregion

        #region Constructors

        public CustomGeometryBase(ShapeLayer shapeLayer) : base(shapeLayer)
        {
            DistanceResizeHandle = new RectDragHandle(new Size(DragHandleSize, DragHandleSize), new Point(), 10, 99);
            DragHandlePen = ShapeStyler.SketchPen;
            MaxStrokeThickness = shapeLayer.MaximumThickenedShapeWidth;
        }

        #endregion

        #region others

        protected override void CreateHandles()
        {
        }

        protected override void HandleResizing(Point point)
        {
            throw new NotImplementedException();
        }

        protected override void HandleTranslate(Point newPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未选择状态
        /// </summary>
        public override void OnDeselected()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 选择时
        /// </summary>
        public override void OnSelected()
        {
            throw new NotImplementedException();
        }

        protected override void OnDragHandleSizeChanges(double dragHandleSize)
        {
            if (DistanceResizeHandle != null)
            {
                DistanceResizeHandle.HandleSize = new Size(dragHandleSize, dragHandleSize);
            }
        }

        protected abstract void OnStrokeThicknessChanges(double strokeThickness);

        #endregion
    }
}