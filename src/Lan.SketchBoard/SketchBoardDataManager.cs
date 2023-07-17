#nullable enable

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Lan.Shapes;
using Lan.Shapes.Custom;
using Lan.Shapes.Enums;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Styler;

#endregion

namespace Lan.SketchBoard
{
    public class SketchBoardDataManager : ISketchBoardDataManager, INotifyPropertyChanged
    {
        #region fields

        private readonly Dictionary<string, Type> _drawingTools = new Dictionary<string, Type>();


        private readonly ShapeStylerFactory _shapeStylerFactory = new ShapeStylerFactory();

        private Type? _currentGeometryType;

        /// <summary>
        ///     当前图层
        /// </summary>
        private ShapeLayer? _currentShapeLayer;

        private SketchBoard? _sketchBoardOwner;

        #endregion

        #region interface implementations

        #region Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region local methods

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region others

        /// <summary>
        ///     select one shape to draw
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
                _currentGeometryType = _drawingTools[drawingTool];
            else
                throw new Exception("the drawing tool does not exist");
        }

        #endregion

        #endregion

        #region implementations

        /// <summary>
        ///     bindable collection of shapes
        /// </summary>
        public ObservableCollection<ShapeVisualBase> Shapes { get; private set; }


        /// <summary>
        ///     this is used to hold all shapes
        /// </summary>
        public VisualCollection VisualCollection { get; private set; }

        /// <summary>
        ///     get all shapes defined in canvas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ShapeVisualBase> GetSketchBoardVisuals()
        {
            // return VisualCollection;
            return null;
        }

        /// <summary>
        ///     shape count
        /// </summary>
        public int ShapeCount => VisualCollection.Count;

        private ShapeVisualBase? _currentGeometryInEdit;

        /// <summary>
        ///  处于编辑状态的图形
        /// </summary>
        public ShapeVisualBase? CurrentGeometryInEdit
        {
            get => _currentGeometryInEdit;
            set
            {
                if (_currentGeometryInEdit != null) _currentGeometryInEdit.State = ShapeVisualState.Normal;
                SetField(ref _currentGeometryInEdit, value);
                if (_currentGeometryInEdit != null) _currentGeometryInEdit.State = ShapeVisualState.Selected;
            }
        }

        private ShapeVisualBase? _selectedGeometry;

        public ShapeVisualBase? SelectedGeometry
        {
            get => _selectedGeometry;
            set
            {
                if (_selectedGeometry != null)
                    _selectedGeometry.State =
                        _selectedGeometry.State == ShapeVisualState.Locked
                            ? ShapeVisualState.Locked
                            : ShapeVisualState.Normal;

                _selectedGeometry = value;

                if (_selectedGeometry != null) _selectedGeometry.State = ShapeVisualState.Selected;
            }
        }


        public void SetGeometryType(Type type)
        {
            _currentGeometryType = type;
        }

        /// <summary>
        ///     当前使用图层
        /// </summary>
        public ShapeLayer? CurrentShapeLayer => _currentShapeLayer;


        /// <summary>
        ///     由sketchboard 向此添加,可用于初始化时加载现有图形
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(ShapeVisualBase shape)
        {
            VisualCollection.Add(shape);
            Shapes.Add(shape);
            CurrentGeometryInEdit = shape;
        }

        /// <summary>
        ///     指定集合位置添加一个新图形
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="index"></param>
        public void AddShape(ShapeVisualBase shape, int index)
        {
            VisualCollection.Insert(index, shape);
            Shapes.Insert(index, shape);
            CurrentGeometryInEdit = shape;
        }

        public void RemoveShape(ShapeVisualBase shape)
        {
            VisualCollection.Remove(shape);
            Shapes.Remove(shape);
        }

        public void RemoveAt(int index)
        {
            VisualCollection.RemoveAt(index);
            Shapes.RemoveAt(index);
        }


        /// <summary>
        ///     not supported
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveAt(int index, int count)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     clear all shapes on canvas
        /// </summary>
        public void ClearAllShapes()
        {
            VisualCollection?.Clear();
            Shapes?.Clear();
            CurrentGeometryInEdit = null;
        }

        public ShapeVisualBase? GetShapeVisual(int index)
        {
            return VisualCollection[index] as ShapeVisualBase;
        }

        /// <summary>
        ///     add a specific geometry with specific data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <param name="parameter"></param>
        public void LoadShape<T, TP>(TP parameter) where T : ShapeVisualBase, IDataExport<TP>
            where TP : IGeometryMetaData
        {
            var shape = (T)Activator.CreateInstance(typeof(T), CurrentShapeLayer)!;
            shape.FromData(parameter);
            AddShape(shape);
        }

        /// <summary>
        ///     设置图层
        /// </summary>
        /// <param name="layer"></param>
        public void SetShapeLayer(ShapeLayer layer)
        {
            _currentShapeLayer = layer;
        }


        public ShapeVisualBase? CreateNewGeometry(Point mousePosition)
        {
            if (_currentGeometryType == null || _currentShapeLayer == null) return null;

            var shape = Activator.CreateInstance(_currentGeometryType, CurrentShapeLayer) as ShapeVisualBase;

            if (shape != null)
            {
                shape.ShapeLayer = CurrentShapeLayer;
                VisualCollection.Add(shape);
                CurrentGeometryInEdit = shape;
                Shapes.Add(shape);
            }

            if (shape is FixedCenterCircle fixedCenterCircle)
                if (_sketchBoardOwner != null)
                    fixedCenterCircle.Center =
                        new Point(_sketchBoardOwner.ActualWidth / 2, _sketchBoardOwner.ActualHeight / 2);

            return shape;
        }

        /// <summary>
        ///     set current geometry as null
        /// </summary>
        public void UnselectGeometry()
        {
            CurrentGeometryInEdit = null;
            _currentGeometryType = null;
        }

        public void InitializeVisualCollection(Visual visual)
        {
            VisualCollection = new VisualCollection(visual);
            Shapes ??= new ObservableCollection<ShapeVisualBase>();
            Shapes.Clear();

            if (visual is SketchBoard sketchBoard) _sketchBoardOwner = sketchBoard;
        }

        public void OnImageViewerPropertyChanged(double scale)
        {
            foreach (var shapeStyler in CurrentShapeLayer.Stylers)
            {
                shapeStyler.Value.SketchPen.Thickness = 0.5 / scale;
                shapeStyler.Value.DragHandleSize = 8 / scale;
                //Console.WriteLine($"dragSize{shapeStyler.Value.DragHandleSize}");
            }
        }

        #endregion
    }
}