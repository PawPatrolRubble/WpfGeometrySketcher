using System;

namespace Lan.Shapes.Scaling
{
    /// <summary>
    /// Provides centralized services for adjusting visual elements based on viewport scaling.
    /// This ensures consistent appearance of shapes regardless of zoom level (like AutoCAD).
    /// 
    /// The key principle: StrokeThickness = BaseThickness / Scale
    /// When zoomed in (scale > 1), thickness decreases to appear the same size on screen.
    /// When zoomed out (scale < 1), thickness increases to remain visible.
    /// </summary>
    public static class ViewportScalingService
    {
        // Default base values for visual elements (in screen pixels)
        public static double BaseStrokeThickness { get; set; } = 1.0;
        public static double BaseDragHandleSize { get; set; } = 8.0;

        /// <summary>
        /// Current viewport scale factor (updated when zoom changes)
        /// </summary>
        public static double CurrentScale { get; private set; } = 1.0;

        /// <summary>
        /// Event raised when the scale changes
        /// </summary>
        public static event Action<double>? ScaleChanged;

        /// <summary>
        /// Update the current scale and notify listeners
        /// </summary>
        public static void SetScale(double scale)
        {
            if (scale <= 0) scale = 1.0;
            
            if (Math.Abs(CurrentScale - scale) > 0.0001)
            {
                CurrentScale = scale;
                ScaleChanged?.Invoke(scale);
            }
        }

        /// <summary>
        /// Calculates the appropriate stroke thickness based on the current scale factor.
        /// This creates the AutoCAD-like effect where lines appear the same thickness
        /// regardless of zoom level.
        /// </summary>
        /// <param name="scale">The current viewport scale factor</param>
        /// <returns>Adjusted stroke thickness that maintains visual consistency at any zoom level</returns>
        public static double CalculateStrokeThickness(double scale)
        {
            // Ensure scale is valid to prevent division by zero
            if (scale <= 0)
                scale = 1.0;

            // Inverse relationship: as scale increases, thickness decreases
            return BaseStrokeThickness / scale;
        }

        /// <summary>
        /// Get stroke thickness using the current stored scale
        /// </summary>
        public static double GetCurrentStrokeThickness()
        {
            return CalculateStrokeThickness(CurrentScale);
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
        /// Get drag handle size using the current stored scale
        /// </summary>
        public static double GetCurrentDragHandleSize()
        {
            return CalculateDragHandleSize(CurrentScale);
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
