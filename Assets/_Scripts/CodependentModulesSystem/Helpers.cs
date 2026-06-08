using UnityEngine;

namespace CodependantModules.Simulation
{
    public static class Helpers
    {
        /// <summary>
        /// Maps an input value to a smooth curve between 0 and 1.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>A value between 0 and 1.</returns>
        public static float Sigmoid(float x)
        {
            return 1f / (1f + Mathf.Exp(-x));
        }

        /// <summary>
        /// Smoothly interpolates between 0 and 1 as the input moves between two edges.
        /// </summary>
        /// <param name="edge0">The lower edge of the interpolation range.</param>
        /// <param name="edge1">The upper edge of the interpolation range.</param>
        /// <param name="x">The input value.</param>
        /// <returns>
        /// A smoothly interpolated value between 0 and 1.
        /// </returns>
        public static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Mathf.InverseLerp(edge0, edge1, x);
            return t * t * (3f - 2f * t);
        }
    }
}