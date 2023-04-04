using System.Windows;

namespace Lan.Shapes
{
    public interface IShapeManipulator
    {
      
    }
    /// <summary>
    /// responsible for displaying and update shape data, update geometry 
    /// </summary>
    public interface IShapeManipulator<T> : IShapeManipulator where T : ShapeVisualBase, new()
    {
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="endPoint"></param>
        void Translate(T shape, Point endPoint);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="point"></param>
        /// <param name="mouseMoveDirection"></param>
        void Rotate(T shape, Point point, bool mouseMoveDirection);
        
        
        
    }
}
