<div align="center">

# 🎨 WPF Geometry Sketcher

**A high-performance WPF control for sketching geometric shapes on images**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=flat-square&logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

[English](#features) | [中文](#功能特点)

</div>

---

## ✨ Overview

WPF Geometry Sketcher is a powerful, extensible library for drawing and manipulating geometric shapes on images in WPF applications. Built on `DrawingVisual` for superior rendering performance compared to standard WPF shape controls.

Perfect for:
- 🏭 **Industrial Vision** - ROI selection for machine vision applications
- 🖼️ **Image Annotation** - Mark up images with shapes and labels
- 📐 **CAD-like Applications** - Technical drawing and measurement tools
- 🎮 **Interactive Graphics** - Any application requiring shape manipulation

![Demo](https://github.com/PawPatrolRubble/WpfGeometrySketcher/assets/44959548/a2c5f68b-cd2b-4539-8e96-45ecdd3da0c6)

---

## 🚀 Features

| Feature | Description |
|---------|-------------|
| 🔷 **Multiple Shape Types** | Rectangle, Circle, Ellipse, Line, Cross, and custom shapes |
| 🔍 **Pixel-Level Zoom** | Smooth zoom in/out with mouse wheel |
| 🖱️ **Pan & Navigate** | CTRL + Left Mouse to pan the canvas |
| 📏 **AutoCAD-like Stroke** | Constant visual line thickness at any zoom level |
| 🎯 **Smart Drag Handles** | Auto-calculated handles for intuitive resizing |
| 🎨 **Customizable Styles** | Configure colors, stroke styles, and fill patterns |
| 📷 **Industrial Camera Support** | Display raw image data from Hikvision, Daheng, etc. |
| 🔢 **Pixel Info Display** | Real-time RGB values and coordinates |
| ⚡ **High Performance** | Built on `DrawingVisual` for optimal rendering |
| 🧩 **Extensible Architecture** | Easy to add custom shape types |

---

## 📦 Installation

### NuGet Package
```bash
dotnet add package Lan.ImageViewer.Prism
```

### Or clone and build
```bash
git clone https://github.com/PawPatrolRubble/WpfGeometrySketcher.git
cd WpfGeometrySketcher
dotnet build
```

---

## 🏁 Quick Start

### 1. Add XAML namespace
```xml
xmlns:imageViewer="clr-namespace:Lan.ImageViewer;assembly=Lan.ImageViewer"
```

### 2. Add the control
```xml
<imageViewer:ImageViewerControl
    Margin="5"
    Padding="10"
    BorderBrush="#3498db"
    BorderThickness="1"
    DataContext="{Binding ImageViewerVM}" />
```

### 3. Set up ViewModel
```csharp
public class MainViewModel
{
    public IImageViewerViewModel ImageViewerVM { get; }

    public MainViewModel()
    {
        ImageViewerVM = new ImageViewerControlViewModel();
        
        // Load an image
        ImageViewerVM.LoadImage("path/to/image.png");
        
        // Set shape type to draw
        ImageViewerVM.SetGeometryType(typeof(Rectangle));
    }
}
```

### 4. Mouse Controls

| Action | Control |
|--------|---------|
| **Draw Shape** | Left Mouse Drag |
| **Select Shape** | Left Click on shape |
| **Move Shape** | Drag selected shape |
| **Resize Shape** | Drag corner handles |
| **Delete Shape** | Right Click on shape |
| **Zoom** | Mouse Wheel |
| **Pan** | CTRL + Left Mouse Drag |

---

## 🎯 Supported Shapes

```
┌─────────────┬─────────────┬─────────────┐
│  Rectangle  │   Circle    │   Ellipse   │
├─────────────┼─────────────┼─────────────┤
│    Line     │    Cross    │   Custom    │
└─────────────┴─────────────┴─────────────┘
```

### Creating Custom Shapes

Extend `ShapeVisualBase` to create your own shapes:

```csharp
public class MyCustomShape : ShapeVisualBase
{
    protected override void CreateHandles()
    {
        // Define drag handles using factory methods
        Handles.AddRange(CreateCornerHandles());
    }

    protected override void HandleResizing(Point point)
    {
        // Handle resize logic
    }

    protected override void HandleTranslate(Point newPoint)
    {
        // Handle move logic
    }

    public override void UpdateVisual()
    {
        // Render your shape
        var dc = RenderOpen();
        // Draw using DrawingContext...
        dc.Close();
    }
}
```

---

## 🏗️ Architecture

```
WpfGeometrySketcher/
├── Lan.Shapes/              # Core shape library
│   ├── Shapes/              # Built-in shape implementations
│   ├── Handle/              # Drag handle system
│   ├── Interfaces/          # Contracts and abstractions
│   ├── Commands/            # Undo/Redo infrastructure
│   ├── Strategies/          # Drawing behavior strategies
│   └── Visitor/             # Shape operation visitors
├── Lan.Shapes.Custom/       # Extended custom shapes
├── Lan.SketchBoard/         # Canvas and interaction logic
├── Lan.ImageViewer/         # Image display control
└── Lan.ImageViewer.Prism/   # Prism MVVM integration
```

### Design Patterns Used
- **Template Method** - Customizable rendering pipeline
- **Strategy** - Swappable drawing behaviors
- **Command** - Undo/Redo support
- **Visitor** - Extensible shape operations
- **Factory Method** - Handle creation

---

## 🔧 Configuration

### Shape Layer Configuration (JSON)
```json
{
  "layers": [
    {
      "name": "Default",
      "stylers": {
        "Normal": {
          "strokeColor": "#FF0000",
          "fillColor": "#33FF0000",
          "strokeThickness": 2,
          "dragHandleSize": 8
        },
        "Selected": {
          "strokeColor": "#00FF00",
          "fillColor": "#3300FF00"
        }
      }
    }
  ]
}
```

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ⭐ Star History

If you find this project useful, please consider giving it a star! ⭐

---

<div align="center">

**[Report Bug](https://github.com/PawPatrolRubble/WpfGeometrySketcher/issues) · [Request Feature](https://github.com/PawPatrolRubble/WpfGeometrySketcher/issues)**

Made with ❤️ for the WPF community

</div>

---

## 功能特点

| 功能 | 描述 |
|------|------|
| 🔷 **多种图形类型** | 矩形、圆形、椭圆、直线、十字线及自定义图形 |
| 🔍 **像素级缩放** | 鼠标滚轮平滑缩放 |
| 🖱️ **平移导航** | CTRL + 鼠标左键拖动画布 |
| 📏 **类AutoCAD线宽** | 任意缩放级别下保持视觉线宽一致 |
| 🎯 **智能拖拽手柄** | 自动计算手柄位置，直观调整大小 |
| 🎨 **可定制样式** | 配置颜色、线型和填充样式 |
| 📷 **工业相机支持** | 支持海康、大恒等工业相机原始图像显示 |
| 🔢 **像素信息显示** | 实时显示RGB值和坐标 |
| ⚡ **高性能渲染** | 基于DrawingVisual实现最优渲染性能 |
| 🧩 **可扩展架构** | 轻松添加自定义图形类型 |

