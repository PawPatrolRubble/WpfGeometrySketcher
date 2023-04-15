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



        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(SketchBoard), new PropertyMetadata(default(ImageSource)));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }


        #region overrides

        protected override int VisualChildrenCount => SketchBoardDataManager?.VisualCollection.Count ?? 0;

        protected override Visual GetVisualChild(int index)
        {
            return SketchBoardDataManager?.VisualCollection[index] ?? default(ShapeVisualBase);
        }

        #endregion


        #region events handling

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                Point mousePos = e.GetPosition(this);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, mousePos);

                if (hitTestResult != null && hitTestResult.VisualHit is ShapeVisualBase visual)
                {
                    // Do something with the clicked DrawingVisual
                    // visual.Transform.Transform(mousePos) will give the position of the mouse click relative to the DrawingVisual
                    visual.OnMouseLeftButtonDown(e.GetPosition(this));
                }
                else
                {
                    SketchBoardDataManager?.SelectedShape?.OnMouseLeftButtonDown(e.GetPosition(this));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                Point mousePos = e.GetPosition(this);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, mousePos);

                if (hitTestResult != null && hitTestResult.VisualHit is ShapeVisualBase visual)
                {
                    visual.OnMouseMove(e.GetPosition(this), e.LeftButton);
                }
                else
                {
                    SketchBoardDataManager?.SelectedShape?.OnMouseMove(mousePos,e.LeftButton);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            try
            {
                SketchBoardDataManager?.MouseUpHandler(e.GetPosition(this));
                //SketchBoardDataManager?.SelectedShape?.OnMouseLeftButtonUp(e.GetPosition(this));

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}