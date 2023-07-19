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


        #endregion

        #region Propeties


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
            SizeChanged += SketchBoard_SizeChanged;
        }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Delete && SketchBoardDataManager?.SelectedGeometry != null)
            {
                SketchBoardDataManager?.RemoveShape(SketchBoardDataManager.SelectedGeometry);
            }
        }

        private void SketchBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(SketchBoardDataManager==null)return;
            Console.WriteLine($"new canvas size: {e.NewSize}");
            var scaleFactor = CalculateStrokeThickness();
            var stylers = SketchBoardDataManager.CurrentShapeLayer.Stylers;

            foreach (var shapeStyler in stylers)
            {
                shapeStyler.Value.SketchPen.Thickness = 2 * scaleFactor;
                shapeStyler.Value.DragHandleSize = 10 * scaleFactor;
            }
            Console.WriteLine($"stroke thickness{scaleFactor}");
        }

        private double CalculateStrokeThickness()
        {
            return Math.Pow(1.8, Math.Log2(ActualWidth + ActualHeight) - 10);
        }

        #region others

        private static void OnSketchBoardDataManagerChangedCallBack(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is SketchBoard sketchBoard && e.NewValue is ISketchBoardDataManager dataManager)
            {
                var oldShapes = dataManager.Shapes;

                dataManager.InitializeVisualCollection(sketchBoard);
                if (oldShapes != null)
                {

                    foreach (var shape in oldShapes)
                    {
                        dataManager.AddShape(shape);
                    }
                }
            }
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
            SketchBoardDataManager?.CurrentGeometryInEdit?.OnMouseRightButtonUp(e.GetPosition(this));
            SketchBoardDataManager?.UnselectGeometry();

            base.OnMouseRightButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                this.Focus();
                SketchBoardDataManager.SelectedGeometry = GetHitTestShape(e.GetPosition(this));

                if (SketchBoardDataManager.SelectedGeometry == null)
                    SketchBoardDataManager.SelectedGeometry = SketchBoardDataManager?.CurrentGeometryInEdit ??
                                                              SketchBoardDataManager?.CreateNewGeometry(e.GetPosition(this));

                //if sketchboard current geometry is not null, it means that it still being sketched
                //and we need to assign it to active shape
                SketchBoardDataManager.SelectedGeometry?.OnMouseLeftButtonDown(e.GetPosition(this));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private ShapeVisualBase? GetHitTestShape(Point mousePosition)
        {
            if ((SketchBoardDataManager.SelectedGeometry?.IsBeingDraggedOrPanMoving ?? false)
                && !SketchBoardDataManager.SelectedGeometry.IsLocked)
            {
                return SketchBoardDataManager.SelectedGeometry;
            }

            //Debug.WriteLine($"it is not dragged, active shape: {ActiveShape?.GetType().Name}");
            ShapeVisualBase? shape = null;

            var hitTestResult = VisualTreeHelper.HitTest(this, mousePosition);

            if (hitTestResult != null) shape = hitTestResult.VisualHit as ShapeVisualBase;

            return shape?.IsLocked ?? true ? null : shape;
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    if (SketchBoardDataManager?.CurrentGeometryInEdit != null)
                    {
                        SketchBoardDataManager?.CurrentGeometryInEdit?.OnMouseMove(e.GetPosition(this), e.LeftButton);
                    }
                    else
                    {
                        SketchBoardDataManager.SelectedGeometry = GetHitTestShape(e.GetPosition(this));
                        SketchBoardDataManager.SelectedGeometry?.OnMouseMove(e.GetPosition(this), e.LeftButton);
                    }
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
                SketchBoardDataManager.SelectedGeometry?.OnMouseLeftButtonUp(e.GetPosition(this));
                //when the active shape is not a newly created geometry, when mouse left button up 
                if (SketchBoardDataManager.SelectedGeometry?.IsGeometryRendered ?? false)
                {
                    SketchBoardDataManager?.UnselectGeometry();
                }

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