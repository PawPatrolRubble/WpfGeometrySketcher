#nullable enable
using System;
using System.Collections.Generic;
using Lan.Shapes;

namespace Lan.SketchBoard
{
    public interface ISketchBoardDataManager
    {
        
        /// <summary>
        /// get all shapes defined in canvas
        /// </summary>
        /// <returns></returns>
        IEnumerable<ShapeVisual> GetSketchBoardVisuals();

        /// <summary>
        /// shape count
        /// </summary>
        int ShapeCount { get; }

        /// <summary>
        /// 当前选中的画图类型
        /// </summary>
        string? SelectedDrawingShape { get; }

        /// <summary>
        /// relate a tool with a shape
        /// </summary>
        /// <param name="displayToolName"></param>
        /// <param name="shapeType"></param>
        void RegisterDrawingTool(string displayToolName, Type shapeType);

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
        IShapeManipulator? CurrentShapeManipulator { get; }

        /// <summary>
        /// 由sketchboard 向此添加,可用于初始化时加载现有图形
        /// </summary>
        /// <param name="shape"></param>
        void AddShape(ShapeVisual shape);

        /// <summary>
        /// 指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        void AddShape(ShapeVisual shape, int index);

        void RemoveShape(ShapeVisual shape);
        void RemoveAt(int index);
        void RemoveAt(int index, int count);

        void ClearAllShapes();
        ShapeVisual GetShapeVisual(int index);
    }
    
}