#nullable enable
using System.Collections.Generic;
using Lan.Shapes.Interfaces;
using Lan.Shapes.Shapes;

namespace Lan.Shapes.Visitor
{
    /// <summary>
    /// Visitor that exports shape data to a serializable format.
    /// Example implementation of the Visitor pattern.
    /// </summary>
    public class ShapeExportVisitor : IShapeVisitor<IGeometryMetaData?>
    {
        private readonly List<IGeometryMetaData> _exportedData = new();

        /// <summary>
        /// Get all exported data
        /// </summary>
        public IReadOnlyList<IGeometryMetaData> ExportedData => _exportedData;

        public IGeometryMetaData? Visit(Rectangle rectangle)
        {
            var data = rectangle.GetMetaData();
            _exportedData.Add(data);
            return data;
        }

        public IGeometryMetaData? Visit(Circle circle)
        {
            var data = circle.GetMetaData();
            _exportedData.Add(data);
            return data;
        }

        public IGeometryMetaData? Visit(Ellipse ellipse)
        {
            var data = ellipse.GetMetaData();
            _exportedData.Add(data);
            return data;
        }

        public IGeometryMetaData? Visit(Line line)
        {
            var data = line.GetMetaData();
            _exportedData.Add(data);
            return data;
        }

        public IGeometryMetaData? Visit(Cross cross)
        {
            var data = cross.GetMetaData();
            _exportedData.Add(data);
            return data;
        }

        public IGeometryMetaData? VisitDefault(ShapeVisualBase shape)
        {
            // For shapes that don't implement IDataExport, return null
            return null;
        }

        /// <summary>
        /// Clear exported data
        /// </summary>
        public void Clear()
        {
            _exportedData.Clear();
        }
    }
}
