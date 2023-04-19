using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lan.Shapes;
using Lan.Shapes.Shapes;

namespace Lan.ImageViewer
{
    public class GeometryTypeManager : IGeometryTypeManager
    {

        #region fields

        private Dictionary<string, Type> _registeredShapeTypes = new Dictionary<string, Type>();
        #endregion

        public void RegisterGeometryType(string geometryName, Type geometryType)
        {
            _registeredShapeTypes.Add(geometryName,geometryType);
        }

        public void RegisterGeometryType<T>() where T : ShapeVisualBase
        {
            RegisterGeometryType(typeof(T).Name, typeof(T));
        }

        public IEnumerable<string> GetRegisteredGeometryTypes()
        {
            //_registeredShapeTypes.Add(nameof(Rectangle), typeof(Rectangle));
            //_registeredShapeTypes.Add(nameof(Ellipse), typeof(Ellipse));

            return _registeredShapeTypes.Keys;
        }

        public Type GetGeometryTypeByName(string name)
        {
            return _registeredShapeTypes[name];
        }

        /// <summary>
        /// read all <see cref="ShapeVisualBase"/> derived classes from assemblies when app startup,
        /// or from a directory where dlls of <see cref="ShapeVisualBase"/>derived classes are defined
        /// </summary>
        public void ReadGeometryTypesFromAssembly()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var derivedTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ShapeVisualBase)));
                foreach (var derivedType in derivedTypes)
                {
                    RegisterGeometryType(derivedType.Name, derivedType);
                }
            }

        }
    }
}
