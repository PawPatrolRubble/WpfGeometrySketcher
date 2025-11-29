#nullable enable

#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
        #region interface implementation

        #region Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region fields

        private readonly Dictionary<string, Type> _drawingTools = new Dictionary<string, Type>();

        private readonly ShapeStylerFactory _shapeStylerFactory = new ShapeStylerFactory();

        private Type? _currentGeometryType;

        /// <summary>
        ///     当前图层
        /// </summary>
        private ShapeLayer? _currentShapeLayer;

        private SketchBoard? _sketchBoard;

        #endregion

        #region local methods

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

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
            {
                _currentGeometryType = _drawingTools[drawingTool];
            }
            else
            {
                throw new Exception("the drawing tool does not exist");
            }
        }

        #endregion

        #endregion

        #region implementations

        /// <summary>
        ///     bindable collection of shapes
        /// </summary>
        //public ObservableCollection<ShapeVisualBase> Shapes { get; private set; }
        private ObservableCollection<ShapeVisualBase> _shapes;

        public ISketchBoard SketchBoard { get => _sketchBoard; }

        public ObservableCollection<ShapeVisualBase> Shapes
        {
            get { return _shapes; }
            set { SetField(ref _shapes, value); }
        }


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
        public int ShapeCount
        {
            get { return VisualCollection.Count; }
        }

        private ShapeVisualBase? _currentGeometryInEdit;

        /// <summary>
        ///     处于编辑状态的图形
        /// </summary>
        public ShapeVisualBase? CurrentGeometryInEdit
        {
            get { return _currentGeometryInEdit; }
            set
            {
                if (_currentGeometryInEdit != null)
                {
                    _currentGeometryInEdit.State = ShapeVisualState.Normal;
                }

                SetField(ref _currentGeometryInEdit, value);
                if (_currentGeometryInEdit != null)
                {
                    _currentGeometryInEdit.State = ShapeVisualState.Selected;
                }
            }
        }

        private ShapeVisualBase? _selectedGeometry;
        private readonly List<ShapeVisualBase> _selectedShapes = new();

        public ShapeVisualBase? SelectedGeometry
        {
            get { return _selectedGeometry; }
            set
            {
                if (_selectedGeometry != null)
                {
                    // Don't reset state if the shape is part of multi-selection
                    if (!_selectedShapes.Contains(_selectedGeometry))
                    {
                        _selectedGeometry.State =
                            _selectedGeometry.State == ShapeVisualState.Locked
                                ? ShapeVisualState.Locked
                                : ShapeVisualState.Normal;
                    }
                }

                _selectedGeometry = value;

                if (_selectedGeometry != null)
                {
                    _selectedGeometry.State = ShapeVisualState.Selected;
                }
            }
        }

        /// <summary>
        /// Collection of currently selected shapes (for multi-selection)
        /// </summary>
        public IReadOnlyList<ShapeVisualBase> SelectedShapes => _selectedShapes;

        /// <summary>
        /// Event raised when the selection changes
        /// </summary>
        public event EventHandler<IReadOnlyList<ShapeVisualBase>>? SelectionChanged;

        /// <summary>
        /// Clear all selected shapes
        /// </summary>
        public void ClearSelection()
        {
            foreach (var shape in _selectedShapes)
            {
                if (shape.State != ShapeVisualState.Locked)
                {
                    shape.State = ShapeVisualState.Normal;
                }
            }
            _selectedShapes.Clear();
            SelectedGeometry = null;
            SelectionChanged?.Invoke(this, _selectedShapes);
        }

        /// <summary>
        /// Select multiple shapes at once
        /// </summary>
        public void SelectShapes(IEnumerable<ShapeVisualBase> shapes, bool addToExisting = false)
        {
            var shapesList = shapes.ToList();
            Debug.WriteLine($"[SelectShapes] Called with {shapesList.Count} shapes, addToExisting={addToExisting}");
            
            if (!addToExisting)
            {
                // Clear existing selection without triggering event yet
                foreach (var shape in _selectedShapes)
                {
                    if (shape.State != ShapeVisualState.Locked)
                    {
                        shape.State = ShapeVisualState.Normal;
                    }
                }
                _selectedShapes.Clear();
            }

            foreach (var shape in shapesList)
            {
                if (!_selectedShapes.Contains(shape) && !shape.IsLocked)
                {
                    _selectedShapes.Add(shape);
                    shape.State = ShapeVisualState.Selected;
                    Debug.WriteLine($"[SelectShapes] Added shape: {shape.GetType().Name}");
                }
            }

            Debug.WriteLine($"[SelectShapes] Total selected: {_selectedShapes.Count}");
            
            // Update single selection to first selected shape
            SelectedGeometry = _selectedShapes.FirstOrDefault();
            
            SelectionChanged?.Invoke(this, _selectedShapes);
        }

        /// <summary>
        /// Delete all currently selected shapes
        /// </summary>
        public void DeleteSelectedShapes()
        {
            Debug.WriteLine($"[DeleteSelectedShapes] Called. SelectedShapes.Count = {_selectedShapes.Count}");
            
            if (_selectedShapes.Count == 0)
            {
                Debug.WriteLine("[DeleteSelectedShapes] No shapes to delete, returning.");
                return;
            }

            // Create a copy to avoid modifying collection while iterating
            var shapesToDelete = _selectedShapes.ToList();
            Debug.WriteLine($"[DeleteSelectedShapes] Deleting {shapesToDelete.Count} shapes.");
            
            foreach (var shape in shapesToDelete)
            {
                Debug.WriteLine($"[DeleteSelectedShapes] Removing shape: {shape.GetType().Name}");
                RemoveShape(shape);
            }

            _selectedShapes.Clear();
            SelectedGeometry = null;
            SelectionChanged?.Invoke(this, _selectedShapes);
            Debug.WriteLine("[DeleteSelectedShapes] Complete.");
        }

        /// <summary>
        /// Move all selected shapes by the specified offset
        /// </summary>
        public void MoveSelectedShapes(Vector offset)
        {
            if (_selectedShapes.Count == 0) return;

            foreach (var shape in _selectedShapes)
            {
                shape.TranslateBy(offset);
            }
        }

        public void SetGeometryType(Type type)
        {
            _currentGeometryType = type;
        }

        /// <summary>
        ///     当前使用图层
        /// </summary>
        public ShapeLayer? CurrentShapeLayer
        {
            get { return _currentShapeLayer; }
        }


        /// <summary>
        ///     由sketchboard 向此添加,可用于初始化时加载现有图形
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(ShapeVisualBase shape)
        {
            VisualCollection.Add(shape);
            Shapes.Add(shape);
            //CurrentGeometryInEdit = shape;
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
            ShapeRemoved?.Invoke(this, shape);
        }

        public void RemoveShapes(Expression<Func<ShapeVisualBase, bool>> predict)
        {
            var shapesToRemove = Shapes.Where(predict.Compile()).ToList();
            foreach (var shape in shapesToRemove)
            {
                RemoveShape(shape);
            }
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
        public ShapeVisualBase LoadShape<T, TP>(TP parameter) where T : ShapeVisualBase, IDataExport<TP>
            where TP : IGeometryMetaData
        {
            var shape = (T)Activator.CreateInstance(typeof(T), CurrentShapeLayer)!;
            shape.FromData(parameter);
            AddShape(shape);
            shape.UpdateVisual();
            return shape;
        }

        public ShapeVisualBase CreateShape<T, TP>(TP parameter) where T : ShapeVisualBase, IDataExport<TP>
            where TP : IGeometryMetaData
        {
            var shape = (T)Activator.CreateInstance(typeof(T), CurrentShapeLayer)!;
            shape.FromData(parameter);
            return shape;
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
            if (_currentGeometryType == null || _currentShapeLayer == null)
            {
                return null;
            }

            var shape = Activator.CreateInstance(_currentGeometryType, CurrentShapeLayer) as ShapeVisualBase;

            if (shape != null)
            {
                shape.ShapeLayer = CurrentShapeLayer;
                VisualCollection.Add(shape);
                CurrentGeometryInEdit = shape;
                Shapes.Add(shape);
            }

            if (shape is FixedCenterCircle fixedCenterCircle)
            {
                if (_sketchBoard != null)
                {
                    fixedCenterCircle.Center =
                        new Point(_sketchBoard.ActualWidth / 2, _sketchBoard.ActualHeight / 2);
                }
            }

            // trigger event
            if (shape != null)
            {
                ShapeCreated?.Invoke(this, shape);
            }

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

            if (visual is SketchBoard sketchBoard)
            {
                _sketchBoard = sketchBoard;
            }

            SketchBoardManagerInitialized?.Invoke(this, this);
        }

        public void OnImageViewerPropertyChanged(double scale)
        {
            if (CurrentShapeLayer == null) return;

            // Store the current scale for future reference
            Lan.Shapes.Scaling.ViewportScalingService.SetScale(scale);

            // Update all stylers with new scale-adjusted values
            foreach (var shapeStyler in CurrentShapeLayer.Stylers)
            {
                shapeStyler.Value.SketchPen.Thickness = Lan.Shapes.Scaling.ViewportScalingService.CalculateStrokeThickness(scale);
                shapeStyler.Value.DragHandleSize = Lan.Shapes.Scaling.ViewportScalingService.CalculateDragHandleSize(scale);
            }

            // Re-render all shapes to reflect the new stroke thickness
            if (Shapes != null)
            {
                foreach (var shape in Shapes)
                {
                    shape.UpdateVisual();
                }
            }
        }

        /// <summary>
        ///     the skethboard is initialized and can load shape from now on
        /// </summary>
        public event EventHandler<ISketchBoardDataManager>? SketchBoardManagerInitialized;

        public event EventHandler<ShapeVisualBase>? ShapeCreated;
        public event EventHandler<ShapeVisualBase>? ShapeRemoved;

        /// <summary>
        ///     triggered when new shape is sketched, right after the mouse up
        /// </summary>
        public Action<ShapeVisualBase>? NewShapeSketched { get; set; }

        #endregion
    }
}