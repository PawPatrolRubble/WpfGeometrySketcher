#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Styler;

namespace Lan.SketchBoard
{
    public class SketchBoardDataManager : ISketchBoardDataManager
    {
        private readonly ShapeStylerFactory _shapeStylerFactory = new ShapeStylerFactory();
        private readonly Dictionary<string, Type> _drawingTools = new Dictionary<string, Type>();

        /// <summary>
        /// 当前图层
        /// </summary>
        private ShapeLayer? _currentShapeLayer;


        /// <summary>
        /// this is used to hold all shapes
        /// </summary>
        public VisualCollection VisualCollection { get; set; } = null!;

        /// <summary>
        /// get all shapes defined in canvas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ShapeVisualBase> GetSketchBoardVisuals()
        {
            // return VisualCollection;
            return null;
        }

        /// <summary>
        /// shape count
        /// </summary>
        public int ShapeCount => VisualCollection.Count;

        /// <summary>
        /// 当前选中的画图类型
        /// </summary>
        public ShapeVisualBase? SelectedShape { get; private set; }

        /// <summary>
        /// relate a tool with a shape
        /// </summary>
        /// <param name="displayToolName"></param>
        /// <param name="shapeType"></param>
        public void RegisterGeometryType(string displayToolName, Type shapeType)
        {
            _drawingTools.Add(displayToolName, shapeType);
        }

        /// <summary>
        /// return a list of registered drawing tools
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetRegisteredGeometryTypes()
        {
            return _drawingTools.Keys;
        }

        /// <summary>
        /// 设置图层
        /// </summary>
        /// <param name="layer"></param>
        public void SetShapeLayer(ShapeLayer layer)
        {
            _currentShapeLayer = layer;
        }

        /// <summary>
        /// 当前使用图层
        /// </summary>
        public ShapeLayer? CurrentShapeLayer => _currentShapeLayer;


        /// <summary>
        /// 由sketchboard 向此添加,可用于初始化时加载现有图形
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(ShapeVisualBase shape)
        {
            VisualCollection.Add(shape);
            SelectedShape = shape;
        }

        /// <summary>
        /// 指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        public void AddShape(ShapeVisualBase shape, int index)
        {
            VisualCollection.Insert(index, shape);
            SelectedShape = shape;
        }

        public void RemoveShape(ShapeVisualBase shape)
        {
            VisualCollection.Remove(shape);
        }

        public void RemoveAt(int index)
        {
            VisualCollection.RemoveAt(index);
        }


        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveAt(int index, int count)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// clear all shapes on canvas
        /// </summary>
        public void ClearAllShapes()
        {
            VisualCollection.Clear();
        }

        public ShapeVisualBase? GetShapeVisual(int index)
        {
            return VisualCollection[index] as ShapeVisualBase;
        }

        /// <summary>
        /// select one shape to draw
        /// </summary>
        /// <param name="drawingTool"></param>
        public void SelectDrawingTool(string drawingTool)
        {
            Debug.Assert(_currentShapeLayer != null, nameof(_currentShapeLayer) + " != null");
            LocalAddNewGeometry(drawingTool, _currentShapeLayer.Styler);
        }

        private void LocalAddNewGeometry(string drawingTool, IShapeStyler shapeStyler)
        {
            if (string.IsNullOrEmpty(drawingTool))
            {
                throw new ArgumentNullException(nameof(drawingTool));
            }

            if (shapeStyler == null)
            {
                throw new ArgumentNullException(nameof(shapeStyler));
            }


            if (VisualCollection == null)
            {
                throw new NullReferenceException("visual collection must be init first");
            }



            if (_drawingTools.ContainsKey(drawingTool))
            {
                var shape = (ShapeVisualBase)Activator.CreateInstance(_drawingTools[drawingTool])!;
                shape.ShapeStyler = shapeStyler;

                SelectedShape = shape;
                VisualCollection.Add(shape);
            }
        }


        /// <summary>
        /// create a shape with custom style
        /// </summary>
        /// <param name="drawingTool"></param>
        /// <param name="styler"></param>
        public void SelectDrawingTool(string drawingTool, IShapeStyler styler)
        {
            LocalAddNewGeometry(drawingTool, styler);
        }
    }
}