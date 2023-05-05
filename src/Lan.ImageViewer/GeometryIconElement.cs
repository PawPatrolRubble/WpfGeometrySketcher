using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Lan.ImageViewer
{
    public  static class GeometryIconElement
    {

        public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.RegisterAttached(
            "IconGeometry", typeof(Geometry), typeof(GeometryIconElement), new PropertyMetadata(default(Geometry)));

        public static void SetIconGeometry(DependencyObject element, Geometry value)
        {
            element.SetValue(IconGeometryProperty, value);
        }

        public static Geometry GetIconGeometry(DependencyObject element)
        {
            return (Geometry)element.GetValue(IconGeometryProperty);
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.RegisterAttached(
            "IconSize", typeof(double), typeof(GeometryIconElement), new PropertyMetadata(default(double)));

        public static void SetIconSize(DependencyObject element, double value)
        {
            element.SetValue(IconSizeProperty, value);
        }

        public static double GetIconSize(DependencyObject element)
        {
            return (double)element.GetValue(IconSizeProperty);
        }
    }
}
