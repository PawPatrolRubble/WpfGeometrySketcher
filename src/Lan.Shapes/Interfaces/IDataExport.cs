#nullable enable
namespace Lan.Shapes.Interfaces
{
    /// <summary>
    /// used to export critical position data
    /// </summary>
    public interface IDataExport<T> where T : IGeometryMetaData
    {
        void FromData(T data);
        T GetMetaData();
    }
}