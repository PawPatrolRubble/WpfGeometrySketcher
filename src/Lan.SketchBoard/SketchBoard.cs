#nullable enable

#region

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Enums;
using Lan.Shapes.Interfaces;

#endregion

namespace Lan.SketchBoard
{
    public class SketchBoard : Canvas
    {
        #region fields

        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            "SketchBoardDataManager", typeof(ISketchBoardDataManager), typeof(SketchBoard),
            new PropertyMetadata(default(ISketchBoardDataManager), OnSketchBoardDataManagerChangedCallBack));


        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(ImageSource), typeof(SketchBoard), new PropertyMetadata(default(ImageSource)));

        private ShapeVisualBase? _activeShape;

        #endregion

        #region Propeties

        private ShapeVisualBase? ActiveShape
        {
            get => _activeShape;
            set
            {
                if (_activeShape != value)
                    if (_activeShape != null)
                        _activeShape.State = ShapeVisualState.Normal;
                _activeShape = value;
            }
        }

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public ISketchBoardDataManager? SketchBoardDataManager
        {
            get => (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty);
            set => SetValue(SketchBoardDataManagerProperty, value);
        }

        #endregion


        public SketchBoard()
        {
            Loaded += (s, e) =>
            {
                if (Application.Current.MainWindow != null)
                    Application.Current.MainWindow.KeyDown += (s, e) =>
                    {
                        if (e.Key == Key.Delete && ActiveShape!=null)
                        {
                            SketchBoardDataManager?.RemoveShape(ActiveShape);
                        }
                    };
            };
        }

        #region others

        private static void OnSketchBoardDataManagerChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is SketchBoard sketchBoard && e.NewValue is ISketchBoardDataManager dataManager)
                dataManager.VisualCollection = new VisualCollection(sketchBoard);
        }

        #endregion


        #region overrides

        protected override int VisualChildrenCount
        {
            get => SketchBoardDataManager?.VisualCollection.Count ?? 0;
        }

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
                    ActiveShape = SketchBoardDataManager?.CurrentGeometry ??
                                  SketchBoardDataManager?.CreateNewGeometry(e.GetPosition(this));

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
                //Debug.WriteLine($"is being dragged, {ActiveShape?.GetType().Name}");
                return ActiveShape;

            //Debug.WriteLine($"it is not dragged, active shape: {ActiveShape?.GetType().Name}");
            ShapeVisualBase? shape = null;

            var hitTestResult = VisualTreeHelper.HitTest(this, mousePosition);

            if (hitTestResult != null) shape = hitTestResult.VisualHit as ShapeVisualBase;


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
                if (ActiveShape?.IsGeometryRendered ?? false) SketchBoardDataManager?.UnselectGeometry();
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