using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lan.SketchBoard
{
    public class SketchBoard : Canvas
    {
        
        public static readonly DependencyProperty SketchBoardDataProperty = DependencyProperty.Register(
            "SketchBoardData", typeof(ISketchBoardDataManager), typeof(SketchBoard),
            new PropertyMetadata(default(ISketchBoardDataManager)));

        public ISketchBoardDataManager SketchBoardData
        {
            get { return (ISketchBoardDataManager)GetValue(SketchBoardDataProperty); }
            set { SetValue(SketchBoardDataProperty, value); }
        }


        #region events handling

        private Point _mouseDownPosition;



        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised on this element. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //if not shape tool is selected return
            if (string.IsNullOrEmpty(SketchBoardData.SelectedDrawingShape))
            {
                return;
            }

            var p = _mouseDownPosition = e.GetPosition(this);

            //hit test if any shape is being selected

        }

        #endregion


    }
}