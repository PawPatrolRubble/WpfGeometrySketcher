#nullable enable
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Styler;

namespace Lan.SketchBoard
{
    public class SketchBoardDataManager : ISketchBoardDataManager
    {
        private ShapeStylerFactory _shapeStylerFactory = new ShapeStylerFactory();
        private readonly Dictionary<string, Type> _drawingTools = new Dictionary<string, Type>();
        private ShapeLayer? _currentShapeLayer;


        /// <summary>
        /// this is used to hold all shapes
        /// </summary>
        public VisualCollection VisualCollection { get; set; } = null!;

        /// <summary>
        /// get all shapes defined in canvas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ShapeVisual> GetSketchBoardVisuals()
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
        public ShapeVisual? SelectedShape { get; private set; }

        /// <summary>
        /// relate a tool with a shape
        /// </summary>
        /// <param name="displayToolName"></param>
        /// <param name="shapeType"></param>
        public void RegisterDrawingTool(string displayToolName, Type shapeType)
        {
            _drawingTools.Add(displayToolName, shapeType);
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
        public void AddShape(ShapeVisual shape)
        {
            VisualCollection.Add(shape);
            SelectedShape = shape;
        }

        /// <summary>
        /// 指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        public void AddShape(ShapeVisual shape, int index)
        {
            VisualCollection.Insert(index, shape);
            SelectedShape = shape;
        }

        public void RemoveShape(ShapeVisual shape)
        {
            VisualCollection.Remove(shape);
        }

        public void RemoveAt(int index)
        {
            VisualCollection.RemoveAt(index);
        }

        public void RemoveAt(int index, int count)
        {
            throw new NotImplementedException();
        }

        public void ClearAllShapes()
        {
            VisualCollection.Clear();
        }

        public ShapeVisual? GetShapeVisual(int index)
        {
            return VisualCollection[index] as ShapeVisual;
        }

        /// <summary>
        /// select one shape to draw
        /// </summary>
        /// <param name="drawingTool"></param>
        public void SelectDrawingTool(string drawingTool)
        {
            //todo throw exception if visualcollection is null

            if (VisualCollection==null)
            {
                throw new NullReferenceException("visual collection must be init first");
            }
            
            if (_drawingTools.ContainsKey(drawingTool))
            {
               var shape =(ShapeVisual)Activator.CreateInstance(_drawingTools[drawingTool])!;
               shape.ShapeStyler = _shapeStylerFactory.ShapeUnselectedVisualState();

               SelectedShape = shape;
               VisualCollection.Add(shape);
            }
        }
    }
}