#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lan.Shapes.Interfaces;
using Lan.SketchBoard;

#endregion

namespace Lan.ImageViewer {
    [TemplatePart(Type = typeof(Canvas), Name = "containerCanvas")]
    [TemplatePart(Type = typeof(Image), Name = "ImageViewer")]
    [TemplatePart(Type = typeof(Grid), Name = "GridContainer")]
    [TemplatePart(Type = typeof(TextBlock), Name = "TbMousePosition")]
    [TemplatePart(Type = typeof(Button), Name = "BtnFit")]
    public class ImageViewer : ImageViewerBasic {
        #region fields

        #endregion

        #region Constructors

        static ImageViewer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer),
                new FrameworkPropertyMetadata(typeof(ImageViewer)));
        }



        #endregion

        #region others

        #endregion

        #region binding properties

        #endregion


        #region dependency properties

        // Register a dependency property with the specified property name,
        // property type, owner type, and property metadata.
        // Assign DependencyPropertyKey to a nonpublic field.

        // Declare a public get accessor.


        // Register a dependency property with the specified property name,
        // property type, owner type, and property metadata.
        // Assign DependencyPropertyKey to a nonpublic field.


        // Declare a public get accessor.


        public static readonly DependencyProperty SketchBoardDataManagerProperty = DependencyProperty.Register(
            "SketchBoardDataManager", typeof(ISketchBoardDataManager), typeof(ImageViewer),
            new PropertyMetadata(default(ISketchBoardDataManager), OnSketchBoardChangeCallBack));

        private static void OnSketchBoardChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ImageViewer imageViewer && e.NewValue is ISketchBoardDataManager sketchBoardDataManager) {
                imageViewer.PropertyChanged += (s, e) => {
                    if (e.PropertyName.Equals("LocalScale")) {

                        sketchBoardDataManager.OnImageViewerPropertyChanged(imageViewer.LocalScale);
                    }
                };
            }
        }

        public ISketchBoardDataManager SketchBoardDataManager {
            get => (ISketchBoardDataManager)GetValue(SketchBoardDataManagerProperty);
            set => SetValue(SketchBoardDataManagerProperty, value);
        }

        #endregion


        #region events handlers

        #endregion
    }
}