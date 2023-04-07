# GeometryShapesSketcher
a wpf control can be used to sketch common shapes like rectangle, ellipse. it is extensible to support more shapes.
本项目主要实现如何在wpf下绘制各种图形，基于drawingVisual比wpf自带图形控件性能上更具优势。

![image](https://user-images.githubusercontent.com/44959548/229743712-e167e41a-2360-47f9-bea4-3824ff6336c2.png)


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