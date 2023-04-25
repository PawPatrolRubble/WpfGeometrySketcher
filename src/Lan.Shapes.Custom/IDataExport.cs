#nullable enable
using System.Collections.Generic;
using System.Windows;

namespace Lan.Shapes.Custom
{
    /// <summary>
    /// used to export critical position data
    /// </summary>
    public interface IDataExport
    {
        IEnumerable<Point> GetDataPoints();

    }
}