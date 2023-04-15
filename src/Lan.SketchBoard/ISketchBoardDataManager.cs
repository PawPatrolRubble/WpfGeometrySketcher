#nullable enable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Styler;

namespace Lan.SketchBoard
{
    /// <summary>
    /// provide the functionality for managing geometry data for <see cref="SketchBoard"/>,
    /// which is only responsible for displaying
    /// </summary>
    public interface ISketchBoardDataManager
    {
        /// <summary>
        /// this is used to hold all shapes
        /// </summary>
        VisualCollection VisualCollection { get; set; }
        
        /// <summary>
        /// get all shapes defined in canvas
        /// </summary>
        /// <returns></returns>
        IEnumerable<ShapeVisualBase> GetSketchBoardVisuals();

        /// <summary>
        /// shape count
        /// </summary>
        int ShapeCount { get; }

        /// <summary>
        /// 当前选中的画图类型
        /// </summary>
        ShapeVisualBase? SelectedShape { get; }

        /// <summary>
        /// relate a tool with a shape
        /// </summary>
        /// <param name="displayToolName"></param>
        /// <param name="shapeType"></param>
        void RegisterGeometryType(string displayToolName, Type shapeType);

        /// <summary>
        /// return a list of registered drawing tools
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetRegisteredGeometryTypes();


        /// <summary>
        /// 设置图层
        /// </summary>
        /// <param name="layer"></param>
        void SetShapeLayer(ShapeLayer layer);


        /// <summary>
        /// 当前使用图层
        /// </summary>
        ShapeLayer? CurrentShapeLayer { get; }

        /// <summary>
        /// manipulator currently used
        /// </summary>
        // IShapeManipulator? CurrentShapeManipulator { get; }

        /// <summary>
        /// 由sketchboard 向此添加,可用于初始化时加载现有图形,
        /// </summary>
        /// <param name="shape"></param>
        void AddShape(ShapeVisualBase shape);

        /// <summary>
        /// 指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        void AddShape(ShapeVisualBase shape, int index);

        /// <summary>
        /// remove one shape
        /// </summary>
        /// <param name="shape"></param>
        void RemoveShape(ShapeVisualBase shape);

        void RemoveAt(int index);

        void RemoveAt(int index, int count);

        /// <summary>
        /// remove all shapes on canvas
        /// </summary>
        void ClearAllShapes();

        ShapeVisualBase? GetShapeVisual(int index);
        
        /// <summary>
        /// select one shape to draw
        /// </summary>
        /// <param name="drawingTool"></param>
        void SelectGeometryType(string drawingTool);

        /// <summary>
        /// provides styler for the new shaped created
        /// </summary>
        /// <param name="drawingTool"></param>
        /// <param name="styler"></param>
        void SelectGeometryType(string drawingTool, IShapeStyler styler);



        void SetSelectedShape(ShapeVisualBase shapeSelectedFromCanvas);

        /// <summary>
        /// handle the mouse up event from canvas
        /// </summary>
        /// <param name="mousePosition"></param>
        void MouseUpHandler(Point mousePosition);

    }
    
}