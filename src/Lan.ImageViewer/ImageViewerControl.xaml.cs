#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

#endregion

namespace Lan.ImageViewer
{
    /// <summary>
    /// Interaction logic for ImageViewerControl.xaml
    /// </summary>
    public partial class ImageViewerControl : UserControl
    {
        #region Constructors

        public ImageViewerControl()
        {
            InitializeComponent();
        }

        #endregion

        #region local methods

        private void AnimateGrids(EventHandler postAnimation, double commGridWidth, double mainGridWidth)

        {
            /* 

             *  If commGrid is right 

             *  commGrid.Margin.Left should move from 0 to -mainGridWidth 

             *  commGrid.Margin.Right should move from 0 to +mainGridWidth 

             *   

             *  If commGrid is left 
             *  commGrid.Margin.Left should move from 0 to +mainGridWidth 
             *  commGrid.Margin.Right should move from 0 to -mainGridWidth 

             *  

             */


            //var sb = new Storyboard();
            //var commGridAnimation
            //    = new ThicknessAnimation
            //    {
            //        From = new Thickness(0),

            //        To = new Thickness((isRightHanded ? -1 : 1) * mainGridWidth, 0,
            //                (isRightHanded ? 1 : -1) * mainGridWidth, 0),

            //        AccelerationRatio = 0.2,
            //        FillBehavior = FillBehavior.Stop,
            //        DecelerationRatio = 0.8,
            //        Duration = DURATION
            //    };

            //var mainGridAnimation
            //    = new ThicknessAnimation

            //    {
            //        From = new Thickness(0),
            //        To = new Thickness((isRightHanded ? 1 : -1) * commGridWidth, 0,
            //                (isRightHanded ? -1 : 1) * commGridWidth, 0),

            //        FillBehavior = FillBehavior.Stop,
            //        AccelerationRatio = 0.2,
            //        DecelerationRatio = 0.8,
            //        Duration = DURATION
            //    };


            //Storyboard.SetTarget(commGridAnimation, commGrid);
            //Storyboard.SetTargetProperty(commGridAnimation,
            //    new PropertyPath(MarginProperty));
            //Storyboard.SetTarget(mainGridAnimation, mainGrid);
            //Storyboard.SetTargetProperty(mainGridAnimation,
            //    new PropertyPath(MarginProperty));

            //sb.Children.Add(commGridAnimation);
            //sb.Children.Add(mainGridAnimation);
            //sb.Completed += postAnimation;
            //sb.Begin();
        }

        private void ToolBarSwitch_OnClick(object sender, RoutedEventArgs e)
        {
            var t = ToolsBorder.Width;
        }

        #endregion

        private void ToolsBorder_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ;
        }
    }
}