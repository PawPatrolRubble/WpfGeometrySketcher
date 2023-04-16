using System;
using System.Collections.Generic;

namespace Lan.Shapes
{
    /// <summary>
    /// used to manage geometry types
    /// </summary>
    public interface IGeometryTypeManager
    {
        void RegisterGeometryType(string geometryName, Type geometryType);
        void RegisterGeometryType<T>() where T : ShapeVisualBase;
        IEnumerable<string> GetRegisteredGeometryTypes();
        Type GetGeometryTypeByName(string name);
    }
}