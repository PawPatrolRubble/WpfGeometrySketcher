#nullable enable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes;

namespace Lan.SketchBoard
{
    public class SketchBoard : Canvas
    {
        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            "SketchBoardDataManager", typeof(ISketchBoardDataManager), typeof(SketchBoard),
            new PropertyMetadata(default(ISketchBoardDataManager), OnSketchBoardDataManagerChangedCallBack));

        private static void OnSketchBoardDataManagerChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is SketchBoard sketchBoard && e.NewValue is ISketchBoardDataManager dataManager)
            {
                dataManager.VisualCollection = new VisualCollection(sketchBoard);
            }
        }

        public ISketchBoardDataManager? SketchBoardDataManager
        {
            get { return (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty); }
            set { SetValue(SketchBoardDataManagerProperty, value); }
        }


        #region overrides

        protected override int VisualChildrenCount => SketchBoardDataManager?.VisualCollection.Count ?? 0;

        protected override Visual GetVisualChild(int index)
        {
            return SketchBoardDataManager?.VisualCollection[index] ?? default(ShapeVisual);
        }

        #endregion


        #region events handling

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            SketchBoardDataManager?.SelectedShape?.OnMouseLeftButtonDown(e.GetPosition(this));
            //hit test if any shape is being selected
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            SketchBoardDataManager?.SelectedShape?.OnMouseMove(e.GetPosition(this), e.MouseDevice.LeftButton);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SketchBoardDataManager?.SelectedShape?.OnMouseLeftButtonUp(e.GetPosition(this));
        }

        #endregion
    }
}