# GeometryShapesSketcher
a wpf control can be used to sketch common shapes like rectangle, ellipse on an image. it is extensible to support more custom shapes.

the sketch area can be zoomed in and out using mouse wheel. 
to pan move the sketch area you need to press CTRL + Left mouse button pressed.

本项目主要实现如何在wpf下绘制各种图形，基于drawingVisual比wpf自带图形控件性能上更具优势。

![image](https://github.com/PawPatrolRubble/WpfGeometrySketcher/assets/44959548/a2c5f68b-cd2b-4539-8e96-45ecdd3da0c6)



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
2. you can zoom in and out in pixel scale, pan move the image using mouse 支持图片像素级放大缩小，支持鼠标平移图片
3. drag handle is auto calculated to ensure unifrom using experience， 根据图形大小自动计算拖拉框
4. rgb value of pixel is displayed， 显示像素rgb值
5. scale rate is supported， 显示缩放倍率
6. it can display raw image data from industrial camera, like Hikvision, Daheng etc. 支持工业相机图像信息实时显示

