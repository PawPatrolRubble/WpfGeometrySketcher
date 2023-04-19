#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Styler;

namespace Lan.SketchBoard
{
    public class SketchBoardDataManager : ISketchBoardDataManager
    {


        private readonly ShapeStylerFactory _shapeStylerFactory = new ShapeStylerFactory();
        private readonly Dictionary<string, Type> _drawingTools = new Dictionary<string, Type>();

        private Type? _currentGeometryType;

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
        public ShapeVisualBase? CurrentGeometry { get; private set; }


        public void SetGeometryType(Type type)
        {
            _currentGeometryType = type;
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
            CurrentGeometry = shape;
        }

        /// <summary>
        /// 指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        public void AddShape(ShapeVisualBase shape, int index)
        {
            VisualCollection.Insert(index, shape);
            CurrentGeometry = shape;
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
        public void SetGeometryType(string drawingTool)
        {
            Debug.Assert(_currentShapeLayer != null, nameof(_currentShapeLayer) + " != null");

            //it is not true when select one new geometry type means the user will create a new shape, it is completely possible that the user
            //switches the geometry type by accident
            // LocalAddNewGeometry(drawingTool, _currentShapeLayer.Styler);

            //todo set current geometry type
            //
            if (_drawingTools.ContainsKey(drawingTool))
            {
                _currentGeometryType = _drawingTools[drawingTool];
            }
            else
            {
                throw new Exception("the drawing tool does not exist");
            }
        }

        /// <summary>
        /// 设置图层
        /// </summary>
        /// <param name="layer"></param>
        public void SetShapeLayer(ShapeLayer layer)
        {
            _currentShapeLayer = layer;
        }


        public ShapeVisualBase? CreateNewGeometry(Point mousePosition)
        {

            if (_currentGeometryType == null || _currentShapeLayer == null)
            {
                return null;
            }

            var shape = Activator.CreateInstance(_currentGeometryType) as ShapeVisualBase;

            if (shape != null)
            {
                shape.ShapeLayer = CurrentShapeLayer;
                VisualCollection.Add(shape);
                CurrentGeometry = shape;
            }

            return shape;
        }

        /// <summary>
        /// set current geometry as null
        /// </summary>
        public void UnselectGeometry()
        {
            CurrentGeometry = null;
            _currentGeometryType = null;
        }
    }
}