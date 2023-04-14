# GeometryShapesSketcher
a wpf control can be used to sketch common shapes like rectangle, ellipse on an image. it is extensible to support more custom shapes.

the sketch area can be zoomed in and out using mouse wheel. 
to pan move the sketch area you need to press CTRL + Left mouse button pressed.

本项目主要实现如何在wpf下绘制各种图形，基于drawingVisual比wpf自带图形控件性能上更具优势。

![image](https://user-images.githubusercontent.com/44959548/231935068-9847e6bf-cce8-463a-8426-921ab581b9bf.png)


## Get started

1. add the follwing to xaml
```xml
<imageViewer:ImageViewerControl
        Margin="5"
        Padding="10"
        BorderBrush="Red"
        DataContext="{Binding Camera2}" //datacontext for imageviewer control
        BorderThickness="1" />
```

2. define view models
``` c#

//define image control viewmodel
public IImageViewerViewModel Camera1 { get; set; }

//instantiation
Camera1 = new ImageViewerControlViewModel();
```


### Notice
this project is still under development, any improvements will be appreciated.


### Note

Polygon geometry is stil not fully supported. translation and scale is not supported yet.
