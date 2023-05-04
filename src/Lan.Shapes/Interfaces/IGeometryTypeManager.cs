using System;
using System.Collections.Generic;

namespace Lan.Shapes.Interfaces
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

        /// <summary>
        /// read all <see cref="ShapeVisualBase"/> derived classes from assemblies when app startup,
        /// or from a directory where dlls of <see cref="ShapeVisualBase"/>derived classes are defined
        /// </summary>
        void ReadGeometryTypesFromAssembly();
    }
}