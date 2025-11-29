#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;

namespace Lan.Shapes.Interfaces
{
    /// <summary>
    /// Provides functionality for managing geometry data for SketchBoard.
    /// </summary>
    public interface IShapeCollection
    {
        /// <summary>
        /// bindable collection of shapes
        /// </summary>
        ObservableCollection<ShapeVisualBase> Shapes { get; }

        /// <summary>
        /// shape count
        /// </summary>
        int ShapeCount { get; }

        /// <summary>
        /// add one shape
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

        void RemoveShapes(Expression<Func<ShapeVisualBase, bool>> predict);

        void RemoveAt(int index);

        void RemoveAt(int index, int count);

        /// <summary>
        /// remove all shapes on canvas
        /// </summary>
        void ClearAllShapes();

        ShapeVisualBase? GetShapeVisual(int index);
    }

    /// <summary>
    /// Provides functionality for selecting shapes.
    /// </summary>
    public interface IShapeSelection
    {
        /// <summary>
        /// 当前选中的画图类型
        /// </summary>
        ShapeVisualBase? CurrentGeometryInEdit { get; set; }

        ShapeVisualBase? SelectedGeometry { get; set; }

        /// <summary>
        /// Collection of currently selected shapes (for multi-selection)
        /// </summary>
        IReadOnlyList<ShapeVisualBase> SelectedShapes { get; }

        /// <summary>
        /// set current geometry as null
        /// </summary>
        void UnselectGeometry();

        /// <summary>
        /// Clear all selected shapes
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Select multiple shapes at once
        /// </summary>
        void SelectShapes(IEnumerable<ShapeVisualBase> shapes, bool addToExisting = false);

        /// <summary>
        /// Delete all currently selected shapes
        /// </summary>
        void DeleteSelectedShapes();

        /// <summary>
        /// Move all selected shapes by the specified offset
        /// </summary>
        void MoveSelectedShapes(Vector offset);

        /// <summary>
        /// triggered when new shape is sketched, right after the mouse up
        /// </summary>
        Action<ShapeVisualBase>? NewShapeSketched { get; set; }

        /// <summary>
        /// Event raised when the selection changes
        /// </summary>
        event EventHandler<IReadOnlyList<ShapeVisualBase>>? SelectionChanged;

        event EventHandler<ShapeVisualBase> ShapeCreated;

        event EventHandler<ShapeVisualBase> ShapeRemoved;
    }

    /// <summary>
    /// Provides functionality for creating shapes.
    /// </summary>
    public interface IShapeFactory
    {
        /// <summary>
        /// Set the geometry type for new shape creation
        /// </summary>
        void SetGeometryType(Type type);

        /// <summary>
        /// Load a shape from serialized data
        /// </summary>
        ShapeVisualBase LoadShape<T, TP>(TP parameter)
            where T : ShapeVisualBase, IDataExport<TP>
            where TP : IGeometryMetaData;

        /// <summary>
        /// Create a shape from data without adding to collection
        /// </summary>
        ShapeVisualBase CreateShape<T, TP>(TP parameter)
            where T : ShapeVisualBase, IDataExport<TP>
            where TP : IGeometryMetaData;

        /// <summary>
        /// Create new geometry from mouse down position
        /// </summary>
        /// <returns>New shape, or null if no geometry type is selected</returns>
        ShapeVisualBase? CreateNewGeometry(Point mousePosition);
    }

    /// <summary>
    /// Provides functionality for managing layers.
    /// </summary>
    public interface ILayerManager
    {
        /// <summary>
        /// 设置图层
        /// </summary>
        /// <param name="layer"></param>
        void SetShapeLayer(ShapeLayer layer);

        /// <summary>
        /// 当前使用图层
        /// </summary>
        ShapeLayer? CurrentShapeLayer { get; }
    }

    /// <summary>
    /// Provides functionality for managing geometry data for SketchBoard.
    /// This interface composes IShapeCollection, IShapeSelection, IShapeFactory, and ILayerManager
    /// following the Interface Segregation Principle.
    /// </summary>
    public interface ISketchBoardDataManager : IShapeCollection, IShapeSelection, IShapeFactory, ILayerManager
    {
        /// <summary>
        /// Reference to the sketch board control
        /// </summary>
        ISketchBoard SketchBoard { get; }

        /// <summary>
        /// Visual collection for rendering
        /// </summary>
        VisualCollection VisualCollection { get; }

        /// <summary>
        /// Get all shapes defined in canvas
        /// </summary>
        IEnumerable<ShapeVisualBase> GetSketchBoardVisuals();

        /// <summary>
        /// Initialize the visual collection with a parent visual
        /// </summary>
        void InitializeVisualCollection(Visual visual);

        /// <summary>
        /// Handle image viewer property changes (e.g., scale)
        /// </summary>
        void OnImageViewerPropertyChanged(double scale);

        /// <summary>
        /// Event raised when the sketch board manager is initialized
        /// </summary>
        event EventHandler<ISketchBoardDataManager>? SketchBoardManagerInitialized;
    }
}