#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Lan.Shapes.Styler;

namespace Lan.Shapes
{
    /// <summary>
    /// responsible for grouping shapes drawn and setting display style in the group
    /// all shapes must be managed by layer, 
    /// layer information is read from app setting.json
    /// a shape layer will instruct renderContext how to show the geometry contained
    /// all shapes in a shapeLayer shape common ui styles,
    /// like line weight, stroke color, when they are selected, hovered over, etc.
    /// </summary>
    public class ShapeLayer
    {
        #region fields

        private List<ShapeVisualBase> _shapeVisuals = new List<ShapeVisualBase>();

        /// <summary>
        /// all shape stylers contained in one layer
        /// </summary>
        private Dictionary<ShapeVisualState, IShapeStyler> _stylers = new Dictionary<ShapeVisualState, IShapeStyler>();


        #endregion

        
        #region properties
        
        public Dictionary<ShapeVisualState,IShapeStyler> Stylers
        {
            get => _stylers;
        }


        /// <summary>
        /// get all shapes contained the layer
        /// </summary>
        public IEnumerable<ShapeVisualBase> Shapes
        {
            get => _shapeVisuals;
        }
        
        
        public int LayerId { get; }
        public string Name { get; }
        public string Description { get; }
        
        public Brush TextForeground { get; } = Brushes.Black;
        public Brush BorderBackground { get; } = Brushes.LightBlue;

        
        #endregion


        #region constructor

        private ShapeLayer(int layerId, string name, string description)
        {
            LayerId = layerId;
            Name = name;
            Description = description;
        }

        public ShapeLayer(ShapeLayerParameter shapeLayerParameter)
        {
            LayerId = shapeLayerParameter.LayerId;
            Name = shapeLayerParameter.Name;
            Description = shapeLayerParameter.Description;

            _stylers = new Dictionary<ShapeVisualState, IShapeStyler>(shapeLayerParameter.StyleSchema.Select(x =>
                new KeyValuePair<ShapeVisualState, IShapeStyler>(x.Key, new ShapeStyler(x.Value))));

            BorderBackground = shapeLayerParameter.BorderBackground;
            TextForeground = shapeLayerParameter.TextForeground;
        }

        
        #endregion


        #region public interfaces
        
        public IEnumerable<ShapeVisualBase> RenderShapes(List<ShapeVisualBase> shapeVisuals)
        {
            foreach (var visual in shapeVisuals)
            {
              RenderWithStyler(GetStyler(visual.State),visual);
            }

            return shapeVisuals;
        }

        private void RenderWithStyler(IShapeStyler styler, ShapeVisualBase shape)
        {
            var dc = shape.RenderOpen();
            dc.DrawGeometry(GetStyler(shape.State).FillColor, styler.SketchPen, shape.RenderGeometry);
            dc.Close();
        }

        #endregion
        
        
        private Brush BrushFromHexString(string hextString)
        {
            var converter = new BrushConverter();
            return (Brush)converter.ConvertFromString(hextString);
        }
        
        

        /// <summary>
        /// get styler based on the state of shape
        /// </summary>
        /// <param name="shapeState"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"> if shapeState is not found</exception>
        public IShapeStyler GetStyler(ShapeVisualState shapeState) => _stylers[shapeState];


      
        public void AddShapeToLayer(ShapeVisualBase shape)
        {
            _shapeVisuals.Add(shape);
        }

        public ShapeLayerParameter ToShapeLayerParameter()
        {
            return new ShapeLayerParameter()
            {
                LayerId = LayerId,
                BorderBackground = BorderBackground,
                Description = Description,
                Name = Name,
                StyleSchema = new Dictionary<ShapeVisualState, ShapeStylerParameter>(_stylers.Select(x => new KeyValuePair<ShapeVisualState, ShapeStylerParameter>(x.Key,x.Value.ToStylerParameter())))
            };
            //throw new NotImplementedException();
        }
    }
}