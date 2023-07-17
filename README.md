# GeometryShapesSketcher
a wpf control can be used to sketch common shapes like rectangle, ellipse on an image. it is extensible to support more custom shapes.

the sketch area can be zoomed in and out using mouse wheel. 
to pan move the sketch area you need to press CTRL + Left mouse button pressed.

本项目主要实现如何在wpf下绘制各种图形，基于drawingVisual比wpf自带图形控件性能上更具优势。


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


### Features
1. extensible to support all kinds of custom geometries 方便扩展，可支持自定义图形
2. drag handle is auto calculated to ensure unifrom using experience， 根据图形大小自动计算拖拉框
3. rgb value of pixel is displayed， 显示像素rgb值
4. scale rate is supported， 显示缩放倍率


### todo list
1. add grid rectangle， 添加带格子矩形，带交互对话框，输入行和列数

