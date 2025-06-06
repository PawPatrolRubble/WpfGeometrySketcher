using System;

namespace Lan.Shapes.Scaling
{
    /// <summary>
    /// Provides centralized services for adjusting visual elements based on viewport scaling.
    /// This ensures consistent appearance of shapes regardless of zoom level.
    /// </summary>
    public static class ViewportScalingService
    {
        // Default base values for visual elements
        public static double BaseStrokeThickness { get; set; } = 1.0;
        public static double BaseDragHandleSize { get; set; } = 8.0;
        
        /// <summary>
        /// Calculates the appropriate stroke thickness based on the current scale factor.
        /// </summary>
        /// <param name="scale">The current viewport scale factor</param>
        /// <returns>Adjusted stroke thickness that maintains visual consistency at any zoom level</returns>
        public static double CalculateStrokeThickness(double scale)
        {
            // Ensure scale is valid to prevent division by zero
            if (scale <= 0)
                scale = 1.0;
                
            return BaseStrokeThickness / scale;
        }
        
        /// <summary>
        /// Calculates the appropriate drag handle size based on the current scale factor.
        /// </summary>
        /// <param name="scale">The current viewport scale factor</param>
        /// <returns>Adjusted drag handle size that maintains visual consistency at any zoom level</returns>
        public static double CalculateDragHandleSize(double scale)
        {
            // Ensure scale is valid to prevent division by zero
            if (scale <= 0)
                scale = 1.0;
                
            return BaseDragHandleSize / scale;
        }
        
        /// <summary>
        /// Calculates stroke thickness based on viewport dimensions.
        /// This is an alternative method used when the direct scale factor is not available.
        /// </summary>
        /// <param name="viewportWidth">Width of the viewport</param>
        /// <param name="viewportHeight">Height of the viewport</param>
        /// <returns>Adjusted stroke thickness based on viewport size</returns>
        public static double CalculateStrokeThicknessFromViewportSize(double viewportWidth, double viewportHeight)
        {
            // Use a logarithmic formula to calculate an appropriate scale factor
            // This provides smooth scaling across different viewport sizes
            return Math.Pow(1.8, Math.Log2(viewportWidth + viewportHeight) - 10);
        }
    }
}
