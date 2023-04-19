#nullable enable
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Lan.Shapes;

namespace Lan.SketchBoard
{
    public class SketchBoard : Canvas
    {
        #region fields

        private ShapeVisualBase? _activeShape;
        private ShapeVisualBase? ActiveShape
        {
            get => _activeShape;
            set
            {
                if (_activeShape != value)
                {
                    if (_activeShape != null) _activeShape.State = ShapeVisualState.Normal;
                }
                _activeShape = value;

            }
        }

        #endregion

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
            return SketchBoardDataManager?.VisualCollection[index] ?? throw new InvalidOperationException();
        }

        #endregion


        #region events handling

        /// <summary>
        /// right click the mouse means ending the drawing of current shape
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            SketchBoardDataManager?.CurrentGeometry?.OnMouseRightButtonUp(e.GetPosition(this));
            SketchBoardDataManager?.UnselectGeometry();

            base.OnMouseRightButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                ActiveShape = GetHitTestShape(e.GetPosition(this));

                if (ActiveShape == null)
                {
                    ActiveShape = SketchBoardDataManager?.CurrentGeometry ?? SketchBoardDataManager?.CreateNewGeometry(e.GetPosition(this));
                }

                //if sketchboard current geometry is not null, it means that it still being sketched
                //and we need to assign it to active shape

                ActiveShape?.OnMouseLeftButtonDown(e.GetPosition(this));

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private ShapeVisualBase? GetHitTestShape(Point mousePosition)
        {
            //Debug.WriteLine($"active: {ActiveShape == null}, active state: {ActiveShape?.State}");
            if (ActiveShape?.IsBeingDraggedOrPanMoving ?? false)
            {
                //Debug.WriteLine($"is being dragged, {ActiveShape?.GetType().Name}");
                return ActiveShape;
            }

            //Debug.WriteLine($"it is not dragged, active shape: {ActiveShape?.GetType().Name}");
            ShapeVisualBase? shape = null;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, mousePosition);

            if (hitTestResult != null)
            {
                shape = hitTestResult.VisualHit as ShapeVisualBase;
            }


            return shape;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (SketchBoardDataManager?.CurrentGeometry != null)
                {
                    SketchBoardDataManager?.CurrentGeometry?.OnMouseMove(e.GetPosition(this), e.LeftButton);
                }
                else
                {
                    ActiveShape = GetHitTestShape(e.GetPosition(this));
                    Debug.WriteLine($"{ActiveShape?.GetType().Name}");
                    ActiveShape?.OnMouseMove(e.GetPosition(this), e.LeftButton);
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
                //when the active shape is not a newly created geometry, when mouse left button up 
                if (ActiveShape?.IsGeometryRendered ?? false)
                {
                    SketchBoardDataManager?.UnselectGeometry();

                }
                ActiveShape?.OnMouseLeftButtonUp(e.GetPosition(this));

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