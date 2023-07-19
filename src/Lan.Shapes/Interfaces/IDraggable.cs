using System;
using System.Collections.Generic;
using System.Text;
using Lan.Shapes.Handle;

namespace Lan.Shapes.Interfaces
{
    public interface IShape
    {
        /// <summary>
        /// false, it will not be rendered
        /// </summary>
        bool IsVisible { get; }
        
        /// <summary>
        /// if true, the shape can react to mouse events
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// is current shape been selected or in editing
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// draw geometry defined
        /// </summary>
        /// <param name="renderContext"></param>
        void Draw(IRenderContext renderContext);

        IGeometry Geometry { get; }

        double Height { get; }
        
        double Width { get; }

    }

    public interface IGeometry
    {

    }




    public interface IRenderContext
    {

    }

    //
    public interface IDraggable
    {
        List<DragHandle> GetDragHandles();
    }

    public interface IStrokeThicknessChangeable
    {
        void OnStrokeThicknessChanges();
    }


}
